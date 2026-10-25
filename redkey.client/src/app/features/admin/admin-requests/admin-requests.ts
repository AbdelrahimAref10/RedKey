import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { REQUESTS, RequestStatus, RequestType } from '../admin.data';
import { REQUEST_STATUS_BADGE, requestStatusLabel, requestTypeLabel } from '../request-status';

@Component({
  selector: 'app-admin-requests',
  imports: [Icon, LocPipe],
  templateUrl: './admin-requests.html',
  styleUrl: './admin-requests.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminRequests {
  protected readonly i18n = inject(I18nService);

  protected readonly tabs: readonly (RequestType | 'all')[] = ['all', 'visit', 'invest', 'contact'];
  protected readonly tab = signal<RequestType | 'all'>('all');
  /** Local status changes (front-end only). */
  private readonly statuses = signal<ReadonlyMap<number, RequestStatus>>(new Map());

  protected readonly rows = computed(() => {
    const tab = this.tab();
    const statuses = this.statuses();
    return REQUESTS.filter(r => tab === 'all' || r.type === tab).map(r => ({
      ...r,
      status: statuses.get(r.id) ?? r.status
    }));
  });

  protected readonly badge = REQUEST_STATUS_BADGE;
  protected readonly statusLabel = requestStatusLabel;
  protected readonly typeLabel = requestTypeLabel;

  protected count(tab: RequestType | 'all'): number {
    return tab === 'all' ? REQUESTS.length : REQUESTS.filter(r => r.type === tab).length;
  }

  protected next(id: number, current: RequestStatus): void {
    this.statuses.update(map => new Map(map).set(id, ((current + 1) % 3) as RequestStatus));
  }
}
