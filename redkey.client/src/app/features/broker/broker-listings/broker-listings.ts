import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, numberAttribute } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Params, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { BrokerUnitsClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { COMPANY } from '../../../core/data/company';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';

const optionalNumber = (value: unknown): number | null => {
  const n = Number(value);
  return value == null || value === '' || Number.isNaN(n) ? null : n;
};

/** The broker's own units in every review status, 10 per page. */
@Component({
  selector: 'app-broker-listings',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, MoneyPipe, Pagination],
  templateUrl: './broker-listings.html',
  styleUrl: './broker-listings.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerListings {
  private readonly router = inject(Router);
  private readonly client = inject(BrokerUnitsClient);
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  readonly purpose = input(null, { transform: optionalNumber });
  readonly status = input(null, { transform: optionalNumber });
  readonly page = input(1, { transform: (v: unknown) => Math.max(1, numberAttribute(v, 1)) });

  protected readonly whatsapp = COMPANY.whatsappLink;

  private readonly lookups = rxResource({ params: () => this.auth.language(), stream: () => this.client.getFormLookups() });
  protected readonly purposes = computed(() => this.lookups.value()?.enums.unitPurposes ?? []);
  protected readonly statuses = computed(() => this.lookups.value()?.enums.unitStatuses ?? []);

  protected readonly units = rxResource({
    params: () => ({ lang: this.auth.language(), purpose: this.purpose(), status: this.status(), page: this.page() }),
    stream: ({ params }) =>
      this.client.getMyUnits(params.status, params.purpose, null, null, null, null, null, null, params.page, null, null)
  });

  protected setFilter(params: Params): void {
    void this.router.navigate([], { queryParams: { ...params, page: null }, queryParamsHandling: 'merge' });
  }

  protected goToPage(page: number): void {
    void this.router.navigate([], { queryParams: { page }, queryParamsHandling: 'merge' });
  }
}
