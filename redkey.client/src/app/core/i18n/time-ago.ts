import { Lang } from './i18n.models';

const formatters = new Map<Lang, Intl.RelativeTimeFormat>();

/** "5 minutes ago" / "منذ 5 دقائق" from an UTC date coming from the API. */
export function timeAgo(utc: string, lang: Lang): string {
  let fmt = formatters.get(lang);
  if (!fmt) {
    fmt = new Intl.RelativeTimeFormat(lang === 'ar' ? 'ar-EG' : 'en', { numeric: 'auto' });
    formatters.set(lang, fmt);
  }

  // The API sends UTC without "Z".
  const date = Date.parse(utc.endsWith('Z') ? utc : `${utc}Z`);
  const seconds = Math.round((date - Date.now()) / 1000);
  const abs = Math.abs(seconds);

  if (abs < 60) return fmt.format(seconds, 'second');
  if (abs < 3600) return fmt.format(Math.round(seconds / 60), 'minute');
  if (abs < 86400) return fmt.format(Math.round(seconds / 3600), 'hour');
  return fmt.format(Math.round(seconds / 86400), 'day');
}
