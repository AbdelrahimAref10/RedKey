import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-language-switcher',
  imports: [TranslatePipe, Icon],
  templateUrl: './language-switcher.html',
  styleUrl: './language-switcher.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class.compact]': 'compact()', '[class.on-dark]': 'onDark()' }
})
export class LanguageSwitcher {
  protected readonly i18n = inject(I18nService);
  private readonly auth = inject(AuthService);

  readonly compact = input(false);
  readonly onDark = input(false);

  protected toggle(): void {
    this.i18n.toggle();
    // Admin / broker: the API language lives in the session token.
    this.auth.syncLanguage(this.i18n.lang());
  }
}
