import { ChangeDetectionStrategy, Component, ElementRef, computed, inject, input, linkedSignal, signal } from '@angular/core';
import { AppNotification } from '../../../core/data/models';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-notification-menu',
  imports: [Icon, LocPipe],
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

  readonly items = input.required<readonly AppNotification[]>();

  protected readonly open = signal(false);
  /** Local read state (front-end only). */
  protected readonly list = linkedSignal(() => this.items());
  protected readonly unread = computed(() => this.list().filter(n => !n.read).length);

  protected toggle(): void {
    this.open.update(v => !v);
  }

  protected markAllRead(): void {
    this.list.update(list => list.map(n => ({ ...n, read: true })));
  }

  protected onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) this.open.set(false);
  }
}
