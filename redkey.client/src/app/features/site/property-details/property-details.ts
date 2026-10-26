import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, signal } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize, map } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import { CustomerUnitsClient, UserRole } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginRequired } from '../../../core/auth/login-required.service';
import { COMPANY } from '../../../core/data/company';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { UnitsCatalog } from '../../../core/units/units-catalog.service';
import { Icon } from '../../../shared/components/icon/icon';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

@Component({
  selector: 'app-property-details',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, ReactiveFormsModule, Icon, MoneyPipe, PropertyCard],
  templateUrl: './property-details.html',
  styleUrl: './property-details.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyDetails {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly loginRequired = inject(LoginRequired);
  private readonly auth = inject(AuthService);
  private readonly catalog = inject(UnitsCatalog);
  private readonly customerUnits = inject(CustomerUnitsClient);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly i18n = inject(I18nService);

  /** Route param. */
  readonly id = input.required<string>();

  protected readonly details = rxResource({
    params: () => ({ id: this.id(), lang: this.i18n.lang() }),
    stream: ({ params }) => this.catalog.unit(params.id, params.lang)
  });
  protected readonly unit = computed(() => this.details.value());

  /** Detail-page images (the 3 uploaded ones; the card image when there are none). */
  protected readonly gallery = computed(() => {
    const d = this.unit();
    if (!d) return [];
    const images = d.images.length ? d.images : [d.unit.coverImageUrl];
    return [images[0], images[1] ?? images[0], images[2] ?? images[1] ?? images[0]];
  });

  private readonly lookups = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.lookups(params) });
  protected readonly preferredTimes = computed(() => this.lookups.value()?.preferredTimes ?? []);

  /** Same purpose, other units. */
  protected readonly similar = rxResource({
    params: () => {
      const d = this.unit();
      return d ? { purpose: d.unit.purpose, id: d.unit.id, lang: this.i18n.lang() } : undefined;
    },
    stream: ({ params }) =>
      this.catalog.units({ purpose: params.purpose, pageSize: 4 }, params.lang)
        .pipe(map(page => page.items.filter(u => u.id !== params.id).slice(0, 3)))
  });

  protected readonly whatsappLink = computed(() => {
    const code = this.unit()?.unit.code;
    return `${COMPANY.whatsappLink}?text=${encodeURIComponent(code ? `RedKey · ${code}` : 'RedKey')}`;
  });

  private readonly user = this.auth.user();
  protected readonly sending = signal(false);

  protected readonly form = inject(NonNullableFormBuilder).group({
    name: [this.user?.fullName ?? '', [Validators.required, Validators.maxLength(150)]],
    phone: [this.user?.phoneNumber ?? '', [Validators.required, Validators.pattern(/^\+?[0-9 ]{8,18}$/)]],
    time: [0, [Validators.min(1)]]
  });

  protected whatsapp(event: Event): void {
    event.preventDefault();
    if (!this.loginRequired.check()) return;
    window.open(this.whatsappLink(), '_blank', 'noopener');
  }

  protected invalid(name: 'name' | 'phone' | 'time'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (!this.loginRequired.check()) return;

    if (this.auth.role() !== UserRole.Customer) {
      this.toast.show(this.i18n.instant('units.customersOnly'), 'info');
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    this.sending.set(true);
    this.customerUnits
      .requestViewing(this.id(), { name: v.name.trim(), phoneNumber: v.phone.trim(), preferredTime: v.time })
      .pipe(
        finalize(() => this.sending.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.toast.show(this.i18n.instant('units.viewingSent'));
          void this.router.navigateByUrl('/request-sent');
        },
        error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
      });
  }
}
