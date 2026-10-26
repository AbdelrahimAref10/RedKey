import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { Lang } from '../i18n/i18n.models';
import {
  AccountClient,
  AuthClient,
  AuthResponseDto,
  AuthUserDto,
  LoginCommand,
  RegisterCommand,
  UserRole
} from '../api/redkey-api.generated';

const STORAGE_KEY = 'rk.session';

export interface Session {
  accessToken: string;
  expiresAtUtc: string;
  user: AuthUserDto;
}

/** Where every role lands after login (customers stay on the website). */
export const ROLE_HOME: Readonly<Record<UserRole, string>> = {
  [UserRole.SuperAdmin]: '/admin',
  [UserRole.Broker]: '/broker',
  [UserRole.Customer]: '/'
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authClient = inject(AuthClient);
  private readonly accountClient = inject(AccountClient);
  private readonly router = inject(Router);

  private readonly _session = signal<Session | null>(readStoredSession());

  readonly session = this._session.asReadonly();
  readonly user = computed(() => this._session()?.user ?? null);
  readonly role = computed(() => this._session()?.user.role ?? null);
  readonly isAuthenticated = computed(() => this._session() !== null);
  /** Session language saved on the server (admin / broker API responses use it). */
  readonly language = computed<Lang>(() => (this._session()?.user.preferredLanguage === 'en' ? 'en' : 'ar'));
  readonly homeUrl = computed(() => {
    const role = this.role();
    return role === null ? '/' : ROLE_HOME[role];
  });

  login(command: LoginCommand): Observable<AuthResponseDto> {
    return this.authClient.login(command).pipe(tap(response => this.setSession(response)));
  }

  register(command: RegisterCommand): Observable<AuthResponseDto> {
    return this.authClient.register(command).pipe(tap(response => this.setSession(response)));
  }

  /**
   * Admin / broker: the API answers in the language stored in their token, so a language switch
   * saves it on the server and swaps the token. Customers send the language with every request instead.
   */
  syncLanguage(lang: Lang): void {
    const role = this.role();
    if (role === null || role === UserRole.Customer || this.language() === lang) return;
    this.accountClient.updateLanguage({ language: lang }).subscribe({
      next: response => this.setSession(response),
      error: () => undefined
    });
  }

  logout(redirect = true): void {
    this._session.set(null);
    removeStoredSession();
    if (redirect) void this.router.navigateByUrl('/');
  }

  /** Front-end only profile edit (until the profile endpoint exists). */
  updateUser(changes: Partial<Pick<AuthUserDto, 'fullName' | 'email' | 'phoneNumber'>>): void {
    const session = this._session();
    if (!session) return;
    const next: Session = { ...session, user: { ...session.user, ...changes } };
    this._session.set(next);
    storeSession(next);
  }

  accessToken(): string | null {
    const session = this._session();
    if (!session) return null;
    if (isExpired(session)) {
      this.logout(false);
      return null;
    }
    return session.accessToken;
  }

  private setSession(response: AuthResponseDto): void {
    const session: Session = {
      accessToken: response.accessToken,
      expiresAtUtc: response.expiresAtUtc,
      user: response.user
    };
    this._session.set(session);
    storeSession(session);
  }
}

function isExpired(session: Session): boolean {
  return Date.parse(session.expiresAtUtc) <= Date.now();
}

function readStoredSession(): Session | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const session = JSON.parse(raw) as Session;
    if (!session?.accessToken || !session.user || isExpired(session)) {
      removeStoredSession();
      return null;
    }
    return session;
  } catch {
    return null;
  }
}

function storeSession(session: Session): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
  } catch {
    /* storage unavailable */
  }
}

function removeStoredSession(): void {
  try {
    localStorage.removeItem(STORAGE_KEY);
  } catch {
    /* storage unavailable */
  }
}
