import { ChangeDetectionStrategy, Component, DestroyRef, effect, inject, input, output, signal, untracked } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { concatMap, finalize, from, toArray } from 'rxjs';
import { apiErrorCode } from '../../../../core/api/api-error';
import { AdminAreasClient, AreaAdminDto } from '../../../../core/api/redkey-api.generated';
import { I18nService } from '../../../../core/i18n/i18n.service';
import { ConfirmService } from '../../../../core/ui/confirm.service';
import { ToastService } from '../../../../core/ui/toast.service';
import { Icon } from '../../../../shared/components/icon/icon';
import { Pagination } from '../../../../shared/components/pagination/pagination';
import { LookupRow } from '../../lookup-admin/lookup-admin';

/** The areas of the selected city: list, add several at once, edit, delete. */
@Component({
  selector: 'app-areas-panel',
  imports: [TranslatePipe, ReactiveFormsModule, Icon, Pagination],
  templateUrl: './areas-panel.html',
  styleUrl: './areas-panel.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AreasPanel {
  private readonly areas = inject(AdminAreasClient);
  private readonly toast = inject(ToastService);
  private readonly confirm = inject(ConfirmService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(NonNullableFormBuilder);
  protected readonly i18n = inject(I18nService);

  readonly city = input.required<LookupRow>();
  /** Areas were added / deleted (the cities table refreshes its count). */
  readonly changed = output<void>();
  readonly close = output<void>();

  protected readonly page = signal(1);
  protected readonly saving = signal(false);
  protected readonly editingId = signal<number | null>(null);

  protected readonly list = rxResource({
    params: () => ({ cityId: this.city().id, page: this.page() }),
    stream: ({ params }) => this.areas.getAreas(params.cityId, null, params.page, null)
  });

  /** New areas: one row per area, saved together. */
  protected readonly newRows = this.fb.array([this.newRow()]);

  protected readonly editForm = this.fb.group({
    nameAr: ['', [Validators.required, Validators.maxLength(100)]],
    nameEn: ['', [Validators.required, Validators.maxLength(100)]],
    sortOrder: [0],
    isActive: [true]
  });

  constructor() {
    // Another city picked → start clean.
    effect(() => {
      this.city();
      untracked(() => {
        this.page.set(1);
        this.editingId.set(null);
        this.resetNewRows();
      });
    });
  }

  protected cityName(): string {
    const c = this.city();
    return this.i18n.lang() === 'ar' ? c.nameAr : c.nameEn;
  }

  protected addRow(): void {
    this.newRows.push(this.newRow());
  }

  protected removeRow(index: number): void {
    if (this.newRows.length > 1) this.newRows.removeAt(index);
  }

  protected saveNew(): void {
    if (this.newRows.invalid) {
      this.newRows.markAllAsTouched();
      return;
    }

    const cityId = this.city().id;
    const start = this.list.value()?.totalCount ?? 0;
    const rows = this.newRows.getRawValue().map((r, i) => ({
      cityId,
      nameAr: r.nameAr.trim(),
      nameEn: r.nameEn.trim(),
      sortOrder: start + i + 1
    }));

    this.saving.set(true);
    from(rows)
      .pipe(
        concatMap(row => this.areas.create(row)),
        toArray(),
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.toast.show(this.i18n.instant('lookupsAdmin.areas.added', { count: rows.length }));
          this.resetNewRows();
          this.list.reload();
          this.changed.emit();
        },
        error: error => {
          this.toast.show(this.i18n.error(apiErrorCode(error)), 'error');
          this.list.reload();
          this.changed.emit();
        }
      });
  }

  protected edit(row: AreaAdminDto): void {
    this.editingId.set(row.id);
    this.editForm.reset({ nameAr: row.nameAr, nameEn: row.nameEn, sortOrder: row.sortOrder, isActive: row.isActive });
  }

  protected cancelEdit(): void {
    this.editingId.set(null);
  }

  protected saveEdit(): void {
    const id = this.editingId();
    if (!id) return;
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const v = this.editForm.getRawValue();
    this.saving.set(true);
    this.areas
      .update(id, { nameAr: v.nameAr.trim(), nameEn: v.nameEn.trim(), sortOrder: v.sortOrder, isActive: v.isActive })
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.toast.show(this.i18n.instant('lookupsAdmin.saved'));
          this.editingId.set(null);
          this.list.reload();
        },
        error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
      });
  }

  protected async remove(row: AreaAdminDto): Promise<void> {
    const confirmed = await this.confirm.ask({
      title: this.i18n.instant('lookupsAdmin.deleteTitle', { name: this.i18n.lang() === 'ar' ? row.nameAr : row.nameEn }),
      message: this.i18n.instant('lookupsAdmin.areas.deleteConfirm'),
      confirmText: this.i18n.instant('lookupsAdmin.delete'),
      tone: 'danger'
    });
    if (!confirmed) return;

    this.areas.delete(row.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: result => {
        // Deleted when no units use it, otherwise only deactivated.
        this.toast.show(this.i18n.instant(result.deleted ? 'lookupsAdmin.deleted' : 'lookupsAdmin.deactivated'), result.deleted ? 'success' : 'info');
        this.list.reload();
        this.changed.emit();
      },
      error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
    });
  }

  private newRow() {
    return this.fb.group({
      nameAr: ['', [Validators.required, Validators.maxLength(100)]],
      nameEn: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  private resetNewRows(): void {
    this.newRows.clear();
    this.newRows.push(this.newRow());
  }
}
