import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { UnitsCatalog } from '../../../../../core/units/units-catalog.service';
import { Icon } from '../../../../../shared/components/icon/icon';
import { PropertyCard } from '../../../../../shared/components/property-card/property-card';

@Component({
  selector: 'app-featured-properties',
  imports: [TranslatePipe, RouterLink, Icon, PropertyCard],
  templateUrl: './featured-properties.html',
  styleUrl: './featured-properties.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FeaturedProperties {
  protected readonly i18n = inject(I18nService);
  private readonly catalog = inject(UnitsCatalog);

  /** null = all purposes. */
  protected readonly purpose = signal<number | null>(null);

  private readonly lookups = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.lookups(params) });
  protected readonly purposes = computed(() => this.lookups.value()?.unitPurposes ?? []);

  protected readonly units = rxResource({
    params: () => ({ lang: this.i18n.lang(), purpose: this.purpose() }),
    stream: ({ params }) => this.catalog.units({ featured: true, purpose: params.purpose, pageSize: 6 }, params.lang)
  });
  protected readonly items = computed(() => this.units.value()?.items ?? []);
}
