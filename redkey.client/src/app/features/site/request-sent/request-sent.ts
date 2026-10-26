import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-request-sent',
  imports: [TranslatePipe, RouterLink, Icon],
  templateUrl: './request-sent.html',
  styleUrl: './request-sent.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RequestSent {
  protected readonly i18n = inject(I18nService);
}
