import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { UnitListItemDto } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginRequired } from '../../../core/auth/login-required.service';
import { FavoritesService } from '../../../core/data/favorites.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-property-card',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, MoneyPipe],
  templateUrl: './property-card.html',
  styleUrl: './property-card.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyCard {
  protected readonly i18n = inject(I18nService);
  private readonly favorites = inject(FavoritesService);
  private readonly auth = inject(AuthService);
  private readonly loginRequired = inject(LoginRequired);

  readonly unit = input.required<UnitListItemDto>();
  /** Above the fold → load eagerly (LCP). */
  readonly priority = input(false);
  /** Where the card leads (dashboards open their own pages). */
  readonly link = input<string | null>(null);
  /** Hides the favorite button (dashboards). */
  readonly showFavorite = input(true);

  protected readonly href = computed(() => this.link() ?? `/properties/${this.unit().id}`);
  protected readonly isFavorite = computed(
    () => this.auth.isAuthenticated() && this.favorites.ids().has(this.unit().id)
  );

  protected toggleFavorite(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.loginRequired.check()) return;
    this.favorites.toggle(this.unit().id);
  }
}
