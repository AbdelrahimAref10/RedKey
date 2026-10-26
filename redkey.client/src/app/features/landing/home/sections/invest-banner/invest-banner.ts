import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-invest-banner',
  imports: [TranslatePipe, RouterLink, Icon],
  templateUrl: './invest-banner.html',
  styleUrl: './invest-banner.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvestBanner {
  protected readonly i18n = inject(I18nService);
}
