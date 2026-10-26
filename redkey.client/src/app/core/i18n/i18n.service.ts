import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable, map } from 'rxjs';
import { Lang, Loc } from './i18n.models';

const STORAGE_KEY = 'rk.lang';

/**
 * Current language + direction on top of ngx-translate. Texts live in `assets/i18n/{ar,en}.json`;
 * templates use `'key' | translate`, code uses `instant('key')`.
 */
@Injectable({ providedIn: 'root' })
export class I18nService {
  private readonly document = inject(DOCUMENT);
  private readonly translate = inject(TranslateService);

  /** Only changes after the language file is loaded, so computeds that call `instant` re-run with the new texts. */
  private readonly _lang = signal<Lang>(readStoredLang());

  readonly lang = this._lang.asReadonly();
  readonly dir = computed(() => (this._lang() === 'ar' ? 'rtl' : 'ltr'));
  readonly isRtl = computed(() => this._lang() === 'ar');

  /** Loads the stored language before the app renders (called from the app initializer). */
  init(): Observable<unknown> {
    const lang = this._lang();
    this.applyToDocument(lang);
    return this.translate.use(lang);
  }

  setLang(lang: Lang): void {
    if (lang === this._lang()) return;
    this.translate.use(lang).pipe(map(() => lang)).subscribe(() => {
      this._lang.set(lang);
      this.applyToDocument(lang);
      try {
        localStorage.setItem(STORAGE_KEY, lang);
      } catch {
        /* storage unavailable (private mode) */
      }
    });
  }

  toggle(): void {
    this.setLang(this._lang() === 'ar' ? 'en' : 'ar');
  }

  /** Translates a key from code; reading `lang()` makes computeds re-run when the language changes. */
  instant(key: string, params?: Record<string, unknown>): string {
    this._lang();
    return this.translate.instant(key, params);
  }

  /** Picks the current language from a bilingual value. */
  pick(value: Loc): string {
    return value[this._lang()];
  }

  /** Translates an API error code (e.g. `auth.invalidCredentials`). */
  error(code: string): string {
    const key = `errors.${code}`;
    const text = this.instant(key);
    return text === key ? this.instant('errors.unknown') : text;
  }

  private applyToDocument(lang: Lang): void {
    const html = this.document.documentElement;
    html.lang = lang;
    html.dir = lang === 'ar' ? 'rtl' : 'ltr';
    this.document.title = lang === 'ar' ? 'RedKey | ريد كي' : 'RedKey';
  }
}

function readStoredLang(): Lang {
  try {
    return localStorage.getItem(STORAGE_KEY) === 'en' ? 'en' : 'ar';
  } catch {
    return 'ar';
  }
}
