import { Injectable, effect, inject, signal, untracked } from '@angular/core';
import { AdminContactMessagesClient, ContactMessageListItemDto } from '../api/redkey-api.generated';
import { LiveNotifications } from '../notifications/live-notifications.service';

const LATEST = 8;

/**
 * Super admin inbox state shared by the header chat icon, the sidebar badge, the dashboard and the
 * messages page: latest messages + unread count, updated live when a message arrives.
 */
@Injectable({ providedIn: 'root' })
export class AdminInbox {
  private readonly client = inject(AdminContactMessagesClient);
  private readonly live = inject(LiveNotifications);

  readonly latest = signal<readonly ContactMessageListItemDto[]>([]);
  readonly unread = signal(0);
  readonly total = signal(0);
  /** Bumped on every live message: pages reload their own list. */
  readonly version = signal(0);

  constructor() {
    effect(() => {
      const message = this.live.messageReceived();
      if (!message) return;
      untracked(() => {
        this.latest.update(list => [message, ...list.filter(m => m.id !== message.id)].slice(0, LATEST));
        this.unread.update(n => n + 1);
        this.total.update(n => n + 1);
        this.version.update(v => v + 1);
      });
    });
  }

  refresh(): void {
    this.client.getMessages(null, null, 1, LATEST).subscribe({
      next: result => {
        this.latest.set(result.page.items);
        this.unread.set(result.unreadCount);
        this.total.set(result.page.totalCount);
      },
      error: () => undefined
    });
  }

  /** After opening a message (the API marks it read). */
  markedRead(id: string): void {
    const wasUnread = this.latest().some(m => m.id === id && !m.isRead);
    this.latest.update(list => list.map(m => (m.id === id ? { ...m, isRead: true } : m)));
    if (wasUnread) this.unread.update(n => Math.max(0, n - 1));
    else this.refresh();
  }

  removed(id: string): void {
    this.latest.update(list => list.filter(m => m.id !== id));
    this.refresh();
  }
}
