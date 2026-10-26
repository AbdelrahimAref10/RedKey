import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../icon/icon';

/** Page numbers with « prev / next », compacted with "…" for long lists. */
@Component({
  selector: 'app-pagination',
  imports: [TranslatePipe, Icon],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Pagination {
  protected readonly i18n = inject(I18nService);

  readonly page = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly pageChange = output<number>();

  /** e.g. 1 … 4 5 [6] 7 8 … 20 */
  protected readonly pages = computed<(number | null)[]>(() => {
    const total = this.totalPages();
    const current = this.page();
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);

    const pages: (number | null)[] = [1];
    const from = Math.max(2, current - 2);
    const to = Math.min(total - 1, current + 2);
    if (from > 2) pages.push(null);
    for (let p = from; p <= to; p++) pages.push(p);
    if (to < total - 1) pages.push(null);
    pages.push(total);
    return pages;
  });

  protected go(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.page()) this.pageChange.emit(page);
  }
}
