import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FavoritesService } from '../../../core/data/favorites.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

@Component({
  selector: 'app-favorites',
  imports: [RouterLink, Icon, PropertyCard],
  templateUrl: './favorites.html',
  styleUrl: './favorites.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Favorites {
  protected readonly i18n = inject(I18nService);
  protected readonly favorites = inject(FavoritesService);
}
