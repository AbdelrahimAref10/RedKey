import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Lang, Loc } from './i18n.models';
import { ar } from './translations/ar';
import { en } from './translations/en';

const STORAGE_KEY = 'rk.lang';

/**
 * Signal based translations: templates read `t().section.key`, which is type checked
 * and re-renders OnPush views automatically when the language changes.
 */
@Injectable({ providedIn: 'root' })
export class I18nService {
  private readonly document = inject(DOCUMENT);

  private readonly _lang = signal<Lang>(readStoredLang());

  readonly lang = this._lang.asReadonly();
  readonly t = computed(() => (this._lang() === 'ar' ? ar : en));
  readonly dir = computed(() => (this._lang() === 'ar' ? 'rtl' : 'ltr'));
  readonly isRtl = computed(() => this._lang() === 'ar');

  constructor() {
    this.applyToDocument(this._lang());
  }

  setLang(lang: Lang): void {
    if (lang === this._lang()) return;
    this._lang.set(lang);
    this.applyToDocument(lang);
    try {
      localStorage.setItem(STORAGE_KEY, lang);
    } catch {
      /* storage unavailable (private mode) */
    }
  }

  toggle(): void {
    this.setLang(this._lang() === 'ar' ? 'en' : 'ar');
  }

  /** Picks the current language from a bilingual value. */
  pick(value: Loc): string {
    return value[this._lang()];
  }

  /** Translates an API error code (e.g. `auth.invalidCredentials`). */
  error(code: string): string {
    const errors = this.t().errors as Record<string, string>;
    return errors[code] ?? errors['unknown'];
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
