import { BrokerFlowStatus } from '../../core/data/status';
import { Loc } from '../../core/i18n/i18n.models';

/** Demo data until the admin endpoints exist. */

export type RequestType = 'visit' | 'invest' | 'contact';
export type RequestStatus = 0 | 1 | 2; // new → contacted → closed
export type UnitStatus = 'published' | 'hidden' | 'sold';

export interface RequestRow {
  id: number;
  name: Loc;
  phone: string;
  type: RequestType;
  details: Loc;
  time: Loc;
  status: RequestStatus;
}

const L = (ar: string, en: string): Loc => ({ ar, en });

export const REQUESTS: readonly RequestRow[] = [
  { id: 1, name: L('محمد عبد الله', 'Mohamed Abdallah'), phone: '010 1122 3344', type: 'visit', details: L('RK-1042 · تاج سيتي · بعد الضهر', 'RK-1042 · Taj City · Afternoon'), time: L('من 5 دقايق', '5 min ago'), status: 0 },
  { id: 2, name: L('ليلى حسام', 'Laila Hossam'), phone: '011 2233 4455', type: 'invest', details: L('برج تايم · 20 – 100 ألف', 'Time Tower · 20K – 100K'), time: L('من 20 دقيقة', '20 min ago'), status: 0 },
  { id: 3, name: L('نهى سمير', 'Noha Samir'), phone: '012 3344 5566', type: 'visit', details: L('RK-1108 · ميفيدا · الصبح', 'RK-1108 · Mivida · Morning'), time: L('من ساعة', '1 hour ago'), status: 1 },
  { id: 4, name: L('عمر طارق', 'Omar Tarek'), phone: '010 4455 6677', type: 'contact', details: L('عايز أعرض وحدتي في زايد', 'Wants to list a unit in Zayed'), time: L('النهارده', 'Today'), status: 1 },
  { id: 5, name: L('هشام كمال', 'Hesham Kamal'), phone: '011 5566 7788', type: 'invest', details: L('شاليهات الساحل · أكتر من 100 ألف', 'Coast chalets · 100K+'), time: L('امبارح', 'Yesterday'), status: 1 },
  { id: 6, name: L('منى حسن', 'Mona Hassan'), phone: '012 6677 8899', type: 'visit', details: L('RK-1003 · ذا سكوير · بالليل', 'RK-1003 · The Square · Evening'), time: L('امبارح', 'Yesterday'), status: 2 }
];

export const UNIT_STATUS: Readonly<Record<number, UnitStatus>> = { 1003: 'sold', 988: 'hidden' };
export const UNIT_REQUESTS: Readonly<Record<number, number>> = {
  1042: 14, 1108: 6, 1003: 9, 997: 11, 1120: 7, 1140: 4, 1152: 3, 1131: 5, 988: 2, 1160: 1, 1171: 3, 1180: 6
};

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

export interface FavoriteByUser {
  name: Loc;
  phone: string;
  unit: Loc;
  price: Loc;
  when: Loc;
  after: Loc;
}

export interface FavoriteByUnit {
  unit: Loc;
  price: Loc;
  count: number;
  requested: number;
  last: Loc;
}

const NO_VISIT = L('لسه ما طلبش معاينة', 'No viewing request yet');

export const FAVORITES_BY_USER: readonly FavoriteByUser[] = [
  { name: L('أحمد علي', 'Ahmed Ali'), phone: '010 1234 5678', unit: L('شقة 3 غرف · تاج سيتي', '3-bed · Taj City'), price: L('8,450,000 ج', 'EGP 8,450,000'), when: L('من ساعة', '1 hour ago'), after: NO_VISIT },
  { name: L('أحمد علي', 'Ahmed Ali'), phone: '010 1234 5678', unit: L('تاون هاوس كورنر · ميفيدا', 'Corner townhouse · Mivida'), price: L('21,900,000 ج', 'EGP 21,900,000'), when: L('امبارح', 'Yesterday'), after: L('طلب معاينة', 'Requested a viewing') },
  { name: L('سلمى رأفت', 'Salma Raafat'), phone: '011 4455 6677', unit: L('شاليه أول صف · مراسي', 'Sea chalet · Marassi'), price: L('14,200,000 ج', 'EGP 14,200,000'), when: L('امبارح', 'Yesterday'), after: NO_VISIT },
  { name: L('خالد منير', 'Khaled Mounir'), phone: '012 8899 0011', unit: L('شقة 3 غرف · تاج سيتي', '3-bed · Taj City'), price: L('8,450,000 ج', 'EGP 8,450,000'), when: L('من يومين', '2 days ago'), after: L('كلّمنا على واتساب', 'Contacted on WhatsApp') },
  { name: L('ريم عادل', 'Reem Adel'), phone: '010 2233 4455', unit: L('دوبلكس بجاردن · زيد ويست', 'Garden duplex · Zed West'), price: L('16,750,000 ج', 'EGP 16,750,000'), when: L('من 3 أيام', '3 days ago'), after: NO_VISIT },
  { name: L('ريم عادل', 'Reem Adel'), phone: '010 2233 4455', unit: L('شقة مفروشة · ذا سكوير', 'Furnished · The Square'), price: L('45,000 / شهر', 'EGP 45,000 / month'), when: L('من 3 أيام', '3 days ago'), after: NO_VISIT }
];

export const FAVORITES_BY_UNIT: readonly FavoriteByUnit[] = [
  { unit: L('شقة 3 غرف · تاج سيتي', '3-bed · Taj City'), price: L('8,450,000 ج', 'EGP 8,450,000'), count: 46, requested: 12, last: L('من ساعة', '1 hour ago') },
  { unit: L('تاون هاوس كورنر · ميفيدا', 'Corner townhouse · Mivida'), price: L('21,900,000 ج', 'EGP 21,900,000'), count: 31, requested: 9, last: L('امبارح', 'Yesterday') },
  { unit: L('شاليه أول صف · مراسي', 'Sea chalet · Marassi'), price: L('14,200,000 ج', 'EGP 14,200,000'), count: 27, requested: 4, last: L('امبارح', 'Yesterday') },
  { unit: L('دوبلكس بجاردن · زيد ويست', 'Garden duplex · Zed West'), price: L('16,750,000 ج', 'EGP 16,750,000'), count: 18, requested: 3, last: L('من 3 أيام', '3 days ago') },
  { unit: L('شقة مفروشة · ذا سكوير', 'Furnished · The Square'), price: L('45,000 / شهر', 'EGP 45,000 / month'), count: 11, requested: 2, last: L('من 3 أيام', '3 days ago') }
];
