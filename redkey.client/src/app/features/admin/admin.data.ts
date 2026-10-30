import { BrokerFlowStatus } from '../../core/data/status';
import { Loc } from '../../core/i18n/i18n.models';

/** Demo data until the admin endpoints exist. */

const L = (ar: string, en: string): Loc => ({ ar, en });

export interface FlowRow {
  id: string;
  status: BrokerFlowStatus;
  /** Status the action button moves the row to (none = no action). */
  next?: BrokerFlowStatus;
  action?: 'approve' | 'reject' | 'markPaid' | 'publish';
}

export interface JoinRow extends FlowRow {
  name: Loc;
  phone: string;
  type: Loc;
  idCard: Loc;
  requestedAt: Loc;
}

export interface BrokerRow {
  name: Loc;
  code: string;
  type: Loc;
  clients: number;
  units: number;
  sales: number;
  status: BrokerFlowStatus;
}

export interface BrokerClientAdminRow extends FlowRow {
  name: Loc;
  phone: string;
  broker: Loc;
  unit: Loc;
  until: Loc;
  commission: Loc;
}

export interface BrokerUnitAdminRow extends FlowRow {
  title: Loc;
  broker: Loc;
  price: Loc;
  documents: Loc;
}

const INDEPENDENT = L('مستقل', 'Independent');
const KARIM = L('كريم وجيه', 'Karim Wageh');
const DASH = L('—', '—');

export const JOIN_REQUESTS: readonly JoinRow[] = [
  { id: 'j1', name: L('أحمد سامي', 'Ahmed Samy'), phone: '011 2345 6789', type: INDEPENDENT, idCard: L('✓', '✓'), requestedAt: L('النهارده', 'Today'), status: 'wait', next: 'ok', action: 'approve' },
  { id: 'j2', name: L('دينا فاروق', 'Dina Farouk'), phone: '012 5555 0101', type: L('تبع شركة · النخبة', 'Company · El Nokhba'), idCard: L('✓', '✓'), requestedAt: L('امبارح', 'Yesterday'), status: 'wait', next: 'ok', action: 'approve' },
  { id: 'j3', name: L('سامح جلال', 'Sameh Galal'), phone: '010 7777 1212', type: INDEPENDENT, idCard: L('ناقصة', 'Missing'), requestedAt: L('امبارح', 'Yesterday'), status: 'wait', next: 'no', action: 'reject' }
];

export const BROKERS: readonly BrokerRow[] = [
  { name: KARIM, code: 'BR-2041', type: INDEPENDENT, clients: 5, units: 2, sales: 1, status: 'ok' },
  { name: L('طارق نبيل', 'Tarek Nabil'), code: 'BR-2077', type: L('تبع شركة · النخبة', 'Company · El Nokhba'), clients: 8, units: 0, sales: 2, status: 'ok' },
  { name: L('رنا مصطفى', 'Rana Mostafa'), code: 'BR-2090', type: INDEPENDENT, clients: 2, units: 1, sales: 0, status: 'ok' },
  { name: L('وائل شريف', 'Wael Sherif'), code: 'BR-1980', type: INDEPENDENT, clients: 0, units: 0, sales: 0, status: 'off' }
];

export const BROKER_CLIENTS_ADMIN: readonly BrokerClientAdminRow[] = [
  { id: 'c1', name: L('محمد عبد الله', 'Mohamed Abdallah'), phone: '010 1122 3344', broker: KARIM, unit: L('تاج سيتي', 'Taj City'), until: L('22 ديسمبر', 'Dec 22'), commission: DASH, status: 'reg' },
  { id: 'c2', name: L('نهى سمير', 'Noha Samir'), phone: '012 3344 5566', broker: KARIM, unit: L('ميفيدا', 'Mivida'), until: L('24 نوفمبر', 'Nov 24'), commission: DASH, status: 'visit' },
  { id: 'c3', name: L('يوسف رامي', 'Youssef Ramy'), phone: '011 9988 7766', broker: L('طارق نبيل', 'Tarek Nabil'), unit: L('تاج سيتي', 'Taj City'), until: DASH, commission: L('2.5% · 211,250 ج', '2.5% · EGP 211,250'), status: 'sold', next: 'paid', action: 'markPaid' },
  { id: 'c4', name: L('منى حسن', 'Mona Hassan'), phone: '012 6677 8899', broker: KARIM, unit: L('مدينتي B12', 'Madinaty B12'), until: DASH, commission: L('2% · 162,500 ج', '2% · EGP 162,500'), status: 'sold', next: 'paid', action: 'markPaid' }
];

export const BROKER_UNITS_ADMIN: readonly BrokerUnitAdminRow[] = [
  { id: 'u1', title: L('شقة 2 غرف · أكتوبر', '2-bed apartment · October'), broker: KARIM, price: L('3,200,000 ج', 'EGP 3,200,000'), documents: L('صور ✓ · عقد ✓ · بطاقة المالك ✓', 'Photos ✓ · Contract ✓ · Owner ID ✓'), status: 'rev', next: 'pub', action: 'publish' },
  { id: 'u2', title: L('دوبلكس · مدينتي B12', 'Duplex · Madinaty B12'), broker: KARIM, price: L('8,400,000 ج', 'EGP 8,400,000'), documents: L('كاملة', 'Complete'), status: 'pub' },
  { id: 'u3', title: L('شقة 3 غرف · الرحاب', '3-bed apartment · Rehab'), broker: L('رنا مصطفى', 'Rana Mostafa'), price: L('5,100,000 ج', 'EGP 5,100,000'), documents: L('كاملة', 'Complete'), status: 'pub' }
];
