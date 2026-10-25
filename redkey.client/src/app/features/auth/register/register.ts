import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  input,
  signal
} from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import { BrokerType, UserRole } from '../../../core/api/redkey-api.generated';
import { AuthService, ROLE_HOME } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';
import { AuthShell } from '../auth-shell/auth-shell';

const PHONE_PATTERN = /^\+?[0-9]{8,15}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,128}$/;

function passwordsMatch(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password && confirm && password !== confirm ? { passwordMismatch: true } : null;
}

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink, Icon, AuthShell],
  templateUrl: './register.html',
  styleUrl: './register.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Register implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly i18n = inject(I18nService);

  protected readonly UserRole = UserRole;
  protected readonly BrokerType = BrokerType;

  /** ?role=broker preselects the broker tab (landing "Join as a broker" button). */
  readonly role = input<string>();

  protected readonly form = inject(NonNullableFormBuilder).group(
    {
      role: [UserRole.Customer as UserRole.Customer | UserRole.Broker],
      fullName: ['', [Validators.required, Validators.maxLength(150)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(PHONE_PATTERN)]],
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
      city: ['', [Validators.maxLength(100)]],
      brokerType: [BrokerType.Independent],
      companyName: ['', [Validators.maxLength(150)]],
      nationalId: ['', [Validators.maxLength(20)]],
      terms: [false, [Validators.requiredTrue]]
    },
    { validators: passwordsMatch }
  );

  private readonly formValue = toSignal(this.form.valueChanges, { initialValue: this.form.getRawValue() });
  protected readonly isBroker = computed(() => this.formValue().role === UserRole.Broker);
  protected readonly isCompany = computed(
    () => this.isBroker() && this.formValue().brokerType === BrokerType.Company
  );

  protected readonly loading = signal(false);
  private readonly errorCode = signal<string | null>(null);
  protected readonly error = computed(() => {
    const code = this.errorCode();
    return code ? this.i18n.error(code) : null;
  });

  ngOnInit(): void {
    if (this.role() === 'broker') this.setRole(UserRole.Broker);
  }

  protected setRole(role: UserRole.Customer | UserRole.Broker): void {
    this.form.controls.role.setValue(role);
  }

  protected invalid(name: 'fullName' | 'email' | 'phoneNumber' | 'password' | 'confirmPassword' | 'companyName' | 'terms'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected get mismatch(): boolean {
    return this.form.hasError('passwordMismatch') && this.form.controls.confirmPassword.touched;
  }

  protected submit(): void {
    const v = this.form.getRawValue();
    const companyMissing = this.isCompany() && !v.companyName.trim();

    if (this.form.invalid || companyMissing) {
      this.form.markAllAsTouched();
      if (companyMissing) this.form.controls.companyName.setErrors({ required: true });
      return;
    }

    this.loading.set(true);
    this.errorCode.set(null);

    const isBroker = v.role === UserRole.Broker;
    this.auth
      .register({
        role: v.role,
        fullName: v.fullName.trim(),
        email: v.email.trim(),
        phoneNumber: v.phoneNumber.trim(),
        password: v.password,
        confirmPassword: v.confirmPassword,
        city: v.city.trim() || undefined,
        preferredLanguage: this.i18n.lang(),
        brokerType: isBroker ? v.brokerType : undefined,
        companyName: isBroker && v.brokerType === BrokerType.Company ? v.companyName.trim() : undefined,
        nationalId: isBroker ? v.nationalId.trim() || undefined : undefined
      })
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: response => void this.router.navigateByUrl(ROLE_HOME[response.user.role]),
        error: error => this.errorCode.set(apiErrorCode(error))
      });
  }
}
