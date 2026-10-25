import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { STATUS_BADGE } from '../../../core/data/status';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { BROKER_CLIENTS } from '../broker.data';

@Component({
  selector: 'app-broker-clients',
  imports: [RouterLink, Icon, LocPipe],
  templateUrl: './broker-clients.html',
  styleUrl: './broker-clients.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerClients {
  protected readonly i18n = inject(I18nService);
  protected readonly clients = BROKER_CLIENTS;
  protected readonly badge = STATUS_BADGE;
}
