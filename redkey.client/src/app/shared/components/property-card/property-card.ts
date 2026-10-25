import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FavoritesService } from '../../../core/data/favorites.service';
import { Property } from '../../../core/data/models';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-property-card',
  imports: [NgOptimizedImage, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './property-card.html',
  styleUrl: './property-card.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyCard {
  protected readonly i18n = inject(I18nService);
  private readonly favorites = inject(FavoritesService);

  readonly property = input.required<Property>();
  /** Route prefix of the current layout: '' (landing) or '/customer'. */
  readonly base = input('');
  /** Above the fold → load eagerly (LCP). */
  readonly priority = input(false);

  protected readonly link = computed(() => [this.base() + '/properties', this.property().id]);
  protected readonly isFavorite = computed(() => this.favorites.ids().has(this.property().id));

  protected toggleFavorite(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.favorites.toggle(this.property().id);
  }
}
