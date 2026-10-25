import { DOCUMENT, NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Purpose } from '../../../core/data/models';
import { PROPERTIES } from '../../../core/data/properties.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-broker-units',
  imports: [NgOptimizedImage, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './broker-units.html',
  styleUrl: './broker-units.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerUnits {
  private readonly document = inject(DOCUMENT);
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  protected readonly filter = signal<Purpose | 'all'>('all');
  protected readonly units = computed(() => {
    const f = this.filter();
    return f === 'all' ? PROPERTIES : PROPERTIES.filter(p => p.purpose === f);
  });

  protected async share(id: number): Promise<void> {
    const url = `${this.document.location.origin}/properties/${id}`;
    try {
      await navigator.clipboard.writeText(url);
    } catch {
      /* clipboard blocked: the toast still shows the link */
    }
    this.toast.show(`${this.i18n.t().broker.units.copied}: ${url}`, 'info');
  }
}
