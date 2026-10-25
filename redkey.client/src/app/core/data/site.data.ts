import { IMAGES } from './images';
import { AppNotification, AreaInfo, Opportunity, PropertyType, Testimonial } from './models';
import type { IconName } from '../../shared/components/icon/icons';

export const OPPORTUNITIES: readonly Opportunity[] = [
  {
    id: 1,
    title: { ar: 'برج تايم — غرف فندقية', en: 'Time Tower — hotel rooms' },
    fullTitle: { ar: 'برج تايم — 120 غرفة فندقية مؤجرة', en: 'Time Tower — 120 leased hotel rooms' },
    area: { ar: 'العاصمة الإدارية', en: 'New Capital' },
    subtitle: { ar: 'الحي المالي، العاصمة الإدارية · إدارة فندقية بعقد 10 سنين', en: 'Financial District, New Capital · 10-year hotel management contract' },
    image: IMAGES.invest.hotelTower,
    yield: 13.5, sharePrice: 5_000, durationYears: 5, interested: 23,
    description: {
      ar: 'الإيجار بيتوزع على المستثمرين كل شهر، وفي آخر المدة العقار بيتباع وكل مستثمر بياخد نصيبه. العوائد متوقعة ومش مضمونة.',
      en: 'Rent is distributed to investors every month; at the end of the term the property is sold and each investor takes their share. Returns are expected, not guaranteed.'
    }
  },
  {
    id: 2,
    title: { ar: 'عمارة سكنية مؤجرة', en: 'Leased residential building' },
    fullTitle: { ar: 'عمارة سكنية مؤجرة بالكامل — 24 شقة', en: 'Fully leased residential building — 24 units' },
    area: { ar: 'مدينة نصر', en: 'Nasr City' },
    subtitle: { ar: 'مدينة نصر، القاهرة · عقود إيجار سنوية', en: 'Nasr City, Cairo · annual lease contracts' },
    image: IMAGES.properties.apartmentGarden,
    yield: 11, sharePrice: 2_500, durationYears: 4, interested: 11,
    description: {
      ar: 'عمارة مؤجرة بالكامل لعائلات بعقود سنوية، والإيجار بيتوزع كل 3 شهور. العوائد متوقعة ومش مضمونة.',
      en: 'A building fully leased to families on annual contracts; rent is distributed quarterly. Returns are expected, not guaranteed.'
    }
  },
  {
    id: 3,
    title: { ar: 'شاليهات إيجار موسمي', en: 'Seasonal rental chalets' },
    fullTitle: { ar: 'شاليهات إيجار موسمي — 18 شاليه', en: 'Seasonal rental chalets — 18 chalets' },
    area: { ar: 'الساحل الشمالي', en: 'North Coast' },
    subtitle: { ar: 'الساحل الشمالي · إدارة تشغيل موسمية', en: 'North Coast · seasonal operation management' },
    image: IMAGES.properties.chaletSea,
    yield: 16, sharePrice: 2_000, durationYears: 3, interested: 7,
    description: {
      ar: 'شاليهات بتتأجر في الصيف بإدارة متخصصة، والأرباح بتتوزع في آخر كل موسم. العوائد متوقعة ومش مضمونة.',
      en: 'Chalets rented during summer by a specialised operator; profits are distributed at the end of each season. Returns are expected, not guaranteed.'
    }
  },
  {
    id: 4,
    title: { ar: 'مول تجاري', en: 'Retail mall' },
    fullTitle: { ar: 'مول تجاري — 40 محل مؤجر', en: 'Retail mall — 40 leased shops' },
    area: { ar: 'الشيخ زايد', en: 'Sheikh Zayed' },
    subtitle: { ar: 'الشيخ زايد · عقود إيجار تجارية طويلة', en: 'Sheikh Zayed · long commercial leases' },
    image: IMAGES.invest.commercialMall,
    yield: 12, sharePrice: 10_000, durationYears: 6, interested: 15,
    description: {
      ar: 'مول مؤجر لعلامات تجارية معروفة بعقود طويلة، والإيجار بيتوزع كل شهر. العوائد متوقعة ومش مضمونة.',
      en: 'A mall leased to well-known brands on long contracts; rent is distributed monthly. Returns are expected, not guaranteed.'
    }
  }
];

export function findOpportunity(id: number): Opportunity | undefined {
  return OPPORTUNITIES.find(o => o.id === id);
}

export const AREAS: readonly AreaInfo[] = [
  { id: 'new-cairo', name: { ar: 'القاهرة الجديدة', en: 'New Cairo' }, image: IMAGES.areas.newCairo },
  { id: 'sheikh-zayed', name: { ar: 'الشيخ زايد', en: 'Sheikh Zayed' }, image: IMAGES.areas.sheikhZayed },
  { id: 'north-coast', name: { ar: 'الساحل الشمالي', en: 'North Coast' }, image: IMAGES.areas.northCoast },
  { id: 'new-capital', name: { ar: 'العاصمة الإدارية', en: 'New Capital' }, image: IMAGES.areas.newCapital }
];

export const PROPERTY_TYPE_ICONS: Readonly<Record<PropertyType, IconName>> = {
  apartment: 'building',
  villa: 'home',
  townhouse: 'grid',
  chalet: 'sun',
  duplex: 'layers',
  office: 'briefcase',
  studio: 'key'
};

export const TESTIMONIALS: readonly Testimonial[] = [
  {
    name: { ar: 'محمد عبد الله', en: 'Mohamed Abdallah' },
    role: { ar: 'اشترى شقة في تاج سيتي', en: 'Bought an apartment in Taj City' },
    quote: {
      ar: 'من أول مكالمة لحد الاستلام كان في حد بيتابع معايا خطوة بخطوة. التجربة كانت أسهل بكتير من اللي كنت متوقعه.',
      en: 'From the first call until handover someone followed up with me step by step. Much easier than I expected.'
    }
  },
  {
    name: { ar: 'ليلى حسام', en: 'Laila Hossam' },
    role: { ar: 'مستثمرة في برج تايم', en: 'Investor in Time Tower' },
    quote: {
      ar: 'الاستثمار بالحصص خلاني أدخل السوق العقاري بمبلغ صغير، والتقارير بتوصلني كل شهر.',
      en: 'Fractional investing let me enter real estate with a small amount, and I get reports every month.'
    }
  },
  {
    name: { ar: 'كريم وجيه', en: 'Karim Wageh' },
    role: { ar: 'وسيط معتمد', en: 'Certified broker' },
    quote: {
      ar: 'العمولة واضحة ومكتوبة على كل وحدة، والعميل بيتحجز باسمي. ده خلاني أشتغل وأنا مطمن.',
      en: 'The commission is clear on every unit and my clients are reserved under my name — I work with peace of mind.'
    }
  }
];

export const CUSTOMER_NOTIFICATIONS: readonly AppNotification[] = [
  {
    id: 1, icon: 'calendar', read: false,
    title: { ar: 'تأكيد المعاينة', en: 'Viewing confirmed' },
    body: { ar: 'معاينة RK-1042 يوم السبت الساعة 5 مساءً.', en: 'RK-1042 viewing on Saturday at 5 PM.' },
    time: { ar: 'من 10 دقايق', en: '10 min ago' }
  },
  {
    id: 2, icon: 'trending', read: false,
    title: { ar: 'السعر نزل', en: 'Price dropped' },
    body: { ar: 'شاليه مراسي اللي في المفضلة نزل 5%.', en: 'The Marassi chalet in your favorites dropped 5%.' },
    time: { ar: 'من ساعة', en: '1 hour ago' }
  },
  {
    id: 3, icon: 'building', read: true,
    title: { ar: 'وحدات جديدة', en: 'New units' },
    body: { ar: '6 وحدات جديدة في التجمع الخامس.', en: '6 new units in the Fifth Settlement.' },
    time: { ar: 'امبارح', en: 'Yesterday' }
  }
];

export const BROKER_NOTIFICATIONS: readonly AppNotification[] = [
  {
    id: 1, icon: 'users', read: false,
    title: { ar: 'عميلك طلب معاينة', en: 'Your client requested a viewing' },
    body: { ar: 'نهى سمير طلبت معاينة وحدة ميفيدا.', en: 'Noha Samir requested a viewing in Mivida.' },
    time: { ar: 'من 20 دقيقة', en: '20 min ago' }
  },
  {
    id: 2, icon: 'money', read: false,
    title: { ar: 'العمولة جاهزة', en: 'Commission ready' },
    body: { ar: 'عمولة منى حسن 162,500 ج.م جاهزة للصرف.', en: 'Mona Hassan commission EGP 162,500 is ready.' },
    time: { ar: 'النهارده', en: 'Today' }
  },
  {
    id: 3, icon: 'checkCircle', read: true,
    title: { ar: 'شقتك اتنشرت', en: 'Your listing is live' },
    body: { ar: 'دوبلكس مدينتي B12 بقى ظاهر للعملاء.', en: 'Madinaty B12 duplex is now visible to customers.' },
    time: { ar: 'امبارح', en: 'Yesterday' }
  }
];

export const ADMIN_NOTIFICATIONS: readonly AppNotification[] = [
  {
    id: 1, icon: 'inbox', read: false,
    title: { ar: 'طلب معاينة جديد', en: 'New viewing request' },
    body: { ar: 'محمد عبد الله · RK-1042 تاج سيتي.', en: 'Mohamed Abdallah · RK-1042 Taj City.' },
    time: { ar: 'من 5 دقايق', en: '5 min ago' }
  },
  {
    id: 2, icon: 'users', read: false,
    title: { ar: 'طلب انضمام وسيط', en: 'Broker join request' },
    body: { ar: 'أحمد سامي بعت طلب انضمام.', en: 'Ahmed Samy sent a join request.' },
    time: { ar: 'النهارده', en: 'Today' }
  },
  {
    id: 3, icon: 'trending', read: true,
    title: { ar: 'مهتم بالاستثمار', en: 'New investor lead' },
    body: { ar: 'ليلى حسام مهتمة ببرج تايم.', en: 'Laila Hossam is interested in Time Tower.' },
    time: { ar: 'من 20 دقيقة', en: '20 min ago' }
  }
];
