import { DOCUMENT, NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, numberAttribute } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Params, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { BrokerUnitsClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';

const optionalNumber = (value: unknown): number | null => {
  const n = Number(value);
  return value == null || value === '' || Number.isNaN(n) ? null : n;
};

/** Published units with their commission (what the broker can sell), 10 per page. */
@Component({
  selector: 'app-broker-units',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, MoneyPipe, Pagination],
  templateUrl: './broker-units.html',
  styleUrl: './broker-units.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerUnits {
  private readonly document = inject(DOCUMENT);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly client = inject(BrokerUnitsClient);
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  readonly purpose = input(null, { transform: optionalNumber });
  readonly page = input(1, { transform: (v: unknown) => Math.max(1, numberAttribute(v, 1)) });

  private readonly lookups = rxResource({ params: () => this.auth.language(), stream: () => this.client.getFormLookups() });
  protected readonly purposes = computed(() => this.lookups.value()?.enums.unitPurposes ?? []);

  protected readonly units = rxResource({
    params: () => ({ lang: this.auth.language(), purpose: this.purpose(), page: this.page() }),
    stream: ({ params }) => this.client.getCatalog(params.purpose, null, null, null, null, null, null, params.page, null, null)
  });

  protected setPurpose(purpose: number | null): void {
    void this.router.navigate([], { queryParams: { purpose, page: null } as Params, queryParamsHandling: 'merge' });
  }

  protected goToPage(page: number): void {
    void this.router.navigate([], { queryParams: { page }, queryParamsHandling: 'merge' });
  }

  protected async share(id: string): Promise<void> {
    const url = `${this.document.location.origin}/properties/${id}`;
    try {
      await navigator.clipboard.writeText(url);
    } catch {
      /* clipboard blocked: the toast still shows the link */
    }
    this.toast.show(`${this.i18n.instant('broker.units.copied')}: ${url}`, 'info');
  }
}
