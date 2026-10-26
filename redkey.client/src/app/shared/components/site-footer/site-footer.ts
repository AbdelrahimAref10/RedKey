import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { COMPANY } from '../../../core/data/company';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ToastService } from '../../../core/ui/toast.service';
import { UnitsCatalog } from '../../../core/units/units-catalog.service';
import { Icon } from '../icon/icon';
import { Logo } from '../logo/logo';

@Component({
  selector: 'app-site-footer',
  imports: [TranslatePipe, RouterLink, Icon, Logo],
  templateUrl: './site-footer.html',
  styleUrl: './site-footer.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SiteFooter {
  protected readonly i18n = inject(I18nService);
  private readonly toast = inject(ToastService);

  protected readonly company = COMPANY;
  protected readonly year = new Date().getFullYear();
  private readonly catalog = inject(UnitsCatalog);
  private readonly unitTypes = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.unitTypes(params) });
  protected readonly types = computed(() => (this.unitTypes.value() ?? []).slice(0, 5));
  protected readonly email = signal('');

  protected subscribe(event: Event): void {
    event.preventDefault();
    if (!this.email().includes('@')) return;
    this.email.set('');
    this.toast.show(this.i18n.instant('footer.subscribed'));
  }
}
