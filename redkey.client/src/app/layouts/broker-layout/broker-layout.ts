import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { BROKER_NOTIFICATIONS } from '../../core/data/site.data';
import { I18nService } from '../../core/i18n/i18n.service';
import { DashboardShell, ShellNavItem } from '../../shared/components/dashboard-shell/dashboard-shell';
import { UserMenuLink } from '../../shared/components/user-menu/user-menu';

/** Layout 4/4 — broker dashboard. */
@Component({
  selector: 'app-broker-layout',
  imports: [DashboardShell],
  templateUrl: './broker-layout.html',
  styleUrl: './broker-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerLayout {
  protected readonly i18n = inject(I18nService);
  protected readonly notifications = BROKER_NOTIFICATIONS;

  protected readonly nav = computed<ShellNavItem[]>(() => {
    const n = this.i18n.t().broker.nav;
    return [
      { label: n.dashboard, link: '/broker', icon: 'grid', exact: true },
      { label: n.units, link: '/broker/units', icon: 'building' },
      { label: n.clients, link: '/broker/clients', icon: 'users' },
      { label: n.listings, link: '/broker/listings', icon: 'home' },
      { label: n.profile, link: '/broker/profile', icon: 'user' }
    ];
  });

  protected readonly menuLinks = computed<UserMenuLink[]>(() => [
    { label: this.i18n.t().account.profile, link: '/broker/profile', icon: 'user' }
  ]);
}
