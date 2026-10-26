import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { catchError, forkJoin, map, of, tap } from 'rxjs';
import { UnitListItemDto } from '../../../core/api/redkey-api.generated';
import { FavoritesService } from '../../../core/data/favorites.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { UnitsCatalog } from '../../../core/units/units-catalog.service';
import { Icon } from '../../../shared/components/icon/icon';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

@Component({
  selector: 'app-favorites',
  imports: [TranslatePipe, RouterLink, Icon, PropertyCard],
  templateUrl: './favorites.html',
  styleUrl: './favorites.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Favorites {
  protected readonly i18n = inject(I18nService);
  protected readonly favorites = inject(FavoritesService);
  private readonly catalog = inject(UnitsCatalog);

  /** Saved ids → units (ids of deleted / unpublished units are forgotten). */
  private readonly units = rxResource({
    params: () => ({ ids: [...this.favorites.ids()], lang: this.i18n.lang() }),
    stream: ({ params }) => {
      if (params.ids.length === 0) return of<UnitListItemDto[]>([]);
      return forkJoin(
        params.ids.map(id => this.catalog.unit(id, params.lang).pipe(
          map(d => d.unit),
          catchError(() => of(null))
        ))
      ).pipe(
        tap(list => this.favorites.forget(params.ids.filter((_, i) => list[i] === null))),
        map(list => list.filter((u): u is UnitListItemDto => u !== null))
      );
    }
  });

  protected readonly items = computed(() => this.units.value() ?? []);
  protected readonly loading = this.units.isLoading;
}
