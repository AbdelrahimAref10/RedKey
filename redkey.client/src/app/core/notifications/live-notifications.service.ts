import { Injectable, computed, effect, inject, signal, untracked } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { ContactMessageListItemDto, NotificationDto, NotificationsClient } from '../api/redkey-api.generated';
import { AuthService } from '../auth/auth.service';
import { I18nService } from '../i18n/i18n.service';
import { timeAgo } from '../i18n/time-ago';
import type { NotificationItem } from '../../shared/components/notification-menu/notification-menu';

const HUB_URL = '/hubs/notifications';
/** Hub events (see the back-end). */
const NOTIFICATION_EVENT = 'notification';
const CONTACT_MESSAGE_EVENT = 'contactMessage';
const SOUND_URL = 'assets/sounds/notification.wav';

/** NotificationType values (back-end enum) → bell icon. */
const NOTIFICATION_ICONS: Readonly<Record<number, NotificationItem['icon']>> = {
  1: 'inbox', // unit submitted
  2: 'checkCircle', // unit approved
  3: 'edit', // changes requested
  4: 'close', // unit rejected
  5: 'calendar', // viewing requested
  6: 'calendar' // viewing request updated
};

/**
 * Notifications of the signed-in user (admin, broker and customer): the latest page from the API
 * + live pushes from the SignalR hub, with an alert sound on every new one. Nothing is static.
 */
@Injectable({ providedIn: 'root' })
export class LiveNotifications {
  private readonly client = inject(NotificationsClient);
  private readonly auth = inject(AuthService);
  private readonly i18n = inject(I18nService);
  private readonly router = inject(Router);

  private connection: HubConnection | null = null;
  private sound: HTMLAudioElement | null = null;

  readonly items = signal<readonly NotificationDto[]>([]);
  readonly unread = signal(0);
  /** The last notification pushed live (pages listen to refresh themselves, e.g. the requests table). */
  readonly received = signal<NotificationDto | null>(null);
  /** Super admins: the last website contact message pushed live (the chat icon listens to it). */
  readonly messageReceived = signal<ContactMessageListItemDto | null>(null);

  /** Shaped for the bell (relative time in the UI language). */
  readonly menuItems = computed<NotificationItem[]>(() => {
    const lang = this.i18n.lang();
    return this.items().map(n => ({
      id: n.id,
      title: n.title,
      body: n.body,
      time: timeAgo(n.createdAtUtc, lang),
      read: n.isRead,
      icon: NOTIFICATION_ICONS[n.type] ?? 'bell'
    }));
  });

  constructor() {
    // The texts come back in the requested language: reload after a language switch.
    effect(() => {
      this.i18n.lang();
      untracked(() => {
        if (this.connection) this.reload();
      });
    });
  }

  /** Called by the layouts that show a bell (idempotent). */
  start(): void {
    if (this.connection) return;
    this.reload();

    this.connection = new HubConnectionBuilder()
      .withUrl(HUB_URL, { accessTokenFactory: () => this.auth.accessToken() ?? '' })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.on(NOTIFICATION_EVENT, (notification: NotificationDto) => {
      // Show it right away, then reload: the pushed text uses the saved language, the list the UI one.
      this.items.update(list => [notification, ...list.filter(n => n.id !== notification.id)].slice(0, 30));
      this.unread.update(count => count + 1);
      this.received.set(notification);
      this.playSound();
      this.reload();
    });

    this.connection.on(CONTACT_MESSAGE_EVENT, (message: ContactMessageListItemDto) => {
      this.messageReceived.set(message);
      this.playSound();
    });

    this.connection.start().catch(() => {
      /* offline: withAutomaticReconnect only covers established connections; the list still loads via REST */
    });
  }

  stop(): void {
    if (this.connection && this.connection.state !== HubConnectionState.Disconnected) void this.connection.stop();
    this.connection = null;
    this.items.set([]);
    this.unread.set(0);
    this.received.set(null);
    this.messageReceived.set(null);
  }

  /** Latest page, in the UI language. */
  reload(): void {
    this.client.getMine(1, 30, this.i18n.lang()).subscribe({
      next: page => {
        this.items.set(page.page.items);
        this.unread.set(page.unreadCount);
      },
      error: () => {
        /* keep the current list */
      }
    });
  }

  /** Marks it read and goes to its link. */
  open(id: string): void {
    const notification = this.items().find(n => n.id === id);
    if (!notification) return;
    if (!notification.isRead) {
      this.items.update(list => list.map(n => (n.id === id ? { ...n, isRead: true } : n)));
      this.unread.update(count => Math.max(0, count - 1));
      this.client.markRead(id).subscribe({ error: () => undefined });
    }
    void this.router.navigateByUrl(notification.linkUrl);
  }

  markAllRead(): void {
    this.items.update(list => list.map(n => ({ ...n, isRead: true })));
    this.unread.set(0);
    this.client.markAllRead().subscribe({ error: () => undefined });
  }

  private playSound(): void {
    this.sound ??= new Audio(SOUND_URL);
    this.sound.currentTime = 0;
    // Browsers block audio until the user interacted with the page once: ignore that case.
    void this.sound.play().catch(() => undefined);
  }
}
