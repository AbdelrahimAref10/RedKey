import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-broker-cta',
  imports: [RouterLink, Icon],
  templateUrl: './broker-cta.html',
  styleUrl: './broker-cta.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerCta {
  protected readonly i18n = inject(I18nService);
}
