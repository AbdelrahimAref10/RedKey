import { ChangeDetectionStrategy, Component, ElementRef, effect, inject, input, linkedSignal, output, viewChild } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { of } from 'rxjs';
import { AdminAccountingClient } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../icon/icon';

/**
 * "Where did the money go?" — the super admin picks the cash / bank account that received a payment
 * (viewing fee now, commissions later). Emits the account id; the caller posts it.
 */
@Component({
  selector: 'app-receive-payment-dialog',
  imports: [TranslatePipe, Icon, MoneyPipe],
  templateUrl: './receive-payment-dialog.html',
  styleUrl: './receive-payment-dialog.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReceivePaymentDialog {
  private readonly client = inject(AdminAccountingClient);
  protected readonly i18n = inject(I18nService);

  readonly open = input(false);
  readonly amount = input(0);
  /** What the money is for, e.g. "RK-1006 · Mona". */
  readonly description = input('');
  readonly busy = input(false);

  readonly confirmed = output<number>();
  readonly cancelled = output<void>();

  private readonly dialog = viewChild.required<ElementRef<HTMLDialogElement>>('dialog');

  protected readonly accounts = rxResource({
    params: () => (this.open() ? { lang: this.i18n.lang() } : undefined),
    stream: ({ params }) => (params ? this.client.getCashAccounts() : of([]))
  });

  /** First account (the cash box) picked by default. */
  protected readonly selected = linkedSignal(() => this.accounts.value()?.[0]?.id ?? null);

  constructor() {
    effect(() => {
      const dialog = this.dialog().nativeElement;
      if (this.open() && !dialog.open) dialog.showModal();
      else if (!this.open() && dialog.open) dialog.close();
    });
  }

  protected confirm(): void {
    const id = this.selected();
    if (id !== null) this.confirmed.emit(id);
  }

  protected backdropClick(event: MouseEvent): void {
    if (event.target === this.dialog().nativeElement) this.cancelled.emit();
  }
}
