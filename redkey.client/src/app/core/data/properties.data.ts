import { Loc } from '../i18n/i18n.models';
import { IMAGES } from './images';
import { PaymentPlan, Property } from './models';

const P = IMAGES.properties;
const fmt = new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 });

/** Installment plans for a sale unit (down payment %, years) + cash discount. */
function salePlans(price: number): PaymentPlan[] {
  const plan = (down: number, years: number): PaymentPlan => {
    const monthly = fmt.format(Math.round((price * (1 - down / 100)) / (years * 12)));
    return {
      label: { ar: `مقدم ${down}% على ${years} سنين`, en: `${down}% down over ${years} years` },
      value: { ar: `قسط ${monthly} ج.م / شهر`, en: `EGP ${monthly} / month` }
    };
  };
  return [
    plan(5, 7),
    plan(10, 8),
    { label: { ar: 'كاش', en: 'Cash' }, value: { ar: 'خصم 15%', en: '15% discount' } }
  ];
}

const RENT_PLANS: PaymentPlan[] = [
  { label: { ar: 'التأمين', en: 'Security deposit' }, value: { ar: 'شهرين', en: '2 months' } },
  { label: { ar: 'الدفع', en: 'Payment' }, value: { ar: 'كل 3 شهور', en: 'Every 3 months' } },
  { label: { ar: 'أقل مدة', en: 'Minimum term' }, value: { ar: 'سنة', en: '1 year' } }
];

const F = {
  security: { ar: 'أمن وحراسة 24 ساعة', en: '24/7 security' },
  parking: { ar: 'جراج خاص', en: 'Private parking' },
  clubhouse: { ar: 'كلوب هاوس', en: 'Clubhouse' },
  pool: { ar: 'حمام سباحة', en: 'Swimming pool' },
  green: { ar: 'مساحات خضراء', en: 'Green areas' },
  schools: { ar: 'قريب من المدارس', en: 'Near schools' },
  sea: { ar: 'فيو مباشر على البحر', en: 'Direct sea view' },
  furnished: { ar: 'مفروشة بالكامل', en: 'Fully furnished' },
  elevator: { ar: 'أسانسير', en: 'Elevator' },
  gym: { ar: 'جيم', en: 'Gym' }
} satisfies Record<string, Loc>;

export const PROPERTIES: readonly Property[] = [
  {
    id: 1042, code: 'RK-1042', purpose: 'sale', type: 'apartment', areaId: 'new-cairo',
    title: { ar: 'شقة 3 غرف بفيو لاندسكيب', en: '3-bedroom apartment with landscape view' },
    compound: { ar: 'تاج سيتي', en: 'Taj City' },
    city: { ar: 'مدينة نصر', en: 'Nasr City' },
    price: 8_450_000, beds: 3, baths: 2, size: 165,
    finishing: { ar: 'نصف تشطيب', en: 'Semi-finished' },
    delivery: { ar: 'استلام 2027', en: 'Delivery 2027' },
    images: [P.apartmentLandscape, P.apartmentGarden, P.apartmentFurnished],
    description: {
      ar: 'شقة بحري على لاندسكيب مفتوح، قريبة من الكلوب هاوس والمدارس. الريسبشن 3 قطع ومطبخ مفتوح وبلكونة 14 متر.',
      en: 'North-facing apartment on an open landscape, close to the clubhouse and schools. 3-piece reception, open kitchen and a 14 m² balcony.'
    },
    paymentPlans: salePlans(8_450_000),
    features: [F.security, F.clubhouse, F.green, F.schools, F.parking],
    featured: true, commission: 2.5
  },
  {
    id: 1108, code: 'RK-1108', purpose: 'sale', type: 'townhouse', areaId: 'new-cairo',
    title: { ar: 'تاون هاوس كورنر', en: 'Corner townhouse' },
    compound: { ar: 'ميفيدا', en: 'Mivida' },
    city: { ar: 'التجمع الخامس', en: 'Fifth Settlement' },
    price: 21_900_000, beds: 4, baths: 4, size: 285,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام 2026', en: 'Delivery 2026' },
    images: [P.townhouseCorner, P.apartmentGarden, P.apartmentLandscape],
    description: {
      ar: 'تاون هاوس كورنر بجاردن خاصة وروف، في قلب ميفيدا وقريب من البوليفارد والخدمات.',
      en: 'Corner townhouse with a private garden and roof, in the heart of Mivida near the boulevard and services.'
    },
    paymentPlans: salePlans(21_900_000),
    features: [F.security, F.pool, F.clubhouse, F.green, F.parking, F.gym],
    featured: true, commission: 3
  },
  {
    id: 1003, code: 'RK-1003', purpose: 'rent', type: 'apartment', areaId: 'new-cairo',
    title: { ar: 'شقة مفروشة', en: 'Furnished apartment' },
    compound: { ar: 'ذا سكوير', en: 'The Square' },
    city: { ar: 'التجمع الخامس', en: 'Fifth Settlement' },
    price: 45_000, beds: 2, baths: 2, size: 130,
    finishing: { ar: 'مفروشة', en: 'Furnished' },
    delivery: { ar: 'استلام فوري', en: 'Ready to move' },
    images: [P.apartmentFurnished, P.apartmentLandscape, P.apartmentGarden],
    description: {
      ar: 'شقة مفروشة بالكامل بأجهزة حديثة، جاهزة للسكن فورًا وقريبة من التسعين الشمالي.',
      en: 'Fully furnished apartment with modern appliances, ready to move in and close to North 90th Street.'
    },
    paymentPlans: RENT_PLANS,
    features: [F.furnished, F.security, F.elevator, F.parking],
    featured: true, commission: 2
  },
  {
    id: 997, code: 'RK-0997', purpose: 'sale', type: 'chalet', areaId: 'north-coast',
    title: { ar: 'شاليه أول صف على البحر', en: 'Sea-front chalet' },
    compound: { ar: 'مراسي', en: 'Marassi' },
    city: { ar: 'الساحل الشمالي', en: 'North Coast' },
    price: 14_200_000, beds: 3, baths: 3, size: 160,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام 2026', en: 'Delivery 2026' },
    images: [P.chaletSea, P.apartmentGarden, P.townhouseCorner],
    description: {
      ar: 'شاليه أول صف على البحر بتراس واسع، في أهدى مرحلة في مراسي.',
      en: 'First-row sea chalet with a wide terrace, in the calmest phase of Marassi.'
    },
    paymentPlans: salePlans(14_200_000),
    features: [F.sea, F.pool, F.security, F.parking],
    featured: true, commission: 2.5
  },
  {
    id: 1120, code: 'RK-1120', purpose: 'sale', type: 'apartment', areaId: 'new-cairo',
    title: { ar: 'شقة بجاردن', en: 'Garden apartment' },
    compound: { ar: 'ميفيدا', en: 'Mivida' },
    city: { ar: 'التجمع الخامس', en: 'Fifth Settlement' },
    price: 9_200_000, beds: 3, baths: 2, size: 170,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام 2026', en: 'Delivery 2026' },
    images: [P.apartmentGarden, P.apartmentLandscape, P.townhouseCorner],
    description: {
      ar: 'شقة أرضي بجاردن 80 متر، على لاندسكيب وقريبة من الكلوب هاوس.',
      en: 'Ground-floor apartment with an 80 m² garden, on the landscape near the clubhouse.'
    },
    paymentPlans: salePlans(9_200_000),
    features: [F.green, F.security, F.clubhouse, F.parking],
    featured: true, commission: 3
  },
  {
    id: 1140, code: 'RK-1140', purpose: 'rent', type: 'villa', areaId: 'sheikh-zayed',
    title: { ar: 'فيلا بحمام سباحة', en: 'Villa with a private pool' },
    compound: { ar: 'الشيخ زايد', en: 'Sheikh Zayed' },
    city: { ar: 'الجيزة', en: 'Giza' },
    price: 55_000, beds: 5, baths: 5, size: 320,
    finishing: { ar: 'مفروشة', en: 'Furnished' },
    delivery: { ar: 'استلام فوري', en: 'Ready to move' },
    images: [IMAGES.hero, P.townhouseCorner, P.apartmentGarden],
    description: {
      ar: 'فيلا مستقلة بحمام سباحة خاص وجاردن كبيرة، مناسبة للعائلات.',
      en: 'Standalone villa with a private pool and a large garden, perfect for families.'
    },
    paymentPlans: RENT_PLANS,
    features: [F.pool, F.furnished, F.green, F.security, F.parking],
    featured: true, commission: 2
  },
  {
    id: 1152, code: 'RK-1152', purpose: 'sale', type: 'duplex', areaId: 'sheikh-zayed',
    title: { ar: 'دوبلكس بجاردن', en: 'Duplex with garden' },
    compound: { ar: 'زيد ويست', en: 'Zed West' },
    city: { ar: 'الشيخ زايد', en: 'Sheikh Zayed' },
    price: 16_750_000, beds: 4, baths: 3, size: 240,
    finishing: { ar: 'نصف تشطيب', en: 'Semi-finished' },
    delivery: { ar: 'استلام 2027', en: 'Delivery 2027' },
    images: [P.townhouseCorner, P.apartmentGarden, P.apartmentLandscape],
    description: {
      ar: 'دوبلكس بجاردن خاصة وتراس، في كمبوند متكامل الخدمات.',
      en: 'Duplex with a private garden and terrace in a fully serviced compound.'
    },
    paymentPlans: salePlans(16_750_000),
    features: [F.green, F.clubhouse, F.gym, F.security],
    featured: false, commission: 2.5
  },
  {
    id: 1131, code: 'RK-1131', purpose: 'sale', type: 'apartment', areaId: 'new-cairo',
    title: { ar: 'شقة كورنر', en: 'Corner apartment' },
    compound: { ar: 'ذا سكوير', en: 'The Square' },
    city: { ar: 'التجمع الخامس', en: 'Fifth Settlement' },
    price: 7_650_000, beds: 3, baths: 2, size: 158,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام 2026', en: 'Delivery 2026' },
    images: [P.apartmentFurnished, P.apartmentLandscape, P.apartmentGarden],
    description: {
      ar: 'شقة كورنر بواجهتين وإضاءة طبيعية طول اليوم.',
      en: 'Corner apartment with two facades and natural light all day.'
    },
    paymentPlans: salePlans(7_650_000),
    features: [F.security, F.elevator, F.parking],
    featured: false, commission: 2.5
  },
  {
    id: 988, code: 'RK-0988', purpose: 'sale', type: 'studio', areaId: 'new-cairo',
    title: { ar: 'ستوديو', en: 'Studio' },
    compound: { ar: 'تاج سيتي', en: 'Taj City' },
    city: { ar: 'مدينة نصر', en: 'Nasr City' },
    price: 3_950_000, beds: 1, baths: 1, size: 68,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام 2026', en: 'Delivery 2026' },
    images: [P.apartmentLandscape, P.apartmentFurnished, P.apartmentGarden],
    description: {
      ar: 'ستوديو عملي مناسب للاستثمار أو السكن الفردي.',
      en: 'A practical studio, great for investment or single living.'
    },
    paymentPlans: salePlans(3_950_000),
    features: [F.security, F.elevator],
    featured: false, commission: 2
  },
  {
    id: 1160, code: 'RK-1160', purpose: 'rent', type: 'office', areaId: 'new-capital',
    title: { ar: 'مكتب إداري', en: 'Administrative office' },
    compound: { ar: 'الحي المالي', en: 'Financial District' },
    city: { ar: 'العاصمة الإدارية', en: 'New Capital' },
    price: 38_000, beds: 0, baths: 1, size: 85,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام فوري', en: 'Ready to move' },
    images: [IMAGES.invest.commercialMall, IMAGES.invest.hotelTower, P.apartmentFurnished],
    description: {
      ar: 'مكتب إداري متشطب بالكامل في برج إداري بخدمات متكاملة.',
      en: 'Fully finished office in an administrative tower with full services.'
    },
    paymentPlans: RENT_PLANS,
    features: [F.security, F.elevator, F.parking],
    featured: false, commission: 2
  },
  {
    id: 1171, code: 'RK-1171', purpose: 'rent', type: 'apartment', areaId: 'sheikh-zayed',
    title: { ar: 'شقة عائلية', en: 'Family apartment' },
    compound: { ar: 'بالم هيلز', en: 'Palm Hills' },
    city: { ar: 'الشيخ زايد', en: 'Sheikh Zayed' },
    price: 32_000, beds: 3, baths: 2, size: 175,
    finishing: { ar: 'تشطيب كامل', en: 'Fully finished' },
    delivery: { ar: 'استلام فوري', en: 'Ready to move' },
    images: [P.apartmentGarden, P.apartmentLandscape, P.apartmentFurnished],
    description: {
      ar: 'شقة عائلية واسعة قريبة من المدارس الدولية والمول.',
      en: 'Spacious family apartment near international schools and the mall.'
    },
    paymentPlans: RENT_PLANS,
    features: [F.schools, F.green, F.security, F.parking],
    featured: false, commission: 2
  },
  {
    id: 1180, code: 'RK-1180', purpose: 'sale', type: 'apartment', areaId: 'new-cairo',
    title: { ar: 'شقة على اللاندسكيب', en: 'Apartment on the landscape' },
    compound: { ar: 'ماونتن فيو', en: 'Mountain View' },
    city: { ar: 'التجمع الخامس', en: 'Fifth Settlement' },
    price: 6_980_000, beds: 2, baths: 2, size: 145,
    finishing: { ar: 'نصف تشطيب', en: 'Semi-finished' },
    delivery: { ar: 'استلام 2027', en: 'Delivery 2027' },
    images: [P.apartmentLandscape, P.apartmentGarden, P.townhouseCorner],
    description: {
      ar: 'شقة على لاندسكيب مفتوح بأقساط مريحة.',
      en: 'Apartment overlooking an open landscape with easy installments.'
    },
    paymentPlans: salePlans(6_980_000),
    features: [F.green, F.security, F.clubhouse],
    featured: false, commission: 2.5
  }
];

const BY_ID = new Map(PROPERTIES.map(p => [p.id, p]));

export function findProperty(id: number): Property | undefined {
  return BY_ID.get(id);
}
