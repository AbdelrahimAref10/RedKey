import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, signal } from '@angular/core';
import { IsActiveMatchOptions, Params, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { UserRole } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { COMPANY } from '../../../core/data/company';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LiveNotifications } from '../../../core/notifications/live-notifications.service';
import { Icon } from '../../../shared/components/icon/icon';
import { LanguageSwitcher } from '../../../shared/components/language-switcher/language-switcher';
import { Logo } from '../../../shared/components/logo/logo';
import { NotificationMenu } from '../../../shared/components/notification-menu/notification-menu';
import { UserMenu, UserMenuLink } from '../../../shared/components/user-menu/user-menu';

interface NavLink {
  key: 'home' | 'buy' | 'rent' | 'invest' | 'about';
  path: string;
  query?: Params;
  match: IsActiveMatchOptions;
}

/** UnitPurpose values (back-end enum), the same the listing filter sends. */
const UNIT_PURPOSE = { sale: 1, rent: 2 } as const;

const EXACT: IsActiveMatchOptions = { paths: 'exact', queryParams: 'ignored', matrixParams: 'ignored', fragment: 'ignored' };
const SUBSET: IsActiveMatchOptions = { paths: 'subset', queryParams: 'subset', matrixParams: 'ignored', fragment: 'ignored' };

@Component({
  selector: 'app-landing-header',
  imports: [TranslatePipe, RouterLink, RouterLinkActive, Icon, Logo, LanguageSwitcher, NotificationMenu, UserMenu],
  templateUrl: './landing-header.html',
  styleUrl: './landing-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LandingHeader {
  protected readonly i18n = inject(I18nService);
  protected readonly auth = inject(AuthService);

  protected readonly company = COMPANY;
  protected readonly menuOpen = signal(false);
  protected readonly isCustomer = computed(() => this.auth.role() === UserRole.Customer);
  /** Real notifications of the signed-in customer (API + SignalR), same service as the dashboards. */
  protected readonly live = inject(LiveNotifications);

  constructor() {
    effect(() => {
      if (this.isCustomer()) this.live.start();
      else this.live.stop();
    });
    inject(DestroyRef).onDestroy(() => this.live.stop());
  }

  protected readonly menuLinks = computed<UserMenuLink[]>(() => {
    return [
      { label: this.i18n.instant('account.profile'), link: '/profile', icon: 'user' },
      { label: this.i18n.instant('account.myUnits'), link: '/my-units', icon: 'home' },
      { label: this.i18n.instant('account.myInvestments'), link: '/my-investments', icon: 'trending' },
      { label: this.i18n.instant('account.favorites'), link: '/favorites', icon: 'heart' }
    ];
  });

  /** Same navigation as the prototype website: buy · rent · invest · about. */
  protected readonly links: readonly NavLink[] = [
    { key: 'home', path: '/', match: EXACT },
    { key: 'buy', path: '/properties', query: { purpose: UNIT_PURPOSE.sale }, match: SUBSET },
    { key: 'rent', path: '/properties', query: { purpose: UNIT_PURPOSE.rent }, match: SUBSET },
    { key: 'invest', path: '/invest', match: SUBSET },
    { key: 'about', path: '/contact', match: SUBSET }
  ];
}
