import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, output, signal } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, finalize } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import { AdminCitiesClient, AdminUnitTypesClient, LookupDeleteResultDto } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ConfirmService } from '../../../core/ui/confirm.service';
import { readImageAsDataUrl } from '../../../core/ui/image-file';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';

export type LookupKind = 'unitTypes' | 'cities';

export interface LookupRow {
  id: number;
  nameAr: string;
  nameEn: string;
  sortOrder: number;
  isActive: boolean;
  iconUrl?: string;
  areasCount?: number;
}

/**
 * Super admin CRUD of the unit types (with icons) and the cities — same screen, kind from the route / host.
 * Cities rows are selectable: the cities page shows the selected city's areas under the table.
 */
@Component({
  selector: 'app-lookup-admin',
  imports: [TranslatePipe, ReactiveFormsModule, Icon, Pagination],
  templateUrl: './lookup-admin.html',
  styleUrl: './lookup-admin.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LookupAdmin {
  private readonly types = inject(AdminUnitTypesClient);
  private readonly cities = inject(AdminCitiesClient);
  private readonly toast = inject(ToastService);
  private readonly confirm = inject(ConfirmService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly i18n = inject(I18nService);

  /** Route data (unit types) or the cities page. */
  readonly kind = input<LookupKind>('unitTypes');
  /** Cities: the highlighted row. */
  readonly selectedId = input<number | null>(null);
  /** Bump to reload the list (e.g. after the areas of a city changed). */
  readonly reloadKey = input(0);

  /** Cities: a row was clicked (null when the selected city was deleted). */
  readonly selectRow = output<LookupRow | null>();

  protected readonly withIcon = computed(() => this.kind() === 'unitTypes');
  protected readonly isCities = computed(() => this.kind() === 'cities');
  protected readonly page = signal(1);
  protected readonly editingId = signal<number | null>(null);
  protected readonly formOpen = signal(false);
  protected readonly saving = signal(false);
  protected readonly icon = signal<string | null>(null);

  /** Translation key prefix of the texts that differ per kind. */
  protected readonly texts = computed(() => (this.isCities() ? 'lookupsAdmin.cities' : 'lookupsAdmin.unitTypes'));

  protected readonly list = rxResource({
    params: () => ({ kind: this.kind(), page: this.page(), reload: this.reloadKey() }),
    stream: ({ params }): Observable<{ items: LookupRow[]; page: number; totalPages?: number }> =>
      params.kind === 'unitTypes'
        ? this.types.getUnitTypes(null, params.page, null)
        : this.cities.getCities(null, params.page, null)
  });

  protected readonly form = inject(NonNullableFormBuilder).group({
    nameAr: ['', [Validators.required, Validators.maxLength(100)]],
    nameEn: ['', [Validators.required, Validators.maxLength(100)]],
    sortOrder: [0],
    isActive: [true]
  });

  protected openNew(): void {
    this.editingId.set(null);
    this.form.reset({ nameAr: '', nameEn: '', sortOrder: (this.list.value()?.items.length ?? 0) + 1, isActive: true });
    this.icon.set(null);
    this.formOpen.set(true);
  }

  protected edit(row: LookupRow, event?: Event): void {
    event?.stopPropagation();
    this.editingId.set(row.id);
    this.form.reset({ nameAr: row.nameAr, nameEn: row.nameEn, sortOrder: row.sortOrder, isActive: row.isActive });
    this.icon.set(row.iconUrl ?? null);
    this.formOpen.set(true);
  }

  protected cancel(): void {
    this.formOpen.set(false);
    this.editingId.set(null);
  }

  protected select(row: LookupRow): void {
    if (this.isCities()) this.selectRow.emit(row);
  }

  protected name(row: LookupRow): string {
    return this.i18n.lang() === 'ar' ? row.nameAr : row.nameEn;
  }

  protected async pickIcon(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) this.icon.set(await readImageAsDataUrl(file, 256));
    input.value = '';
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const id = this.editingId();
    const icon = this.icon() ?? undefined;
    const body = { nameAr: v.nameAr.trim(), nameEn: v.nameEn.trim(), sortOrder: v.sortOrder };

    const request: Observable<unknown> = this.withIcon()
      ? id ? this.types.update(id, { ...body, isActive: v.isActive, icon }) : this.types.create({ ...body, icon })
      : id ? this.cities.update(id, { ...body, isActive: v.isActive }) : this.cities.create(body);

    this.saving.set(true);
    request
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: created => {
          this.toast.show(this.i18n.instant('lookupsAdmin.saved'));
          this.cancel();
          this.list.reload();
          // A new city opens right away so its areas can be added.
          if (this.isCities() && !id && typeof created === 'number') {
            this.selectRow.emit({ id: created, ...body, isActive: true, areasCount: 0 });
          }
        },
        error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
      });
  }

  protected async remove(row: LookupRow, event?: Event): Promise<void> {
    event?.stopPropagation();
    const confirmed = await this.confirm.ask({
      title: this.i18n.instant('lookupsAdmin.deleteTitle', { name: this.name(row) }),
      message: this.i18n.instant(`${this.texts()}.deleteConfirm`),
      confirmText: this.i18n.instant('lookupsAdmin.delete'),
      tone: 'danger'
    });
    if (!confirmed) return;

    const request: Observable<LookupDeleteResultDto> = this.withIcon() ? this.types.delete(row.id) : this.cities.delete(row.id);
    request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: result => {
        // Deleted when no units use it, otherwise only deactivated.
        this.toast.show(this.i18n.instant(result.deleted ? 'lookupsAdmin.deleted' : 'lookupsAdmin.deactivated'), result.deleted ? 'success' : 'info');
        if (result.deleted && row.id === this.selectedId()) this.selectRow.emit(null);
        this.list.reload();
      },
      error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
    });
  }
}
