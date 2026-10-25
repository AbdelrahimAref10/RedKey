import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PropertyType } from '../../../core/data/models';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../icon/icon';
import { Logo } from '../logo/logo';

@Component({
  selector: 'app-site-footer',
  imports: [RouterLink, Icon, Logo],
  templateUrl: './site-footer.html',
  styleUrl: './site-footer.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SiteFooter {
  protected readonly i18n = inject(I18nService);
  private readonly toast = inject(ToastService);

  /** Route prefix of the current layout: '' (landing) or '/customer'. */
  readonly base = input('');

  protected readonly year = new Date().getFullYear();
  protected readonly types: readonly PropertyType[] = ['apartment', 'villa', 'townhouse', 'chalet'];
  protected readonly email = signal('');

  protected subscribe(event: Event): void {
    event.preventDefault();
    if (!this.email().includes('@')) return;
    this.email.set('');
    this.toast.show(this.i18n.t().footer.subscribed);
  }
}
