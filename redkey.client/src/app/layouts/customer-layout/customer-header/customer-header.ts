import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CUSTOMER_NOTIFICATIONS } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';
import { LanguageSwitcher } from '../../../shared/components/language-switcher/language-switcher';
import { Logo } from '../../../shared/components/logo/logo';
import { NotificationMenu } from '../../../shared/components/notification-menu/notification-menu';
import { UserMenu, UserMenuLink } from '../../../shared/components/user-menu/user-menu';

@Component({
  selector: 'app-customer-header',
  imports: [RouterLink, RouterLinkActive, Icon, Logo, LanguageSwitcher, NotificationMenu, UserMenu],
  templateUrl: './customer-header.html',
  styleUrl: './customer-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CustomerHeader {
  protected readonly i18n = inject(I18nService);

  protected readonly menuOpen = signal(false);
  protected readonly notifications = CUSTOMER_NOTIFICATIONS;

  protected readonly menuLinks = computed<UserMenuLink[]>(() => {
    const t = this.i18n.t().account;
    return [
      { label: t.profile, link: '/customer/profile', icon: 'user' },
      { label: t.favorites, link: '/customer/favorites', icon: 'heart' }
    ];
  });
}
