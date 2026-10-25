/**
 * Every image of the site lives in `src/assets/images/...` (served as `assets/images/...`).
 * Replace a file with the same name to change it everywhere.
 */
export const IMAGES = {
  logo: 'assets/images/logo/redkey-logo.svg',
  logoMark: 'assets/images/logo/redkey-mark.svg',
  hero: 'assets/images/hero/hero-villa.jpg',
  authSide: 'assets/images/auth/auth-side.jpg',
  aboutTeam: 'assets/images/about/about-team.jpg',
  properties: {
    apartmentLandscape: 'assets/images/properties/apartment-landscape.jpg',
    townhouseCorner: 'assets/images/properties/townhouse-corner.jpg',
    apartmentFurnished: 'assets/images/properties/apartment-furnished.jpg',
    chaletSea: 'assets/images/properties/chalet-sea.jpg',
    apartmentGarden: 'assets/images/properties/apartment-garden.jpg'
  },
  invest: {
    hotelTower: 'assets/images/invest/hotel-tower.jpg',
    commercialMall: 'assets/images/invest/commercial-mall.jpg'
  },
  areas: {
    newCairo: 'assets/images/areas/new-cairo.jpg',
    sheikhZayed: 'assets/images/areas/sheikh-zayed.jpg',
    northCoast: 'assets/images/areas/north-coast.jpg',
    newCapital: 'assets/images/areas/new-capital.jpg'
  }
} as const;
