import { BrokerFlowStatus } from '../../core/data/status';
import { Loc } from '../../core/i18n/i18n.models';

export interface BrokerClientRow {
  id: number;
  name: Loc;
  phone: string;
  unit: Loc;
  reservation: Loc;
  status: BrokerFlowStatus;
  commission: Loc;
}

export interface BrokerListingRow {
  id: number;
  title: Loc;
  price: number;
  uploadedAt: Loc;
  documents: Loc;
  status: BrokerFlowStatus;
}

const NONE: Loc = { ar: '—', en: '—' };

/** Demo data until the broker endpoints exist. */
export const BROKER_CODE = 'BR-2041';

export const BROKER_CLIENTS: readonly BrokerClientRow[] = [
  {
    id: 1, phone: '010 1122 3344', status: 'reg', commission: NONE,
    name: { ar: 'محمد عبد الله', en: 'Mohamed Abdallah' },
    unit: { ar: 'شقة 3 غرف · تاج سيتي', en: '3-bed apartment · Taj City' },
    reservation: { ar: 'باقي 52 يوم', en: '52 days left' }
  },
  {
    id: 2, phone: '012 3344 5566', status: 'visit', commission: NONE,
    name: { ar: 'نهى سمير', en: 'Noha Samir' },
    unit: { ar: 'شقة بجاردن · ميفيدا', en: 'Garden apartment · Mivida' },
    reservation: { ar: 'باقي 31 يوم', en: '31 days left' }
  },
  {
    id: 3, phone: '012 6677 8899', status: 'sold',
    name: { ar: 'منى حسن', en: 'Mona Hassan' },
    unit: { ar: 'دوبلكس · مدينتي B12', en: 'Duplex · Madinaty B12' },
    reservation: { ar: 'اتباع', en: 'Sold' },
    commission: { ar: '2% · 162,500 ج.م', en: '2% · EGP 162,500' }
  },
  {
    id: 4, phone: '011 9988 7766', status: 'visit', commission: NONE,
    name: { ar: 'يوسف رامي', en: 'Youssef Ramy' },
    unit: { ar: 'تاون هاوس · ميفيدا', en: 'Townhouse · Mivida' },
    reservation: { ar: 'باقي 12 يوم', en: '12 days left' }
  },
  {
    id: 5, phone: '010 5544 3322', status: 'reg', commission: NONE,
    name: { ar: 'سارة فتحي', en: 'Sara Fathy' },
    unit: { ar: 'شاليه · مراسي', en: 'Chalet · Marassi' },
    reservation: { ar: 'باقي 58 يوم', en: '58 days left' }
  }
];

export const BROKER_LISTINGS: readonly BrokerListingRow[] = [
  {
    id: 1, price: 8_400_000, status: 'pub',
    title: { ar: 'دوبلكس · مدينتي B12', en: 'Duplex · Madinaty B12' },
    uploadedAt: { ar: '20 أكتوبر', en: 'Oct 20' },
    documents: { ar: 'كاملة', en: 'Complete' }
  },
  {
    id: 2, price: 3_200_000, status: 'rev',
    title: { ar: 'شقة 2 غرف · أكتوبر', en: '2-bed apartment · October' },
    uploadedAt: { ar: 'امبارح', en: 'Yesterday' },
    documents: { ar: 'صور ✓ · عقد ✓ · بطاقة المالك ✓', en: 'Photos ✓ · Contract ✓ · Owner ID ✓' }
  }
];
