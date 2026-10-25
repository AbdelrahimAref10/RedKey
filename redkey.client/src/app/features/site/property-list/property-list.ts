import { ChangeDetectionStrategy, Component, computed, inject, input, linkedSignal, signal } from '@angular/core';
import { AreaId, PropertyType, Purpose } from '../../../core/data/models';
import { PROPERTIES } from '../../../core/data/properties.data';
import { AREAS } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

const TYPES: readonly PropertyType[] = ['apartment', 'villa', 'townhouse', 'chalet', 'duplex', 'office', 'studio'];

@Component({
  selector: 'app-property-list',
  imports: [PropertyCard, Icon, LocPipe],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyList {
  protected readonly i18n = inject(I18nService);

  // Route data + query params (bound by withComponentInputBinding).
  readonly base = input('');
  readonly purpose = input<string>();
  readonly type = input<string>();
  readonly area = input<string>();
  readonly maxPrice = input<string>();

  protected readonly types = TYPES;
  protected readonly areas = AREAS;

  protected readonly purposeFilter = linkedSignal<Purpose | 'all'>(() => {
    const p = this.purpose();
    return p === 'sale' || p === 'rent' ? p : 'all';
  });
  protected readonly typeFilter = linkedSignal<PropertyType | ''>(() => (this.type() as PropertyType) ?? '');
  protected readonly areaFilter = linkedSignal<AreaId | ''>(() => (this.area() as AreaId) ?? '');
  protected readonly priceFilter = linkedSignal(() => Number(this.maxPrice()) || 0);
  protected readonly query = signal('');

  protected readonly results = computed(() => {
    const purpose = this.purposeFilter();
    const type = this.typeFilter();
    const area = this.areaFilter();
    const maxPrice = this.priceFilter();
    const q = this.query().trim().toLowerCase();

    return PROPERTIES.filter(
      p =>
        (purpose === 'all' || p.purpose === purpose) &&
        (!type || p.type === type) &&
        (!area || p.areaId === area) &&
        (!maxPrice || p.purpose === 'rent' || p.price <= maxPrice) &&
        (!q ||
          p.code.toLowerCase().includes(q) ||
          p.compound.ar.includes(q) ||
          p.city.ar.includes(q) ||
          p.compound.en.toLowerCase().includes(q) ||
          p.city.en.toLowerCase().includes(q))
    );
  });

  protected readonly title = computed(() => {
    const t = this.i18n.t();
    const purpose = this.purposeFilter();
    return purpose === 'sale' ? t.site.listings.forSale : purpose === 'rent' ? t.site.listings.forRent : t.nav.properties;
  });

  protected reset(): void {
    this.purposeFilter.set('all');
    this.typeFilter.set('');
    this.areaFilter.set('');
    this.priceFilter.set(0);
    this.query.set('');
  }
}
