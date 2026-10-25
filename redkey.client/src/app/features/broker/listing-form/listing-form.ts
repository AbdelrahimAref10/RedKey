import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PropertyType } from '../../../core/data/models';
import { AREAS } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

type DocKey = 'photos' | 'contract' | 'ownerId';

@Component({
  selector: 'app-listing-form',
  imports: [ReactiveFormsModule, RouterLink, Icon, LocPipe],
  templateUrl: './listing-form.html',
  styleUrl: './listing-form.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ListingForm {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  protected readonly types: readonly PropertyType[] = ['apartment', 'duplex', 'villa', 'townhouse', 'studio', 'office'];
  protected readonly areas = AREAS;
  protected readonly rooms = [1, 2, 3, 4, 5];
  protected readonly docs: readonly DocKey[] = ['photos', 'contract', 'ownerId'];

  /** Selected file names (front-end only, nothing is uploaded yet). */
  protected readonly files = signal<Record<DocKey, string>>({ photos: '', contract: '', ownerId: '' });

  protected readonly form = inject(NonNullableFormBuilder).group({
    type: ['apartment' as PropertyType],
    area: [AREAS[0].id as string],
    size: [0, [Validators.required, Validators.min(20)]],
    price: [0, [Validators.required, Validators.min(1)]],
    rooms: [3],
    owner: ['', [Validators.required, Validators.maxLength(200)]]
  });

  protected pick(key: DocKey, event: Event): void {
    const input = event.target as HTMLInputElement;
    const names = Array.from(input.files ?? []).map(f => f.name).join(', ');
    this.files.update(current => ({ ...current, [key]: names }));
  }

  protected invalid(name: 'size' | 'price' | 'owner'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toast.show(this.i18n.t().common.savedLocally);
    void this.router.navigateByUrl('/broker/listings');
  }
}
