import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, input, linkedSignal, signal, untracked } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { apiErrorCode } from '../../../core/api/api-error';
import { AdminViewingRequestsClient, LandingLookupsClient, ViewingRequestAdminDto } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { timeAgo } from '../../../core/i18n/time-ago';
import { LiveNotifications } from '../../../core/notifications/live-notifications.service';
import { VIEWING_REQUESTED, ViewingRequestsSummary } from '../../../core/requests/viewing-requests-summary.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { ReceivePaymentDialog } from '../../../shared/components/receive-payment-dialog/receive-payment-dialog';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { REQUEST_STATUS_BADGE } from '../request-status';

/** ViewingRequestStatus.Completed (back-end enum): posts the fee. */
const VIEWING_DONE = 4;

/** Status menu size (px), to place it before it renders. */
const MENU_WIDTH = 190;
const MENU_HEIGHT = 180;

/** Viewing requests from the website: filter by status, change the status in place (saved right away). */
@Component({
  selector: 'app-admin-requests',
  imports: [TranslatePipe, RouterLink, Icon, Pagination, ReceivePaymentDialog, MoneyPipe],
  templateUrl: './admin-requests.html',
  styleUrl: './admin-requests.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:click)': 'closeMenu()',
    '(window:resize)': 'closeMenu()',
    '(document:keydown.escape)': 'closeMenu()'
  }
})
export class AdminRequests {
  private readonly client = inject(AdminViewingRequestsClient);
  private readonly lookupsClient = inject(LandingLookupsClient);
  private readonly toast = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly live = inject(LiveNotifications);
  protected readonly summary = inject(ViewingRequestsSummary);
  protected readonly i18n = inject(I18nService);

  /** ?highlight=<id> (the notification link): that row is highlighted. */
  readonly highlight = input<string>();

  protected readonly status = signal<number | null>(null);
  protected readonly page = signal(1);
  protected readonly search = signal('');
  private searchTimer: ReturnType<typeof setTimeout> | undefined;

  /** The row whose status menu is open, and where the menu floats (fixed, above the table). */
  protected readonly menuFor = signal<string | null>(null);
  protected readonly menuPos = signal<{ top: number; left: number; up: boolean } | null>(null);
  protected readonly badge = REQUEST_STATUS_BADGE;

  /** Status names from the back-end (tabs + menu). */
  private readonly lookups = rxResource({
    params: () => this.i18n.lang(),
    stream: ({ params }) => this.lookupsClient.getLookups(params)
  });
  protected readonly statuses = computed(() => this.lookups.value()?.viewingRequestStatuses ?? []);

  private readonly list = rxResource({
    params: () => ({ status: this.status(), page: this.page(), search: this.search(), lang: this.i18n.lang() }),
    stream: ({ params }) => this.client.getRequests(params.status, params.search || null, params.page, null)
  });

  /** Local copy of the rows so a status change shows instantly. */
  protected readonly rows = linkedSignal(() => this.list.value()?.items ?? []);
  protected readonly result = computed(() => this.list.value());
  protected readonly loading = computed(() => this.list.isLoading());

  constructor() {
    this.summary.refresh();

    // A new request arrived live: show it.
    effect(() => {
      if (this.live.received()?.type === VIEWING_REQUESTED) untracked(() => this.list.reload());
    });

    // Opened from a notification: bring the row into view once it is rendered.
    effect(() => {
      const id = this.highlight();
      if (!id || !this.rows().some(r => r.id === id)) return;
      setTimeout(() => document.getElementById(`request-${id}`)?.scrollIntoView({ block: 'center', behavior: 'smooth' }));
    });

    // The dashboard scrolls inside its own container: any scroll (capture) closes the floating menu.
    const onScroll = () => this.menuFor() && this.closeMenu();
    document.addEventListener('scroll', onScroll, true);
    this.destroyRef.onDestroy(() => {
      clearTimeout(this.searchTimer);
      document.removeEventListener('scroll', onScroll, true);
    });
  }

  protected count(status: number | null): number | undefined {
    const s = this.summary.value();
    if (!s) return undefined;
    switch (status) {
      case null: return s.total;
      case 1: return s.new;
      case 2: return s.contacted;
      case 3: return s.closed;
      default: return s.completed;
    }
  }

  protected setStatusFilter(status: number | null): void {
    this.status.set(status);
    this.page.set(1);
  }

  protected onSearch(value: string): void {
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => {
      this.search.set(value.trim());
      this.page.set(1);
    }, 350);
  }

  protected time(row: ViewingRequestAdminDto): string {
    return timeAgo(row.createdAtUtc, this.i18n.lang());
  }

  protected toggleMenu(row: ViewingRequestAdminDto, event: Event): void {
    event.stopPropagation();
    if (this.menuFor() === row.id) {
      this.closeMenu();
      return;
    }

    // Floats over the page (not clipped / scrolled by the table): under the badge, or above it near the bottom.
    const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
    const rtl = this.i18n.isRtl();
    const up = window.innerHeight - rect.bottom < MENU_HEIGHT + 16;
    const left = rtl ? rect.left : rect.right - MENU_WIDTH;
    this.menuPos.set({
      top: up ? rect.top - MENU_HEIGHT - 6 : rect.bottom + 6,
      left: Math.max(8, Math.min(left, window.innerWidth - MENU_WIDTH - 8)),
      up
    });
    this.menuFor.set(row.id);
  }

  protected closeMenu(): void {
    this.menuFor.set(null);
    this.menuPos.set(null);
  }

  protected readonly menuRow = computed(() => this.rows().find(r => r.id === this.menuFor()) ?? null);

  /** "Viewing done" with a fee waits in this dialog for the account that received the money. */
  protected readonly receiving = signal<{ row: ViewingRequestAdminDto; value: number; name: string } | null>(null);
  protected readonly posting = signal(false);

  /** Optimistic: the row changes at once, then the API saves it (rolled back on error). */
  protected setStatus(row: ViewingRequestAdminDto, value: number, name: string): void {
    this.closeMenu();
    if (row.status === value) return;

    // Viewing done + a fee → ask where the money went first (it is posted to that account).
    if (value === VIEWING_DONE && row.viewingFee > 0) {
      this.receiving.set({ row, value, name });
      return;
    }
    this.save(row, value, name);
  }

  protected onReceived(cashAccountId: number): void {
    const pending = this.receiving();
    if (!pending) return;
    this.save(pending.row, pending.value, pending.name, cashAccountId);
  }

  private save(row: ViewingRequestAdminDto, value: number, name: string, cashAccountId?: number): void {
    const previous = { status: row.status, statusName: row.statusName };
    this.patchRow(row.id, { status: value, statusName: name });

    this.posting.set(true);
    this.client
      .updateStatus(row.id, { status: value, cashAccountId })
      .pipe(
        finalize(() => {
          this.posting.set(false);
          this.receiving.set(null);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          if (cashAccountId !== undefined) this.toast.show(this.i18n.instant('accounting.receive.posted'));
          this.summary.refresh();
          // Filtered on the old status: the row no longer belongs to this tab.
          if (this.status() !== null) this.list.reload();
        },
        error: error => {
          this.patchRow(row.id, previous);
          this.toast.show(this.i18n.error(apiErrorCode(error)), 'error');
        }
      });
  }

  private patchRow(id: string, patch: Partial<ViewingRequestAdminDto>): void {
    this.rows.update(rows => rows.map(r => (r.id === id ? ({ ...r, ...patch } as ViewingRequestAdminDto) : r)));
  }
}
