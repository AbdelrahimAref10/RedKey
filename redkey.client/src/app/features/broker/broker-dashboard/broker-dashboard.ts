import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { PROPERTIES } from '../../../core/data/properties.data';
import { STATUS_BADGE } from '../../../core/data/status';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { BROKER_CLIENTS, BROKER_CODE, BROKER_LISTINGS } from '../broker.data';

@Component({
  selector: 'app-broker-dashboard',
  imports: [RouterLink, Icon, LocPipe, MoneyPipe],
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
  protected readonly topUnits = [...PROPERTIES]
    .filter(p => p.purpose === 'sale')
    .sort((a, b) => b.commission - a.commission || b.price - a.price)
    .slice(0, 4);

  protected readonly kpis = {
    clients: BROKER_CLIENTS.filter(c => c.status !== 'sold').length,
    sales: BROKER_CLIENTS.filter(c => c.status === 'sold').length,
    commission: '162,500',
    listings: BROKER_LISTINGS.length
  };
}
