import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { JournalEntryDto } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../icon/icon';

const amount = new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

/** One journal entry as accountants read it: header + debit / credit lines + totals. */
@Component({
  selector: 'app-journal-entry-card',
  imports: [TranslatePipe, Icon],
  templateUrl: './journal-entry-card.html',
  styleUrl: './journal-entry-card.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class JournalEntryCard {
  protected readonly i18n = inject(I18nService);

  readonly entry = input.required<JournalEntryDto>();
  /** Shows the unit code (journal page); hidden inside the unit drawer. */
  readonly showUnit = input(true);

  protected readonly date = computed(() => {
    const utc = this.entry().entryDateUtc;
    const d = new Date(utc.endsWith('Z') ? utc : `${utc}Z`);
    return new Intl.DateTimeFormat(this.i18n.lang() === 'ar' ? 'ar-EG' : 'en-GB', { dateStyle: 'medium', timeStyle: 'short' }).format(d);
  });

  protected format(value: number): string {
    return value ? amount.format(value) : '';
  }
}
