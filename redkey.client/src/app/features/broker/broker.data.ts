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

