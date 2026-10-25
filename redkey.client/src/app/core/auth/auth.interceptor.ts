import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { I18nService } from '../i18n/i18n.service';
import { AuthService } from './auth.service';

/**
 * Adds the bearer token + Accept-Language to the generated API client requests,
 * and signs the user out when the API answers 401 on a protected endpoint.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const i18n = inject(I18nService);

  const token = auth.accessToken();
  const isAuthEndpoint = req.url.includes('/api/auth/');

  const request = req.clone({
    setHeaders: {
      'Accept-Language': i18n.lang(),
      ...(token && !isAuthEndpoint ? { Authorization: `Bearer ${token}` } : {})
    }
  });

  return next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && !isAuthEndpoint) {
        auth.logout();
      }
      return throwError(() => error);
    })
  );
};
