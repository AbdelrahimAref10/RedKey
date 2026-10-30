import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { CustomerFavoritesClient } from '../../../core/api/redkey-api.generated';
import { FavoritesService } from '../../../core/data/favorites.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

/** The customer's saved units, from the server (same list on the website and the mobile app). */
@Component({
  selector: 'app-favorites',
  imports: [TranslatePipe, RouterLink, Icon, PropertyCard, Pagination],
  templateUrl: './favorites.html',
  styleUrl: './favorites.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Favorites {
  private readonly client = inject(CustomerFavoritesClient);
  protected readonly i18n = inject(I18nService);
  protected readonly favorites = inject(FavoritesService);

  protected readonly page = signal(1);

  /** Reloads after a heart is toggled here or anywhere else. */
  private readonly list = rxResource({
    params: () => ({ page: this.page(), lang: this.i18n.lang(), version: this.favorites.version() }),
    stream: ({ params }) => this.client.getFavorites(params.page, null, params.lang)
  });

  protected readonly result = computed(() => this.list.value());
  /** Hearts removed on this page disappear at once (before the reload). */
  protected readonly items = computed(() => {
    const ids = this.favorites.ids();
    return (this.list.value()?.items ?? []).filter(u => ids.has(u.id));
  });
  protected readonly total = computed(() => this.list.value()?.totalCount ?? this.favorites.count());
  protected readonly loading = this.list.isLoading;
}
