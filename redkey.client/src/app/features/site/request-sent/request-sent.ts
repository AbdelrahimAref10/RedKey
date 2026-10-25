import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-request-sent',
  imports: [RouterLink, Icon],
  templateUrl: './request-sent.html',
  styleUrl: './request-sent.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RequestSent {
  protected readonly i18n = inject(I18nService);
}
