import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { UserRole } from '../api/redkey-api.generated';
import { AuthService } from './auth.service';

/**
 * Only lets the given role match the route. `canMatch` (instead of `canActivate`)
 * also prevents downloading the lazy chunk of a layout the user can't open.
 */
export function roleGuard(role: UserRole): CanMatchFn {
  return (_route, segments) => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.isAuthenticated() || auth.accessToken() === null) {
      const returnUrl = '/' + segments.map(s => s.path).join('/');
      return router.createUrlTree(['/login'], { queryParams: { returnUrl } });
    }

    return auth.role() === role ? true : router.parseUrl(auth.homeUrl());
  };
}

/** Login / register pages: a signed-in user goes straight to their layout. */
export const guestGuard: CanMatchFn = () => {
  const auth = inject(AuthService);
  return auth.isAuthenticated() ? inject(Router).parseUrl(auth.homeUrl()) : true;
};
