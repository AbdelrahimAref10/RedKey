import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, input, linkedSignal, signal, untracked } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { of } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import { AdminContactMessagesClient, ContactMessageListItemDto } from '../../../core/api/redkey-api.generated';
import { flagUrl } from '../../../core/data/countries';
import { I18nService } from '../../../core/i18n/i18n.service';
import { timeAgo } from '../../../core/i18n/time-ago';
import { AdminInbox } from '../../../core/messages/admin-inbox.service';
import { ConfirmService } from '../../../core/ui/confirm.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';

/** Website contact messages: inbox on one side, the opened message on the other (mail client layout). */
@Component({
  selector: 'app-admin-messages',
  imports: [TranslatePipe, RouterLink, Icon, Pagination],
  templateUrl: './admin-messages.html',
  styleUrl: './admin-messages.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminMessages {
  private readonly client = inject(AdminContactMessagesClient);
  private readonly inbox = inject(AdminInbox);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly confirm = inject(ConfirmService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly i18n = inject(I18nService);

  /** Route param: the opened message. */
  readonly id = input<string>();

  protected readonly unreadOnly = signal(false);
  protected readonly page = signal(1);
  protected readonly search = signal('');
  private searchTimer: ReturnType<typeof setTimeout> | undefined;
  protected readonly copied = signal(false);
  protected readonly flagUrl = flagUrl;
  protected readonly unreadCount = this.inbox.unread;

  private readonly list = rxResource({
    params: () => ({
      unread: this.unreadOnly(),
      page: this.page(),
      search: this.search(),
      lang: this.i18n.lang(),
      live: this.inbox.version()
    }),
    stream: ({ params }) => this.client.getMessages(params.unread || null, params.search || null, params.page, null)
  });

  /** Local copy, so opening a message turns it "read" in the list at once. */
  protected readonly rows = linkedSignal(() => this.list.value()?.page.items ?? []);
  protected readonly pageInfo = computed(() => this.list.value()?.page);
  protected readonly listLoading = computed(() => this.list.isLoading());

  /** The opened message (opening it marks it read on the server). */
  protected readonly message = rxResource({
    params: () => {
      const id = this.id();
      return id ? { id, lang: this.i18n.lang() } : undefined;
    },
    stream: ({ params }) => (params ? this.client.open(params.id) : of(undefined))
  });

  protected readonly whatsappLink = computed(() => {
    const m = this.message.value();
    if (!m) return '';
    const text = this.i18n.instant('messages.whatsappGreeting', { name: m.name.split(' ')[0], subject: m.subjectName });
    return `https://wa.me/${m.dialCode}${m.phoneNumber}?text=${encodeURIComponent(text)}`;
  });

  protected readonly receivedAt = computed(() => {
    const m = this.message.value();
    if (!m) return '';
    const date = new Date(m.createdAtUtc.endsWith('Z') ? m.createdAtUtc : `${m.createdAtUtc}Z`);
    return new Intl.DateTimeFormat(this.i18n.lang() === 'ar' ? 'ar-EG' : 'en-GB', { dateStyle: 'full', timeStyle: 'short' }).format(date);
  });

  constructor() {
    // Opened → read, in the list and in the header badge.
    effect(() => {
      const opened = this.message.value();
      if (!opened) return;
      untracked(() => {
        const row = this.rows().find(r => r.id === opened.id);
        if (row && !row.isRead) this.rows.update(rows => rows.map(r => (r.id === opened.id ? { ...r, isRead: true } : r)));
        this.inbox.markedRead(opened.id);
      });
    });

    this.destroyRef.onDestroy(() => clearTimeout(this.searchTimer));
  }

  protected setUnreadOnly(value: boolean): void {
    this.unreadOnly.set(value);
    this.page.set(1);
  }

  protected onSearch(value: string): void {
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => {
      this.search.set(value.trim());
      this.page.set(1);
    }, 350);
  }

  protected time(row: ContactMessageListItemDto): string {
    return timeAgo(row.createdAtUtc, this.i18n.lang());
  }

  protected initials(name: string): string {
    return name.split(' ').filter(Boolean).slice(0, 2).map(p => p[0]).join('').toUpperCase();
  }

  protected phone(m: { dialCode: string; phoneNumber: string }): string {
    return `+${m.dialCode} ${m.phoneNumber}`;
  }

  protected countryName(iso: string): string {
    return new Intl.DisplayNames([this.i18n.lang()], { type: 'region' }).of(iso) ?? iso;
  }

  protected async copyPhone(m: { dialCode: string; phoneNumber: string }): Promise<void> {
    try {
      await navigator.clipboard.writeText(`+${m.dialCode}${m.phoneNumber}`);
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), 1800);
    } catch {
      /* clipboard blocked */
    }
  }

  protected async remove(id: string): Promise<void> {
    const confirmed = await this.confirm.ask({
      title: this.i18n.instant('messages.deleteTitle'),
      message: this.i18n.instant('messages.deleteConfirm'),
      confirmText: this.i18n.instant('lookupsAdmin.delete'),
      tone: 'danger'
    });
    if (!confirmed) return;

    this.client.delete(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.toast.show(this.i18n.instant('messages.deleted'));
        this.inbox.removed(id);
        this.list.reload();
        void this.router.navigate(['/admin/messages']);
      },
      error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
    });
  }
}
