import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { AppNotification } from '../../../core/data/models';
import { I18nService } from '../../../core/i18n/i18n.service';
import type { IconName } from '../icon/icons';
import { Icon } from '../icon/icon';
import { LanguageSwitcher } from '../language-switcher/language-switcher';
import { Logo } from '../logo/logo';
import { NotificationMenu } from '../notification-menu/notification-menu';
import { UserMenu, UserMenuLink } from '../user-menu/user-menu';

export interface ShellNavItem {
  label: string;
  link: string;
  icon: IconName;
  exact?: boolean;
  badge?: number;
}

/** Sidebar + top bar used by the admin and broker layouts. */
@Component({
  selector: 'app-dashboard-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, Icon, Logo, LanguageSwitcher, NotificationMenu, UserMenu],
  templateUrl: './dashboard-shell.html',
  styleUrl: './dashboard-shell.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardShell {
  protected readonly i18n = inject(I18nService);
  private readonly auth = inject(AuthService);

  readonly brandSuffix = input.required<string>();
  readonly homeLink = input.required<string>();
  readonly nav = input.required<readonly ShellNavItem[]>();
  readonly notifications = input.required<readonly AppNotification[]>();
  readonly menuLinks = input.required<readonly UserMenuLink[]>();
  readonly roleLabel = input('');

  protected readonly sidebarOpen = signal(false);

  constructor() {
    inject(Router)
      .events.pipe(
        filter(e => e instanceof NavigationEnd),
        takeUntilDestroyed(inject(DestroyRef))
      )
      .subscribe(() => this.sidebarOpen.set(false));
  }

  protected logout(): void {
    this.auth.logout();
  }
}
