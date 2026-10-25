import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserRole } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

/** Profile page shared by the customer, broker and admin layouts (front-end only for now). */
@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule, Icon],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Profile {
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  protected readonly user = this.auth.user;
  protected readonly editing = signal(false);

  protected readonly initials = computed(() =>
    (this.user()?.fullName ?? '?')
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0])
      .join('')
      .toUpperCase()
  );

  protected readonly roleLabel = computed(() => {
    const roles = this.i18n.t().roles;
    switch (this.user()?.role) {
      case UserRole.SuperAdmin: return roles.superAdmin;
      case UserRole.Broker: return roles.broker;
      default: return roles.customer;
    }
  });

  protected readonly form = inject(NonNullableFormBuilder).group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.pattern(/^\+?[0-9]{8,15}$/)]],
    city: [''],
    address: [''],
    birthDate: ['']
  });

  protected readonly passwordForm = inject(NonNullableFormBuilder).group({
    current: ['', Validators.required],
    next: ['', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,128}$/)]]
  });

  constructor() {
    this.reset();
  }

  protected startEdit(): void {
    this.editing.set(true);
    this.form.enable();
  }

  protected cancel(): void {
    this.reset();
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    this.auth.updateUser({ fullName: v.fullName.trim(), email: v.email.trim(), phoneNumber: v.phoneNumber.trim() || undefined });
    this.toast.show(this.i18n.t().common.savedLocally);
    this.editing.set(false);
    this.form.disable();
  }

  protected changePassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    this.passwordForm.reset();
    this.toast.show(this.i18n.t().common.savedLocally);
  }

  protected invalid(name: 'fullName' | 'email' | 'phoneNumber'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  private reset(): void {
    const user = this.user();
    this.form.reset({
      fullName: user?.fullName ?? '',
      email: user?.email ?? '',
      phoneNumber: user?.phoneNumber ?? '',
      city: '',
      address: '',
      birthDate: ''
    });
    this.form.disable();
    this.editing.set(false);
  }
}
