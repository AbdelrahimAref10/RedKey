import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-language-switcher',
  imports: [Icon],
  templateUrl: './language-switcher.html',
  styleUrl: './language-switcher.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class.compact]': 'compact()', '[class.on-dark]': 'onDark()' }
})
export class LanguageSwitcher {
  protected readonly i18n = inject(I18nService);

  readonly compact = input(false);
  readonly onDark = input(false);
}
