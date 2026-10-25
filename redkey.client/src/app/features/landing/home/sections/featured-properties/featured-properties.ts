import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Purpose } from '../../../../../core/data/models';
import { PROPERTIES } from '../../../../../core/data/properties.data';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { Icon } from '../../../../../shared/components/icon/icon';
import { PropertyCard } from '../../../../../shared/components/property-card/property-card';

const FEATURED = PROPERTIES.filter(p => p.featured);

@Component({
  selector: 'app-featured-properties',
  imports: [RouterLink, Icon, PropertyCard],
  templateUrl: './featured-properties.html',
  styleUrl: './featured-properties.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FeaturedProperties {
  protected readonly i18n = inject(I18nService);

  protected readonly filter = signal<Purpose | 'all'>('all');
  protected readonly items = computed(() => {
    const f = this.filter();
    return (f === 'all' ? FEATURED : FEATURED.filter(p => p.purpose === f)).slice(0, 6);
  });
}
