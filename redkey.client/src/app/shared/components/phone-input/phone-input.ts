import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  computed,
  forwardRef,
  inject,
  input,
  signal,
  viewChild
} from '@angular/core';
import { AbstractControl, ControlValueAccessor, NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors, Validator } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { COUNTRIES, Country, DEFAULT_COUNTRY, findCountry, flagUrl } from '../../../core/data/countries';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../icon/icon';

/** The value of a phone field: the picked country + the national number (digits only, no leading 0). */
export interface PhoneValue {
  countryIso: string;
  dialCode: string;
  number: string;
}

interface CountryOption extends Country {
  name: string;
  /** Lower-case name in both languages + code, for the search box. */
  search: string;
}

const MIN_DIGITS = 6;
const MAX_DIGITS = 14;

/** Digits only, without the trunk prefix 0 (01012345678 → 1012345678). */
export function nationalDigits(value: string): string {
  return value.replace(/\D/g, '').replace(/^0+/, '');
}

/**
 * Country picker (flag + calling code, searchable) + the number. Works with reactive forms
 * (value: {@link PhoneValue}) and validates itself (required + length).
 */
@Component({
  selector: 'app-phone-input',
  imports: [TranslatePipe, Icon],
  templateUrl: './phone-input.html',
  styleUrl: './phone-input.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => PhoneInput), multi: true },
    { provide: NG_VALIDATORS, useExisting: forwardRef(() => PhoneInput), multi: true }
  ],
  host: {
    '(document:click)': 'onDocumentClick($event)',
    '[class.invalid]': 'invalid()',
    '[class.open]': 'open()'
  }
})
export class PhoneInput implements ControlValueAccessor, Validator {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  protected readonly i18n = inject(I18nService);

  /** Red border (the form decides when: usually invalid + touched). */
  readonly invalid = input(false);
  readonly inputId = input<string>();

  private readonly searchBox = viewChild<ElementRef<HTMLInputElement>>('searchBox');
  private readonly list = viewChild<ElementRef<HTMLElement>>('list');

  protected readonly iso = signal(DEFAULT_COUNTRY);
  protected readonly number = signal('');
  protected readonly open = signal(false);
  protected readonly query = signal('');
  protected readonly active = signal(0);
  protected readonly disabled = signal(false);
  protected readonly flagUrl = flagUrl;

  /** Names in the UI language (Intl), sorted; the search also matches the other language and the code. */
  private readonly options = computed<CountryOption[]>(() => {
    const lang = this.i18n.lang();
    const local = new Intl.DisplayNames([lang], { type: 'region' });
    const other = new Intl.DisplayNames([lang === 'ar' ? 'en' : 'ar'], { type: 'region' });
    const collator = new Intl.Collator(lang);
    return COUNTRIES.map(c => {
      const name = local.of(c.iso) ?? c.iso;
      return { ...c, name, search: `${name} ${other.of(c.iso) ?? ''} ${c.iso} +${c.dial}`.toLowerCase() };
    }).sort((a, b) => collator.compare(a.name, b.name));
  });

  protected readonly filtered = computed(() => {
    const q = this.query().trim().toLowerCase().replace(/^\+/, '');
    const all = this.options();
    if (!q) return all;
    return all.filter(c => c.search.includes(q) || c.dial.startsWith(q));
  });

  protected readonly selected = computed(() => this.options().find(c => c.iso === this.iso()) ?? this.options()[0]);

  private onChange: (value: PhoneValue | null) => void = () => undefined;
  private onTouched: () => void = () => undefined;

  // ---- ControlValueAccessor ----
  writeValue(value: PhoneValue | null): void {
    this.iso.set(findCountry(value?.countryIso)?.iso ?? DEFAULT_COUNTRY);
    this.number.set(value?.number ?? '');
  }

  registerOnChange(fn: (value: PhoneValue | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled: boolean): void {
    this.disabled.set(disabled);
  }

  // ---- Validator ----
  validate(control: AbstractControl<PhoneValue | null>): ValidationErrors | null {
    const digits = nationalDigits(control.value?.number ?? '');
    if (!digits) return { required: true };
    return digits.length < MIN_DIGITS || digits.length > MAX_DIGITS ? { phone: true } : null;
  }

  // ---- UI ----
  protected onNumber(event: Event): void {
    const input = event.target as HTMLInputElement;
    // Digits and spaces only while typing; the leading 0 is dropped when saving.
    const clean = input.value.replace(/[^\d ]/g, '').slice(0, 20);
    if (clean !== input.value) input.value = clean;
    this.number.set(clean);
    this.emit();
  }

  protected toggle(): void {
    if (this.disabled()) return;
    if (this.open()) {
      this.close();
      return;
    }
    this.query.set('');
    this.active.set(Math.max(0, this.filtered().findIndex(c => c.iso === this.iso())));
    this.open.set(true);
    setTimeout(() => {
      this.searchBox()?.nativeElement.focus();
      this.scrollActiveIntoView();
    });
  }

  protected pick(country: CountryOption): void {
    this.iso.set(country.iso);
    this.close();
    this.emit();
    (this.host.nativeElement.querySelector('input[type=tel]') as HTMLInputElement | null)?.focus();
  }

  protected onSearch(value: string): void {
    this.query.set(value);
    this.active.set(0);
  }

  protected onSearchKey(event: KeyboardEvent): void {
    const count = this.filtered().length;
    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.active.update(i => Math.min(count - 1, i + 1));
        this.scrollActiveIntoView();
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.active.update(i => Math.max(0, i - 1));
        this.scrollActiveIntoView();
        break;
      case 'Enter': {
        event.preventDefault();
        const country = this.filtered()[this.active()];
        if (country) this.pick(country);
        break;
      }
      case 'Escape':
        event.preventDefault();
        this.close();
        break;
    }
  }

  protected onBlur(): void {
    this.onTouched();
  }

  protected onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) this.close();
  }

  private close(): void {
    this.open.set(false);
    this.onTouched();
  }

  private emit(): void {
    const country = findCountry(this.iso())!;
    this.onChange({ countryIso: country.iso, dialCode: country.dial, number: nationalDigits(this.number()) });
  }

  private scrollActiveIntoView(): void {
    setTimeout(() => this.list()?.nativeElement.querySelector('.active')?.scrollIntoView({ block: 'nearest' }));
  }
}
