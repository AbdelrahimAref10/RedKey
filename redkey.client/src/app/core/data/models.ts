import { Loc } from '../i18n/i18n.models';

export interface Opportunity {
  id: number;
  title: Loc;
  fullTitle: Loc;
  area: Loc;
  subtitle: Loc;
  image: string;
  /** Expected annual yield %. */
  yield: number;
  sharePrice: number;
  durationYears: number;
  description: Loc;
  interested: number;
}

export interface Testimonial {
  name: Loc;
  role: Loc;
  quote: Loc;
}
