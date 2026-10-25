export type Lang = 'ar' | 'en';

/** A text that exists in both languages (used by the static front-end data). */
export interface Loc {
  ar: string;
  en: string;
}
