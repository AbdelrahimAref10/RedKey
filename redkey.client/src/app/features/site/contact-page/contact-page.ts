import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginRequired } from '../../../core/auth/login-required.service';
import { COMPANY } from '../../../core/data/company';
import { IMAGES } from '../../../core/data/images';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-contact-page',
  imports: [NgOptimizedImage, ReactiveFormsModule, Icon],
  templateUrl: './contact-page.html',
  styleUrl: './contact-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContactPage {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly loginRequired = inject(LoginRequired);
  protected readonly i18n = inject(I18nService);

  protected readonly company = COMPANY;
  protected readonly image = IMAGES.aboutTeam;
  private readonly user = inject(AuthService).user();

  protected readonly form = inject(NonNullableFormBuilder).group({
    name: [this.user?.fullName ?? '', [Validators.required, Validators.maxLength(150)]],
    phone: [this.user?.phoneNumber ?? '', [Validators.required, Validators.pattern(/^\+?[0-9 ]{8,18}$/)]],
    subject: [0],
    message: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  protected invalid(name: 'name' | 'phone' | 'message'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (!this.loginRequired.check()) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toast.show(this.i18n.t().common.sentLocally);
    void this.router.navigateByUrl('/request-sent');
  }
}
