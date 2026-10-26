import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, linkedSignal, numberAttribute } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Params, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { UnitsCatalog } from '../../../core/units/units-catalog.service';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

const optionalNumber = (value: unknown): number | null => {
  const n = Number(value);
  return value == null || value === '' || Number.isNaN(n) ? null : n;
};

/**
 * Units of the website, 10 per page. The URL query string is the single source of truth for
 * the filters (?purpose=1&type=2&city=1&area=3&maxPrice=…&search=…&page=2), so pages can be shared / reloaded.
 */
@Component({
  selector: 'app-property-list',
  imports: [TranslatePipe, PropertyCard, Icon, Pagination],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyList {
  private readonly router = inject(Router);
  private readonly catalog = inject(UnitsCatalog);
  protected readonly i18n = inject(I18nService);

  // Query params (bound by withComponentInputBinding).
  readonly purpose = input(null, { transform: optionalNumber });
  readonly type = input(null, { transform: optionalNumber });
  readonly city = input(null, { transform: optionalNumber });
  readonly area = input(null, { transform: optionalNumber });
  readonly maxPrice = input(null, { transform: optionalNumber });
  readonly search = input<string | undefined>();
  readonly page = input(1, { transform: (v: unknown) => Math.max(1, numberAttribute(v, 1)) });

  /** Search box text (debounced into the URL). */
  protected readonly searchText = linkedSignal(() => this.search() ?? '');
  private searchTimer: ReturnType<typeof setTimeout> | undefined;

  private readonly lookups = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.lookups(params) });
  private readonly types = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.unitTypes(params) });
  private readonly cityList = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.cities(params) });
  private readonly areaList = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.areas(params) });

  protected readonly purposes = computed(() => this.lookups.value()?.unitPurposes ?? []);
  protected readonly unitTypes = computed(() => this.types.value() ?? []);
  protected readonly cities = computed(() => this.cityList.value() ?? []);
  /** Areas of the selected city (all areas when no city is picked). */
  protected readonly areas = computed(() => {
    const city = this.city();
    const areas = this.areaList.value() ?? [];
    return city === null ? areas : areas.filter(a => a.cityId === city);
  });

  protected readonly units = rxResource({
    params: () => ({
      lang: this.i18n.lang(),
      purpose: this.purpose(),
      unitTypeId: this.type(),
      cityId: this.city(),
      areaId: this.area(),
      maxPrice: this.maxPrice(),
      search: this.search() ?? null,
      page: this.page()
    }),
    stream: ({ params }) => this.catalog.units(params, params.lang)
  });

  protected readonly result = computed(() => this.units.value());

  protected readonly title = computed(() => {
    const purpose = this.purposes().find(p => p.value === this.purpose());
    return purpose ? `${this.i18n.instant('nav.properties')} · ${purpose.name}` : this.i18n.instant('nav.properties');
  });

  constructor() {
    inject(DestroyRef).onDestroy(() => clearTimeout(this.searchTimer));
  }

  /** Any filter change goes back to page 1. */
  protected setFilter(params: Params): void {
    void this.router.navigate([], { queryParams: { ...params, page: null }, queryParamsHandling: 'merge' });
  }

  protected onSearch(value: string): void {
    this.searchText.set(value);
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(
      () => void this.router.navigate([], {
        queryParams: { search: value.trim() || null, page: null },
        queryParamsHandling: 'merge',
        replaceUrl: true
      }),
      400
    );
  }

  protected goToPage(page: number): void {
    void this.router.navigate([], { queryParams: { page }, queryParamsHandling: 'merge' });
  }

  protected reset(): void {
    void this.router.navigate([], { queryParams: {} });
  }
}
