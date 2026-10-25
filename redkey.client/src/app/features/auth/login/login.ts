import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import { AuthService, ROLE_HOME } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';
import { AuthShell } from '../auth-shell/auth-shell';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, Icon, AuthShell],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly i18n = inject(I18nService);

  /** ?returnUrl=... (set by the role guard). */
  readonly returnUrl = input<string>();

  protected readonly form = inject(NonNullableFormBuilder).group({
    identifier: ['', [Validators.required, Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.maxLength(128)]]
  });

  protected readonly loading = signal(false);
  protected readonly showPassword = signal(false);
  private readonly errorCode = signal<string | null>(null);
  protected readonly error = computed(() => {
    const code = this.errorCode();
    return code ? this.i18n.error(code) : null;
  });

  protected invalid(name: 'identifier' | 'password'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorCode.set(null);

    this.auth
      .login(this.form.getRawValue())
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: response => {
          const home = ROLE_HOME[response.user.role];
          const returnUrl = this.returnUrl();
          void this.router.navigateByUrl(returnUrl?.startsWith(home) ? returnUrl : home);
        },
        error: error => this.errorCode.set(apiErrorCode(error))
      });
  }
}
