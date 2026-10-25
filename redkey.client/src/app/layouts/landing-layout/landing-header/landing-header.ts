import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';
import { LanguageSwitcher } from '../../../shared/components/language-switcher/language-switcher';
import { Logo } from '../../../shared/components/logo/logo';

@Component({
  selector: 'app-landing-header',
  imports: [RouterLink, RouterLinkActive, Icon, Logo, LanguageSwitcher],
  templateUrl: './landing-header.html',
  styleUrl: './landing-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LandingHeader {
  protected readonly i18n = inject(I18nService);
  protected readonly auth = inject(AuthService);

  protected readonly menuOpen = signal(false);

  protected readonly links = [
    { key: 'home', path: '/', exact: true },
    { key: 'properties', path: '/properties', exact: false },
    { key: 'invest', path: '/invest', exact: false },
    { key: 'contact', path: '/contact', exact: false }
  ] as const;
}
