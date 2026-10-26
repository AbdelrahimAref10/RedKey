import { ChangeDetectionStrategy, Component, ElementRef, inject, input, output, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import type { IconName } from '../icon/icons';
import { Icon } from '../icon/icon';

export interface NotificationItem {
  id: string;
  title: string;
  body: string;
  time: string;
  read: boolean;
  icon: IconName;
}

/** Bell + dropdown. The owner decides where an item leads (`opened`) and how "read" is stored. */
@Component({
  selector: 'app-notification-menu',
  imports: [TranslatePipe, Icon],
  templateUrl: './notification-menu.html',
  styleUrl: './notification-menu.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:click)': 'onDocumentClick($event)',
    '(document:keydown.escape)': 'open.set(false)'
  }
})
export class NotificationMenu {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  protected readonly i18n = inject(I18nService);

  readonly items = input.required<readonly NotificationItem[]>();
  readonly unread = input.required<number>();
  readonly opened = output<NotificationItem>();
  readonly markAllRead = output<void>();

  protected readonly open = signal(false);

  protected toggle(): void {
    this.open.update(v => !v);
  }

  protected select(item: NotificationItem): void {
    this.open.set(false);
    this.opened.emit(item);
  }

  protected onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) this.open.set(false);
  }
}
