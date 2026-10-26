import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { AdminAccountingClient } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { JournalEntryCard } from '../../../shared/components/journal-entry-card/journal-entry-card';
import { Pagination } from '../../../shared/components/pagination/pagination';

type Tab = 'treasury' | 'journal';

/** The company books: treasury balances, this month's profit, cash movements and the journal. */
@Component({
  selector: 'app-admin-accounting',
  imports: [TranslatePipe, Icon, MoneyPipe, Pagination, JournalEntryCard],
  templateUrl: './admin-accounting.html',
  styleUrl: './admin-accounting.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminAccounting {
  private readonly client = inject(AdminAccountingClient);
  protected readonly i18n = inject(I18nService);

  protected readonly tab = signal<Tab>('treasury');
  protected readonly cashAccount = signal<number | null>(null);
  protected readonly treasuryPage = signal(1);
  protected readonly journalPage = signal(1);

  protected readonly overview = rxResource({
    params: () => this.i18n.lang(),
    stream: () => this.client.getOverview(null, null)
  });

  protected readonly treasury = rxResource({
    params: () => (this.tab() === 'treasury'
      ? { account: this.cashAccount(), page: this.treasuryPage(), lang: this.i18n.lang() }
      : undefined),
    stream: ({ params }) => this.client.getTreasury(params!.account, params!.page, null)
  });

  protected readonly journal = rxResource({
    params: () => (this.tab() === 'journal' ? { page: this.journalPage(), lang: this.i18n.lang() } : undefined),
    stream: ({ params }) => this.client.getJournal(null, params!.page, null)
  });

  protected setAccount(id: number | null): void {
    this.cashAccount.set(id);
    this.treasuryPage.set(1);
  }

  protected date(utc: string): string {
    const d = new Date(utc.endsWith('Z') ? utc : `${utc}Z`);
    return new Intl.DateTimeFormat(this.i18n.lang() === 'ar' ? 'ar-EG' : 'en-GB', { dateStyle: 'medium', timeStyle: 'short' }).format(d);
  }
}
