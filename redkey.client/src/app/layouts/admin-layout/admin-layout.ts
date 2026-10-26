import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../core/i18n/i18n.service';
import { AdminInbox } from '../../core/messages/admin-inbox.service';
import { ViewingRequestsSummary } from '../../core/requests/viewing-requests-summary.service';
import { DashboardShell, ShellNavItem } from '../../shared/components/dashboard-shell/dashboard-shell';
import { UserMenuLink } from '../../shared/components/user-menu/user-menu';

/** Layout 3/4 — super admin dashboard (prototype: "لوحة الأدمن"). */
@Component({
  selector: 'app-admin-layout',
  imports: [TranslatePipe, DashboardShell],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminLayout {
  protected readonly i18n = inject(I18nService);
  private readonly requests = inject(ViewingRequestsSummary);
  private readonly inbox = inject(AdminInbox);

  constructor() {
    this.requests.refresh();
  }

  protected readonly nav = computed<ShellNavItem[]>(() => {
    // New viewing requests (live: refreshed when one arrives or a status changes).
    const newRequests = this.requests.value()?.new || undefined;
    const unreadMessages = this.inbox.unread() || undefined;
    return [
      { label: this.i18n.instant('admin.nav.dashboard'), link: '/admin', icon: 'grid', exact: true },
      { label: this.i18n.instant('admin.nav.units'), link: '/admin/units', icon: 'building' },
      { label: this.i18n.instant('admin.nav.unitTypes'), link: '/admin/unit-types', icon: 'layers' },
      { label: this.i18n.instant('admin.nav.cities'), link: '/admin/cities', icon: 'pin' },
      { label: this.i18n.instant('admin.nav.requests'), link: '/admin/requests', icon: 'inbox', badge: newRequests },
      { label: this.i18n.instant('admin.nav.accounting'), link: '/admin/accounting', icon: 'wallet' },
      { label: this.i18n.instant('admin.nav.messages'), link: '/admin/messages', icon: 'message', badge: unreadMessages },
      { label: this.i18n.instant('admin.nav.opportunities'), link: '/admin/opportunities', icon: 'trending' },
      { label: this.i18n.instant('admin.nav.brokers'), link: '/admin/brokers', icon: 'users', badge: 3 },
      { label: this.i18n.instant('admin.nav.favorites'), link: '/admin/favorites', icon: 'heart' },
      { label: this.i18n.instant('admin.nav.profile'), link: '/admin/profile', icon: 'user' }
    ];
  });

  protected readonly menuLinks = computed<UserMenuLink[]>(() => [
    { label: this.i18n.instant('account.profile'), link: '/admin/profile', icon: 'user' }
  ]);
}
