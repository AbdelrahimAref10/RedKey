import { IMAGES } from '../../core/data/images';
import { Loc } from '../../core/i18n/i18n.models';

/** What the card shows about the unit (demo data until the customer endpoints exist). */
export interface MyUnitInfo {
  unitId: string;
  code: string;
  image: string;
  type: Loc;
  title: Loc;
  compound: Loc;
  city: Loc;
  delivery: Loc;
}

/** Demo data for the signed-in customer until the customer endpoints exist. */

export interface OwnedUnit {
  kind: 'owned';
  info: MyUnitInfo;
  paid: number;
  total: number;
  installmentsPaid: number;
  installmentsTotal: number;
  nextPayment: Loc;
  nextAmount: number;
}

export interface RentedUnit {
  kind: 'rented';
  info: MyUnitInfo;
  monthlyRent: number;
  contractEnds: Loc;
  nextPayment: Loc;
}

export type MyUnit = OwnedUnit | RentedUnit;

export const MY_UNITS: readonly MyUnit[] = [
  {
    kind: 'owned',
    info: {
      unitId: 'demo-owned', code: 'RK-1042', image: IMAGES.properties.apartmentLandscape,
      type: { ar: 'شقة', en: 'Apartment' },
      title: { ar: 'شقة 3 غرف بفيو لاندسكيب', en: '3-bedroom apartment with landscape view' },
      compound: { ar: 'تاج سيتي', en: 'Taj City' }, city: { ar: 'مدينة نصر', en: 'Nasr City' },
      delivery: { ar: 'استلام 2027', en: 'Delivery 2027' }
    },
    paid: 1_690_000, total: 8_450_000, installmentsPaid: 12, installmentsTotal: 84,
    nextPayment: { ar: '1 ديسمبر', en: 'Dec 1' }, nextAmount: 95_565
  },
  {
    kind: 'rented',
    info: {
      unitId: 'demo-rented', code: 'RK-1003', image: IMAGES.properties.apartmentFurnished,
      type: { ar: 'شقة', en: 'Apartment' },
      title: { ar: 'شقة مفروشة', en: 'Furnished apartment' },
      compound: { ar: 'ذا سكوير', en: 'The Square' }, city: { ar: 'التجمع الخامس', en: 'Fifth Settlement' },
      delivery: { ar: 'استلام فوري', en: 'Ready to move' }
    },
    monthlyRent: 45_000,
    contractEnds: { ar: '30 سبتمبر 2027', en: 'Sep 30, 2027' },
    nextPayment: { ar: '1 يناير', en: 'Jan 1' }
  }
];

export interface MyInvestment {
  opportunityId: number;
  shares: number;
  invested: number;
  received: number;
  since: Loc;
  nextDistribution: Loc;
  status: 'active' | 'pending';
}

export const MY_INVESTMENTS: readonly MyInvestment[] = [
  {
    opportunityId: 1, shares: 4, invested: 20_000, received: 1_350, status: 'active',
    since: { ar: 'مارس 2026', en: 'Mar 2026' }, nextDistribution: { ar: '1 نوفمبر', en: 'Nov 1' }
  },
  {
    opportunityId: 3, shares: 10, invested: 20_000, received: 2_400, status: 'active',
    since: { ar: 'يونيو 2025', en: 'Jun 2025' }, nextDistribution: { ar: 'آخر الموسم', en: 'End of season' }
  },
  {
    opportunityId: 2, shares: 2, invested: 5_000, received: 0, status: 'pending',
    since: { ar: 'أكتوبر 2026', en: 'Oct 2026' }, nextDistribution: { ar: '—', en: '—' }
  }
];
