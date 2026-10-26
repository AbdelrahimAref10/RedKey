import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { AdminAccountingClient } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { JournalEntryCard } from '../../../shared/components/journal-entry-card/journal-entry-card';

/** Side drawer: every journal entry of one unit + what the company earned from it (super admin). */
@Component({
  selector: 'app-unit-journal-drawer',
  imports: [TranslatePipe, Icon, MoneyPipe, JournalEntryCard],
  templateUrl: './unit-journal-drawer.html',
  styleUrl: './unit-journal-drawer.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '(document:keydown.escape)': 'close.emit()' }
})
export class UnitJournalDrawer {
  private readonly client = inject(AdminAccountingClient);
  protected readonly i18n = inject(I18nService);

  readonly unitId = input.required<string>();
  readonly unitCode = input('');
  readonly close = output<void>();

  protected readonly journal = rxResource({
    params: () => ({ id: this.unitId(), lang: this.i18n.lang() }),
    stream: ({ params }) => this.client.getUnitJournal(params.id)
  });
}
