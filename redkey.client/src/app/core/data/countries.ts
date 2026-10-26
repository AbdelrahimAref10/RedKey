/**
 * Country calling codes (ITU E.164), keyed by ISO 3166 alpha-2.
 * Names are not stored: they come from `Intl.DisplayNames` in the UI language. Flags: assets/flags/{iso}.svg.
 */
export interface Country {
  iso: string;
  dial: string;
}

const DIAL_CODES: Readonly<Record<string, string>> = {
  // Arab world first in the data; the picker sorts by name anyway.
  EG: '20', SA: '966', AE: '971', KW: '965', QA: '974', BH: '973', OM: '968', JO: '962', LB: '961',
  SY: '963', IQ: '964', PS: '970', YE: '967', LY: '218', TN: '216', DZ: '213', MA: '212', SD: '249',
  SS: '211', MR: '222', SO: '252', DJ: '253', KM: '269',

  AF: '93', AL: '355', AD: '376', AO: '244', AI: '1264', AG: '1268', AR: '54', AM: '374', AW: '297',
  AU: '61', AT: '43', AZ: '994', BS: '1242', BD: '880', BB: '1246', BY: '375', BE: '32', BZ: '501',
  BJ: '229', BM: '1441', BT: '975', BO: '591', BA: '387', BW: '267', BR: '55', BN: '673', BG: '359',
  BF: '226', BI: '257', KH: '855', CM: '237', CA: '1', CV: '238', KY: '1345', CF: '236', TD: '235',
  CL: '56', CN: '86', CO: '57', CG: '242', CD: '243', CR: '506', CI: '225', HR: '385', CU: '53',
  CW: '599', CY: '357', CZ: '420', DK: '45', DM: '1767', DO: '1809', EC: '593', SV: '503', GQ: '240',
  ER: '291', EE: '372', SZ: '268', ET: '251', FJ: '679', FI: '358', FR: '33', GF: '594', PF: '689',
  GA: '241', GM: '220', GE: '995', DE: '49', GH: '233', GI: '350', GR: '30', GL: '299', GD: '1473',
  GP: '590', GU: '1671', GT: '502', GN: '224', GW: '245', GY: '592', HT: '509', HN: '504', HK: '852',
  HU: '36', IS: '354', IN: '91', ID: '62', IR: '98', IE: '353', IT: '39', JM: '1876', JP: '81',
  KZ: '7', KE: '254', KI: '686', XK: '383', KG: '996', LA: '856', LV: '371', LS: '266', LR: '231',
  LI: '423', LT: '370', LU: '352', MO: '853', MG: '261', MW: '265', MY: '60', MV: '960', ML: '223',
  MT: '356', MH: '692', MQ: '596', MU: '230', YT: '262', MX: '52', FM: '691', MD: '373', MC: '377',
  MN: '976', ME: '382', MS: '1664', MZ: '258', MM: '95', NA: '264', NR: '674', NP: '977', NL: '31',
  NC: '687', NZ: '64', NI: '505', NE: '227', NG: '234', KP: '850', MK: '389', NO: '47', PK: '92',
  PW: '680', PA: '507', PG: '675', PY: '595', PE: '51', PH: '63', PL: '48', PT: '351', PR: '1787',
  RE: '262', RO: '40', RU: '7', RW: '250', KN: '1869', LC: '1758', VC: '1784', WS: '685', SM: '378',
  ST: '239', SN: '221', RS: '381', SC: '248', SL: '232', SG: '65', SX: '1721', SK: '421', SI: '386',
  SB: '677', ZA: '27', KR: '82', ES: '34', LK: '94', SR: '597', SE: '46', CH: '41', TW: '886',
  TJ: '992', TZ: '255', TH: '66', TL: '670', TG: '228', TO: '676', TT: '1868', TR: '90', TM: '993',
  TC: '1649', TV: '688', UG: '256', UA: '380', GB: '44', US: '1', UY: '598', UZ: '998', VU: '678',
  VA: '379', VE: '58', VN: '84', VG: '1284', VI: '1340', ZM: '260', ZW: '263'
};

export const COUNTRIES: readonly Country[] = Object.entries(DIAL_CODES).map(([iso, dial]) => ({ iso, dial }));

/** Default of every phone field. */
export const DEFAULT_COUNTRY = 'EG';

export function findCountry(iso: string | null | undefined): Country | undefined {
  return iso ? COUNTRIES.find(c => c.iso === iso.toUpperCase()) : undefined;
}

export function flagUrl(iso: string): string {
  return `assets/flags/${iso.toLowerCase()}.svg`;
}

/** "+201012345678" → Egypt + 1012345678 (longest calling code that matches; EG wins its own prefix). */
export function parseInternational(phone: string | null | undefined): { countryIso: string; number: string } | null {
  const raw = (phone ?? '').trim();
  if (!raw.startsWith('+') && !raw.startsWith('00')) return null;
  const digits = raw.replace(/\D/g, '').replace(/^00/, '');
  const match = COUNTRIES
    .filter(c => digits.startsWith(c.dial))
    .sort((a, b) => b.dial.length - a.dial.length || (a.iso === DEFAULT_COUNTRY ? -1 : 0))[0];
  return match ? { countryIso: match.iso, number: digits.slice(match.dial.length).replace(/^0+/, '') } : null;
}
