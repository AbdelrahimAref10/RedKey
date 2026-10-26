import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, input, signal, untracked } from '@angular/core';
import { rxResource, takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormArray, FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, finalize } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import {
  AdminUnitsClient,
  BrokerUnitsClient,
  CreateUnitCommand,
  FeatureInput,
  PaymentPlanInput,
  UnitEditDto,
  UnitFormLookupsDto
} from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { COMPANY } from '../../../core/data/company';
import { I18nService } from '../../../core/i18n/i18n.service';
import { readImageAsDataUrl } from '../../../core/ui/image-file';
import { ConfirmService } from '../../../core/ui/confirm.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';
import { UnitJournalDrawer } from '../../admin/unit-journal-drawer/unit-journal-drawer';

export type UnitEditorMode = 'admin' | 'broker';

const MAX_GALLERY = 3;
const IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp'];

/** Only the photo types the API accepts (drag & drop can bring anything). */
function images(files: FileList | null): File[] {
  return Array.from(files ?? []).filter(f => IMAGE_TYPES.includes(f.type));
}

/** Same API surface for both dashboards (the back-end runs the same commands). */
interface UnitsApi {
  getFormLookups(): Observable<UnitFormLookupsDto>;
  getUnitForEdit(id: string): Observable<UnitEditDto>;
  create(command: CreateUnitCommand): Observable<string>;
  update(id: string, command: CreateUnitCommand): Observable<void>;
  delete(id: string): Observable<void>;
}

/**
 * Create / edit a unit. Used by the super admin (published right away, can review broker units)
 * and by brokers (every save goes to review). Mode comes from the route data.
 */
@Component({
  selector: 'app-unit-editor',
  imports: [TranslatePipe, ReactiveFormsModule, RouterLink, Icon, UnitJournalDrawer],
  templateUrl: './unit-editor.html',
  styleUrl: './unit-editor.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UnitEditor {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly confirm = inject(ConfirmService);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly adminUnits = inject(AdminUnitsClient);
  private readonly brokerUnits = inject(BrokerUnitsClient);
  protected readonly i18n = inject(I18nService);

  /** Route data. */
  readonly mode = input<UnitEditorMode>('admin');
  /** Route param (absent = new unit). */
  readonly id = input<string>();

  protected readonly isAdmin = computed(() => this.mode() === 'admin');
  protected readonly listUrl = computed(() => (this.isAdmin() ? '/admin/units' : '/broker/listings'));
  private get api(): UnitsApi {
    return this.isAdmin() ? this.adminUnits : this.brokerUnits;
  }

  // Dropdowns (types, cities, areas, enums) in the session language.
  protected readonly lookups = rxResource({
    params: () => ({ mode: this.mode(), lang: this.auth.language() }),
    stream: () => this.api.getFormLookups()
  });

  // The unit being edited.
  protected readonly unit = rxResource({
    params: () => {
      const id = this.id();
      return id ? { id, lang: this.auth.language() } : undefined;
    },
    stream: ({ params }) => this.api.getUnitForEdit(params.id)
  });

  /** Super admin: the unit journal drawer. */
  protected readonly journalOpen = signal(false);
  protected readonly cover = signal<string | null>(null);
  protected readonly gallery = signal<string[]>([]);
  protected readonly saving = signal(false);
  protected readonly reviewing = signal(false);
  private readonly errorCode = signal<string | null>(null);
  protected readonly error = computed(() => {
    const code = this.errorCode();
    return code ? this.i18n.error(code) : null;
  });

  protected readonly whatsappLink = computed(() => {
    const code = this.unit.value()?.code;
    return `${COMPANY.whatsappLink}?text=${encodeURIComponent(code ? `RedKey · ${code}` : 'RedKey')}`;
  });

  protected readonly form = this.fb.group({
    unitTypeId: this.fb.control<number | null>(null, Validators.required),
    /** Only filters the areas list; the API takes the city from the area. */
    cityId: this.fb.control<number | null>(null, Validators.required),
    areaId: this.fb.control<number | null>(null, Validators.required),
    purpose: this.fb.control<number | null>(null, Validators.required),
    finishing: this.fb.control<number | null>(null, Validators.required),
    titleAr: ['', [Validators.required, Validators.maxLength(200)]],
    titleEn: ['', [Validators.required, Validators.maxLength(200)]],
    compoundAr: ['', [Validators.required, Validators.maxLength(150)]],
    compoundEn: ['', [Validators.required, Validators.maxLength(150)]],
    price: this.fb.control<number | null>(null, [Validators.required, Validators.min(1)]),
    size: this.fb.control<number | null>(null, [Validators.required, Validators.min(1)]),
    bedrooms: [0, [Validators.min(0), Validators.max(50)]],
    bathrooms: [0, [Validators.min(0), Validators.max(50)]],
    brokerCommission: [0, [Validators.min(0), Validators.max(100)]],
    /** Super admin only (the field is hidden for brokers and the API ignores theirs). */
    viewingFee: [0, [Validators.min(0), Validators.max(1_000_000)]],
    isFeatured: [false],
    descriptionAr: ['', [Validators.required, Validators.maxLength(4000)]],
    descriptionEn: ['', [Validators.required, Validators.maxLength(4000)]],
    deliveryAr: ['', Validators.maxLength(100)],
    deliveryEn: ['', Validators.maxLength(100)],
    paymentPlans: this.fb.array<FormGroup>([]),
    features: this.fb.array<FormGroup>([])
  });

  private readonly cityId = toSignal(this.form.controls.cityId.valueChanges, { initialValue: null });
  /** Areas of the selected city. */
  protected readonly cityAreas = computed(() => {
    const cityId = this.cityId();
    return cityId === null ? [] : (this.lookups.value()?.areas ?? []).filter(a => a.cityId === cityId);
  });

  protected readonly reviewForm = this.fb.group({
    decision: this.fb.control<number | null>(null, Validators.required),
    note: ['', Validators.maxLength(1000)]
  });

  get plans(): FormArray<FormGroup> {
    return this.form.controls.paymentPlans;
  }

  get features(): FormArray<FormGroup> {
    return this.form.controls.features;
  }

  constructor() {
    // Fill the form once the unit is loaded (edit mode); later reloads (after a review) keep the user's edits.
    let filledFor: string | null = null;
    effect(() => {
      const dto = this.unit.value();
      if (dto && dto.id !== filledFor) {
        filledFor = dto.id;
        untracked(() => this.fill(dto));
      }
    });
  }

  private fill(dto: UnitEditDto): void {
    this.form.patchValue({
      unitTypeId: dto.unitTypeId,
      cityId: dto.cityId,
      areaId: dto.areaId,
      purpose: dto.purpose,
      finishing: dto.finishing,
      titleAr: dto.titleAr,
      titleEn: dto.titleEn,
      compoundAr: dto.compoundAr,
      compoundEn: dto.compoundEn,
      price: dto.price,
      size: dto.size,
      bedrooms: dto.bedrooms,
      bathrooms: dto.bathrooms,
      brokerCommission: dto.brokerCommission,
      viewingFee: dto.viewingFee,
      isFeatured: dto.isFeatured,
      descriptionAr: dto.descriptionAr,
      descriptionEn: dto.descriptionEn,
      deliveryAr: dto.deliveryAr ?? '',
      deliveryEn: dto.deliveryEn ?? ''
    });
    this.plans.clear();
    dto.paymentPlans.forEach(p => this.plans.push(this.planGroup(p.labelAr, p.labelEn, p.valueAr, p.valueEn)));
    this.features.clear();
    dto.features.forEach(f => this.features.push(this.featureGroup(f.nameAr, f.nameEn)));
    this.cover.set(dto.coverImageUrl);
    this.gallery.set([...dto.galleryImages]);
  }

  protected planGroup(labelAr = '', labelEn = '', valueAr = '', valueEn = ''): FormGroup {
    return this.fb.group({
      labelAr: [labelAr, [Validators.required, Validators.maxLength(150)]],
      labelEn: [labelEn, [Validators.required, Validators.maxLength(150)]],
      valueAr: [valueAr, [Validators.required, Validators.maxLength(150)]],
      valueEn: [valueEn, [Validators.required, Validators.maxLength(150)]]
    });
  }

  protected featureGroup(nameAr = '', nameEn = ''): FormGroup {
    return this.fb.group({
      nameAr: [nameAr, [Validators.required, Validators.maxLength(100)]],
      nameEn: [nameEn, [Validators.required, Validators.maxLength(100)]]
    });
  }

  protected invalid(name: string): boolean {
    const control = this.form.get(name);
    return !!control && control.invalid && control.touched;
  }

  /** Which drop zone a file is dragged over (highlight). */
  protected readonly dragOver = signal<'cover' | 'gallery' | null>(null);
  protected readonly maxGallery = MAX_GALLERY;
  /** Always 3 slots: an image or an empty slot. */
  protected readonly gallerySlots = computed(() =>
    Array.from({ length: MAX_GALLERY }, (_, i) => this.gallery()[i] ?? null));

  protected async pickCover(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    await this.setCover(input.files);
    input.value = '';
  }

  protected async pickGallery(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    await this.addToGallery(input.files);
    input.value = '';
  }

  protected onDragOver(event: DragEvent, zone: 'cover' | 'gallery'): void {
    event.preventDefault();
    this.dragOver.set(zone);
  }

  protected async onDrop(event: DragEvent, zone: 'cover' | 'gallery'): Promise<void> {
    event.preventDefault();
    this.dragOver.set(null);
    const files = event.dataTransfer?.files ?? null;
    await (zone === 'cover' ? this.setCover(files) : this.addToGallery(files));
  }

  protected removeCover(): void {
    this.cover.set(null);
  }

  protected removeGalleryImage(index: number): void {
    this.gallery.update(list => list.filter((_, i) => i !== index));
  }

  protected readonly canAddImage = computed(() => this.gallery().length < MAX_GALLERY);

  private async setCover(files: FileList | null): Promise<void> {
    const file = images(files)[0];
    if (file) this.cover.set(await readImageAsDataUrl(file));
  }

  private async addToGallery(files: FileList | null): Promise<void> {
    const free = MAX_GALLERY - this.gallery().length;
    const picked = images(files).slice(0, free);
    const urls = await Promise.all(picked.map(f => readImageAsDataUrl(f)));
    this.gallery.update(list => [...list, ...urls]);
  }

  protected save(): void {
    const cover = this.cover();
    if (this.form.invalid || !cover) {
      this.form.markAllAsTouched();
      this.errorCode.set(cover ? 'validation.required' : 'units.coverRequired');
      return;
    }

    const v = this.form.getRawValue();
    const command: CreateUnitCommand = {
      unitTypeId: v.unitTypeId!,
      areaId: v.areaId!,
      purpose: v.purpose!,
      finishing: v.finishing!,
      titleAr: v.titleAr,
      titleEn: v.titleEn,
      compoundAr: v.compoundAr,
      compoundEn: v.compoundEn,
      price: v.price!,
      size: v.size!,
      bedrooms: v.bedrooms,
      bathrooms: v.bathrooms,
      brokerCommission: v.brokerCommission,
      viewingFee: this.isAdmin() ? v.viewingFee : undefined,
      isFeatured: this.isAdmin() && v.isFeatured,
      descriptionAr: v.descriptionAr,
      descriptionEn: v.descriptionEn,
      deliveryAr: v.deliveryAr || undefined,
      deliveryEn: v.deliveryEn || undefined,
      coverImage: cover,
      galleryImages: this.gallery(),
      paymentPlans: this.plans.getRawValue() as PaymentPlanInput[],
      features: this.features.getRawValue() as FeatureInput[]
    };

    const id = this.id();
    const request: Observable<unknown> = id ? this.api.update(id, command) : this.api.create(command);

    this.saving.set(true);
    this.errorCode.set(null);
    request
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.toast.show(this.i18n.instant('unitForm.saved'));
          void this.router.navigateByUrl(this.listUrl());
        },
        error: error => this.errorCode.set(apiErrorCode(error))
      });
  }

  protected review(): void {
    const id = this.id();
    if (!id || this.reviewForm.invalid) {
      this.reviewForm.markAllAsTouched();
      return;
    }

    const v = this.reviewForm.getRawValue();
    this.reviewing.set(true);
    this.adminUnits
      .review(id, { decision: v.decision!, note: v.note.trim() || undefined })
      .pipe(
        finalize(() => this.reviewing.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.toast.show(this.i18n.instant('unitForm.reviewed'));
          this.reviewForm.reset();
          this.unit.reload();
        },
        error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
      });
  }

  protected async remove(): Promise<void> {
    const id = this.id();
    if (!id) return;
    const confirmed = await this.confirm.ask({
      title: this.i18n.instant('unitForm.deleteTitle'),
      message: this.i18n.instant('unitForm.deleteConfirm'),
      confirmText: this.i18n.instant('unitForm.delete'),
      tone: 'danger'
    });
    if (!confirmed) return;

    this.api.delete(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.toast.show(this.i18n.instant('unitForm.deleted'));
        void this.router.navigateByUrl(this.listUrl());
      },
      error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
    });
  }
}
