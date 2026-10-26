import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { I18nService } from '../i18n/i18n.service';
import { ToastService } from '../ui/toast.service';
import { AuthService } from './auth.service';

/**
 * Actions on units (favorite, viewing request, investment interest) need a signed-in user:
 * visitors are sent to the login page and brought back to where they were.
 */
@Injectable({ providedIn: 'root' })
export class LoginRequired {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly i18n = inject(I18nService);

  /** Returns true when the user is signed in; otherwise redirects to login and returns false. */
  check(): boolean {
    if (this.auth.isAuthenticated()) return true;

    this.toast.show(this.i18n.instant('auth.loginFirst'), 'info');
    void this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
    return false;
  }
}
