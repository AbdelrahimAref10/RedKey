import { ChangeDetectionStrategy, Component, computed, effect, inject, untracked } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import { AdminUnitsClient, AdminViewingRequestsClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { timeAgo } from '../../../core/i18n/time-ago';
import { AdminInbox } from '../../../core/messages/admin-inbox.service';
import { LiveNotifications } from '../../../core/notifications/live-notifications.service';
import { VIEWING_REQUESTED, ViewingRequestsSummary } from '../../../core/requests/viewing-requests-summary.service';
import { Icon } from '../../../shared/components/icon/icon';
import { JOIN_REQUESTS } from '../admin.data';
import { REQUEST_STATUS_BADGE } from '../request-status';

/** UnitStatus values (back-end enum). */
const PENDING_REVIEW = 1;
const APPROVED = 2;

@Component({
  selector: 'app-admin-dashboard',
  imports: [TranslatePipe, RouterLink, Icon, LocPipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminDashboard {
  private readonly auth = inject(AuthService);
  private readonly units = inject(AdminUnitsClient);
  private readonly requests = inject(AdminViewingRequestsClient);
  private readonly live = inject(LiveNotifications);
  protected readonly summary = inject(ViewingRequestsSummary);
  protected readonly inbox = inject(AdminInbox);
  protected readonly i18n = inject(I18nService);

  protected readonly firstName = computed(() => this.auth.user()?.fullName.split(' ')[0] ?? '');
  protected readonly badge = REQUEST_STATUS_BADGE;
  /** Broker join requests are not built in the back-end yet. */
  protected readonly pending = JOIN_REQUESTS;

  /** Published units + units waiting for review (only the counts: page size 1). */
  protected readonly unitCounts = rxResource({
    stream: () =>
      forkJoin({
        published: this.units.getUnits(APPROVED, null, null, null, null, null, null, null, 1, 1, null),
        pending: this.units.getUnits(PENDING_REVIEW, null, null, null, null, null, null, null, 1, 1, null)
      })
  });

  /** The 5 latest viewing requests. */
  protected readonly latest = rxResource({
    params: () => this.i18n.lang(),
    stream: () => this.requests.getRequests(null, null, 1, 5)
  });

  constructor() {
    this.summary.refresh();
    effect(() => {
      if (this.live.received()?.type === VIEWING_REQUESTED) untracked(() => this.latest.reload());
    });
  }

  protected time(utc: string): string {
    return timeAgo(utc, this.i18n.lang());
  }
}
