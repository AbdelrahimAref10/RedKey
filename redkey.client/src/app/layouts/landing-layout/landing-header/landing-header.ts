import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { IsActiveMatchOptions, Params, RouterLink, RouterLinkActive } from '@angular/router';
import { UserRole } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { COMPANY } from '../../../core/data/company';
import { CUSTOMER_NOTIFICATIONS } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
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

const EXACT: IsActiveMatchOptions = { paths: 'exact', queryParams: 'ignored', matrixParams: 'ignored', fragment: 'ignored' };
const SUBSET: IsActiveMatchOptions = { paths: 'subset', queryParams: 'subset', matrixParams: 'ignored', fragment: 'ignored' };

@Component({
  selector: 'app-landing-header',
  imports: [RouterLink, RouterLinkActive, Icon, Logo, LanguageSwitcher, NotificationMenu, UserMenu],
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
  protected readonly notifications = CUSTOMER_NOTIFICATIONS;

  protected readonly menuLinks = computed<UserMenuLink[]>(() => {
    const t = this.i18n.t().account;
    return [
      { label: t.profile, link: '/profile', icon: 'user' },
      { label: t.myUnits, link: '/my-units', icon: 'home' },
      { label: t.myInvestments, link: '/my-investments', icon: 'trending' },
      { label: t.favorites, link: '/favorites', icon: 'heart' }
    ];
  });

  /** Same navigation as the prototype website: buy · rent · invest · about. */
  protected readonly links: readonly NavLink[] = [
    { key: 'home', path: '/', match: EXACT },
    { key: 'buy', path: '/properties', query: { purpose: 'sale' }, match: SUBSET },
    { key: 'rent', path: '/properties', query: { purpose: 'rent' }, match: SUBSET },
    { key: 'invest', path: '/invest', match: SUBSET },
    { key: 'about', path: '/contact', match: SUBSET }
  ];
}
