import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AdminFavoritesClient, UnitListItemDto } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { timeAgo } from '../../../core/i18n/time-ago';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';

/** UnitPurpose.Rent (back-end enum): the price is monthly. */
const RENT = 2;

/** Who saved what (with phone / WhatsApp for follow-up) + the most saved units. */
@Component({
  selector: 'app-admin-favorites',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, MoneyPipe, Pagination],
  templateUrl: './admin-favorites.html',
  styleUrl: './admin-favorites.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminFavorites {
  private readonly client = inject(AdminFavoritesClient);
  protected readonly i18n = inject(I18nService);

  protected readonly tab = signal<'user' | 'unit'>('user');
  protected readonly search = signal('');
  /** "Most saved" row clicked → the savers of that unit. */
  protected readonly unitFilter = signal<UnitListItemDto | null>(null);
  protected readonly userPage = signal(1);
  protected readonly unitPage = signal(1);
  private searchTimer: ReturnType<typeof setTimeout> | undefined;

  protected readonly byUser = rxResource({
    params: () => (this.tab() === 'user'
      ? { search: this.search(), unit: this.unitFilter()?.id ?? null, page: this.userPage(), lang: this.i18n.lang() }
      : undefined),
    stream: ({ params }) => this.client.getFavorites(params!.search || null, params!.unit, params!.page, null)
  });

  protected readonly byUnit = rxResource({
    params: () => (this.tab() === 'unit' ? { page: this.unitPage(), lang: this.i18n.lang() } : undefined),
    stream: ({ params }) => this.client.getByUnit(params!.page, null)
  });

  /** Bar length relative to the most saved unit of the page. */
  protected readonly maxSaved = computed(() => Math.max(1, ...(this.byUnit.value()?.items ?? []).map(r => r.savedCount)));

  constructor() {
    inject(DestroyRef).onDestroy(() => clearTimeout(this.searchTimer));
  }

  protected onSearch(value: string): void {
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => {
      this.search.set(value.trim());
      this.userPage.set(1);
    }, 350);
  }

  protected showSavers(unit: UnitListItemDto): void {
    this.unitFilter.set(unit);
    this.userPage.set(1);
    this.tab.set('user');
  }

  protected clearUnitFilter(): void {
    this.unitFilter.set(null);
    this.userPage.set(1);
  }

  protected isRent(unit: UnitListItemDto): boolean {
    return unit.purpose === RENT;
  }

  protected time(utc: string): string {
    return timeAgo(utc, this.i18n.lang());
  }

  protected whatsapp(phone: string, name: string, unit: UnitListItemDto): string {
    const text = this.i18n.instant('admin.favorites.whatsappText', { name: name.split(' ')[0], unit: `${unit.code} · ${unit.title}` });
    return `https://wa.me/${phone.replace(/\D/g, '')}?text=${encodeURIComponent(text)}`;
  }
}
