import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { ADMIN_NOTIFICATIONS } from '../../core/data/site.data';
import { I18nService } from '../../core/i18n/i18n.service';
import { DashboardShell, ShellNavItem } from '../../shared/components/dashboard-shell/dashboard-shell';
import { UserMenuLink } from '../../shared/components/user-menu/user-menu';

/** Layout 3/4 — super admin dashboard (prototype: "لوحة الأدمن"). */
@Component({
  selector: 'app-admin-layout',
  imports: [DashboardShell],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminLayout {
  protected readonly i18n = inject(I18nService);
  protected readonly notifications = ADMIN_NOTIFICATIONS;

  protected readonly nav = computed<ShellNavItem[]>(() => {
    const n = this.i18n.t().admin.nav;
    return [
      { label: n.dashboard, link: '/admin', icon: 'grid', exact: true },
      { label: n.units, link: '/admin/units', icon: 'building' },
      { label: n.requests, link: '/admin/requests', icon: 'inbox', badge: 12 },
      { label: n.opportunities, link: '/admin/opportunities', icon: 'trending' },
      { label: n.brokers, link: '/admin/brokers', icon: 'users', badge: 3 },
      { label: n.favorites, link: '/admin/favorites', icon: 'heart' },
      { label: n.profile, link: '/admin/profile', icon: 'user' }
    ];
  });

  protected readonly menuLinks = computed<UserMenuLink[]>(() => [
    { label: this.i18n.t().account.profile, link: '/admin/profile', icon: 'user' }
  ]);
}
