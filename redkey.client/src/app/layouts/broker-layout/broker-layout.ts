import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../core/i18n/i18n.service';
import { DashboardShell, ShellNavItem } from '../../shared/components/dashboard-shell/dashboard-shell';
import { UserMenuLink } from '../../shared/components/user-menu/user-menu';

/** Layout 4/4 — broker dashboard. */
@Component({
  selector: 'app-broker-layout',
  imports: [TranslatePipe, DashboardShell],
  templateUrl: './broker-layout.html',
  styleUrl: './broker-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerLayout {
  protected readonly i18n = inject(I18nService);

  protected readonly nav = computed<ShellNavItem[]>(() => {
    return [
      { label: this.i18n.instant('broker.nav.dashboard'), link: '/broker', icon: 'grid', exact: true },
      { label: this.i18n.instant('broker.nav.units'), link: '/broker/units', icon: 'building' },
      { label: this.i18n.instant('broker.nav.clients'), link: '/broker/clients', icon: 'users' },
      { label: this.i18n.instant('broker.nav.listings'), link: '/broker/listings', icon: 'home' },
      { label: this.i18n.instant('broker.nav.profile'), link: '/broker/profile', icon: 'user' }
    ];
  });

  protected readonly menuLinks = computed<UserMenuLink[]>(() => [
    { label: this.i18n.instant('account.profile'), link: '/broker/profile', icon: 'user' }
  ]);
}
