import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import {
  AreaDto,
  CityDto,
  CustomerLookupsClient,
  CustomerUnitsClient,
  LandingLookupsClient,
  LandingUnitsClient,
  LookupsDto,
  PagedResultOfUnitListItemDto,
  UnitDetailsDto,
  UnitTypeDto,
  UserRole
} from '../api/redkey-api.generated';
import { AuthService } from '../auth/auth.service';
import { Lang } from '../i18n/i18n.models';

export interface UnitsFilter {
  purpose?: number | null;
  unitTypeId?: number | null;
  cityId?: number | null;
  areaId?: number | null;
  maxPrice?: number | null;
  search?: string | null;
  featured?: boolean | null;
  page?: number | null;
  pageSize?: number | null;
}

/**
 * Website units (landing + customer pages share the same screens): visitors use the landing
 * endpoints, a signed-in customer uses the customer endpoints. Both always send the language.
 */
@Injectable({ providedIn: 'root' })
export class UnitsCatalog {
  private readonly auth = inject(AuthService);
  private readonly landingUnits = inject(LandingUnitsClient);
  private readonly landingLookups = inject(LandingLookupsClient);
  private readonly customerUnits = inject(CustomerUnitsClient);
  private readonly customerLookups = inject(CustomerLookupsClient);

  /** Lookups rarely change: one request per language per page load. */
  private readonly cache = new Map<string, Observable<unknown>>();

  private get isCustomer(): boolean {
    return this.auth.role() === UserRole.Customer;
  }

  units(f: UnitsFilter, lang: Lang): Observable<PagedResultOfUnitListItemDto> {
    const args = [f.purpose, f.unitTypeId, f.cityId, f.areaId, f.maxPrice, f.search || null, f.featured, f.page, f.pageSize, lang] as const;
    return this.isCustomer ? this.customerUnits.getUnits(...args) : this.landingUnits.getUnits(...args);
  }

  unit(id: string, lang: Lang): Observable<UnitDetailsDto> {
    return this.isCustomer ? this.customerUnits.getUnit(id, lang) : this.landingUnits.getUnit(id, lang);
  }

  unitTypes(lang: Lang): Observable<UnitTypeDto[]> {
    return this.cached(`types:${lang}`, () =>
      this.isCustomer ? this.customerLookups.getUnitTypes(lang) : this.landingLookups.getUnitTypes(lang));
  }

  cities(lang: Lang): Observable<CityDto[]> {
    return this.cached(`cities:${lang}`, () =>
      this.isCustomer ? this.customerLookups.getCities(lang) : this.landingLookups.getCities(lang));
  }

  /** All active areas (each has its `cityId`); pages filter them by the selected city. */
  areas(lang: Lang): Observable<AreaDto[]> {
    return this.cached(`areas:${lang}`, () =>
      this.isCustomer ? this.customerLookups.getAreas(null, lang) : this.landingLookups.getAreas(null, lang));
  }

  lookups(lang: Lang): Observable<LookupsDto> {
    return this.cached(`lookups:${lang}`, () =>
      this.isCustomer ? this.customerLookups.getLookups(lang) : this.landingLookups.getLookups(lang));
  }

  private cached<T>(key: string, load: () => Observable<T>): Observable<T> {
    let request = this.cache.get(key) as Observable<T> | undefined;
    if (!request) {
      request = load().pipe(shareReplay({ bufferSize: 1, refCount: false }));
      this.cache.set(key, request);
    }
    return request;
  }
}
