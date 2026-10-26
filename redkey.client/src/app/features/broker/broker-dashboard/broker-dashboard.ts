import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { BrokerUnitsClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { STATUS_BADGE } from '../../../core/data/status';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { BROKER_CLIENTS, BROKER_CODE } from '../broker.data';

@Component({
  selector: 'app-broker-dashboard',
  imports: [TranslatePipe, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './broker-dashboard.html',
  styleUrl: './broker-dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerDashboard {
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  protected readonly code = BROKER_CODE;
  protected readonly badge = STATUS_BADGE;
  protected readonly firstName = computed(() => this.auth.user()?.fullName.split(' ')[0] ?? '');
  protected readonly clients = BROKER_CLIENTS.slice(0, 4);
  private readonly client = inject(BrokerUnitsClient);

  /** Latest published units with their commission. */
  private readonly catalog = rxResource({
    params: () => this.auth.language(),
    stream: () => this.client.getCatalog(null, null, null, null, null, null, null, 1, 4, null)
  });
  protected readonly topUnits = computed(() => this.catalog.value()?.items ?? []);

  /** Total of the broker's own units (every status). */
  private readonly mine = rxResource({
    params: () => this.auth.language(),
    stream: () => this.client.getMyUnits(null, null, null, null, null, null, null, null, 1, 1, null)
  });
  protected readonly listingsCount = computed(() => this.mine.value()?.totalCount ?? 0);

  protected readonly kpis = {
    clients: BROKER_CLIENTS.filter(c => c.status !== 'sold').length,
    sales: BROKER_CLIENTS.filter(c => c.status === 'sold').length,
    commission: '162,500'
  };
}
