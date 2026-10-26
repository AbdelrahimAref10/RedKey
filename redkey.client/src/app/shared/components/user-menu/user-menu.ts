import { ChangeDetectionStrategy, Component, ElementRef, computed, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import type { IconName } from '../icon/icons';
import { Icon } from '../icon/icon';

export interface UserMenuLink {
  label: string;
  link: string;
  icon: IconName;
}

@Component({
  selector: 'app-user-menu',
  imports: [TranslatePipe, Icon, RouterLink],
  templateUrl: './user-menu.html',
  styleUrl: './user-menu.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:click)': 'onDocumentClick($event)',
    '(document:keydown.escape)': 'open.set(false)'
  }
})
export class UserMenu {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  readonly links = input.required<readonly UserMenuLink[]>();
  readonly roleLabel = input('');

  protected readonly open = signal(false);
  protected readonly user = this.auth.user;
  protected readonly firstName = computed(() => this.user()?.fullName.split(' ')[0] ?? '');
  protected readonly initials = computed(() =>
    (this.user()?.fullName ?? '?')
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0])
      .join('')
      .toUpperCase()
  );

  protected toggle(): void {
    this.open.update(v => !v);
  }

  protected logout(): void {
    this.open.set(false);
    this.auth.logout();
  }

  protected onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) this.open.set(false);
  }
}
