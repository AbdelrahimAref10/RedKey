import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, numberAttribute } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { findOpportunity } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-opportunity-details',
  imports: [NgOptimizedImage, RouterLink, ReactiveFormsModule, Icon, LocPipe],
  templateUrl: './opportunity-details.html',
  styleUrl: './opportunity-details.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OpportunityDetails {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  readonly base = input('');
  readonly id = input(0, { transform: numberAttribute });

  protected readonly opportunity = computed(() => findOpportunity(this.id()));
  protected readonly fmt = new Intl.NumberFormat('en-US');

  private readonly user = inject(AuthService).user();

  protected readonly form = inject(NonNullableFormBuilder).group({
    name: [this.user?.fullName ?? '', [Validators.required, Validators.maxLength(150)]],
    phone: [this.user?.phoneNumber ?? '', [Validators.required, Validators.pattern(/^\+?[0-9 ]{8,18}$/)]],
    amount: [1]
  });

  protected invalid(name: 'name' | 'phone'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toast.show(this.i18n.t().common.sentLocally);
    void this.router.navigateByUrl(this.base() + '/request-sent');
  }
}
