import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { STATUS_BADGE } from '../../../core/data/status';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { BROKER_LISTINGS } from '../broker.data';

@Component({
  selector: 'app-broker-listings',
  imports: [RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './broker-listings.html',
  styleUrl: './broker-listings.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerListings {
  protected readonly i18n = inject(I18nService);
  protected readonly listings = BROKER_LISTINGS;
  protected readonly badge = STATUS_BADGE;
}
