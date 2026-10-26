import { ChangeDetectionStrategy, Component, ElementRef, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ContactMessageListItemDto } from '../../../core/api/redkey-api.generated';
import { I18nService } from '../../../core/i18n/i18n.service';
import { timeAgo } from '../../../core/i18n/time-ago';
import { AdminInbox } from '../../../core/messages/admin-inbox.service';
import { Icon } from '../icon/icon';

/** Super admin header: chat icon with the unread count + the latest website messages. */
@Component({
  selector: 'app-messages-menu',
  imports: [TranslatePipe, RouterLink, Icon],
  templateUrl: './messages-menu.html',
  styleUrl: './messages-menu.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:click)': 'onDocumentClick($event)',
    '(document:keydown.escape)': 'open.set(false)'
  }
})
export class MessagesMenu {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly router = inject(Router);
  protected readonly inbox = inject(AdminInbox);
  protected readonly i18n = inject(I18nService);

  protected readonly open = signal(false);

  constructor() {
    this.inbox.refresh();
  }

  protected toggle(): void {
    this.open.update(v => !v);
  }

  protected select(message: ContactMessageListItemDto): void {
    this.open.set(false);
    void this.router.navigate(['/admin/messages', message.id]);
  }

  protected time(utc: string): string {
    return timeAgo(utc, this.i18n.lang());
  }

  protected initials(name: string): string {
    return name.split(' ').filter(Boolean).slice(0, 2).map(p => p[0]).join('').toUpperCase();
  }

  protected onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) this.open.set(false);
  }
}
