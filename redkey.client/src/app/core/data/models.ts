import type { IconName } from '../../shared/components/icon/icons';
import { Loc } from '../i18n/i18n.models';

export type Purpose = 'sale' | 'rent';
export type PropertyType = 'apartment' | 'villa' | 'townhouse' | 'chalet' | 'duplex' | 'office' | 'studio';
export type AreaId = 'new-cairo' | 'sheikh-zayed' | 'north-coast' | 'new-capital';

export interface PaymentPlan {
  label: Loc;
  value: Loc;
}

export interface Property {
  id: number;
  code: string;
  title: Loc;
  compound: Loc;
  city: Loc;
  areaId: AreaId;
  purpose: Purpose;
  type: PropertyType;
  /** EGP. Monthly when purpose = rent. */
  price: number;
  beds: number;
  baths: number;
  size: number;
  finishing: Loc;
  delivery: Loc;
  images: readonly string[];
  description: Loc;
  paymentPlans: readonly PaymentPlan[];
  features: readonly Loc[];
  featured: boolean;
  /** Broker commission %. */
  commission: number;
}

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

export interface AreaInfo {
  id: AreaId;
  name: Loc;
  image: string;
}

export interface AppNotification {
  id: number;
  icon: IconName;
  title: Loc;
  body: Loc;
  time: Loc;
  read: boolean;
}

export interface Testimonial {
  name: Loc;
  role: Loc;
  quote: Loc;
}
