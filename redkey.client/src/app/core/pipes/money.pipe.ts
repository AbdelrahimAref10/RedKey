import { Pipe, PipeTransform } from '@angular/core';
import { Lang } from '../i18n/i18n.models';

const numberFormat = new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 });

/**
 * `{{ 8450000 | money: lang() }}` → "8,450,000 ج.م" / "EGP 8,450,000".
 * `{{ 45000 | money: lang() : true }}` adds the monthly suffix (rent).
 */
@Pipe({ name: 'money' })
export class MoneyPipe implements PipeTransform {
  transform(value: number | null | undefined, lang: Lang, monthly = false): string {
    if (value == null) return '';
    const amount = numberFormat.format(value);
    if (lang === 'ar') return `${amount} ج.م${monthly ? ' / شهر' : ''}`;
    return `EGP ${amount}${monthly ? ' / month' : ''}`;
  }
}
