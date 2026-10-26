import { Injectable, effect, inject, signal, untracked } from '@angular/core';
import { AdminViewingRequestsClient, ViewingRequestsSummaryDto } from '../api/redkey-api.generated';
import { LiveNotifications } from '../notifications/live-notifications.service';

/** NotificationType.ViewingRequested (back-end enum). */
export const VIEWING_REQUESTED = 5;

/**
 * Super admin: viewing requests count per status (sidebar badge, dashboard, requests tabs).
 * Refreshes itself when a new request arrives live.
 */
@Injectable({ providedIn: 'root' })
export class ViewingRequestsSummary {
  private readonly client = inject(AdminViewingRequestsClient);
  private readonly live = inject(LiveNotifications);

  readonly value = signal<ViewingRequestsSummaryDto | null>(null);

  constructor() {
    effect(() => {
      if (this.live.received()?.type === VIEWING_REQUESTED) untracked(() => this.refresh());
    });
  }

  refresh(): void {
    this.client.getSummary().subscribe({
      next: summary => this.value.set(summary),
      error: () => undefined
    });
  }
}
