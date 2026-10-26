import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, linkedSignal, numberAttribute } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Params, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AdminUnitsClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { Pagination } from '../../../shared/components/pagination/pagination';

const optionalNumber = (value: unknown): number | null => {
  const n = Number(value);
  return value == null || value === '' || Number.isNaN(n) ? null : n;
};

/** Every unit (admin + broker ones) with its creator and review status, 10 per page. */
@Component({
  selector: 'app-admin-units',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, MoneyPipe, Icon, Pagination],
  templateUrl: './admin-units.html',
  styleUrl: './admin-units.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminUnits {
  private readonly router = inject(Router);
  private readonly client = inject(AdminUnitsClient);
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  // Query params.
  readonly purpose = input(null, { transform: optionalNumber });
  readonly status = input(null, { transform: optionalNumber });
  readonly search = input<string | undefined>();
  readonly page = input(1, { transform: (v: unknown) => Math.max(1, numberAttribute(v, 1)) });

  protected readonly searchText = linkedSignal(() => this.search() ?? '');
  private searchTimer: ReturnType<typeof setTimeout> | undefined;

  // Admin requests use the session language (token), so they reload when it changes.
  private readonly lookups = rxResource({ params: () => this.auth.language(), stream: () => this.client.getFormLookups() });
  protected readonly purposes = computed(() => this.lookups.value()?.enums.unitPurposes ?? []);
  protected readonly statuses = computed(() => this.lookups.value()?.enums.unitStatuses ?? []);

  protected readonly units = rxResource({
    params: () => ({
      lang: this.auth.language(),
      purpose: this.purpose(),
      status: this.status(),
      search: this.search() || null,
      page: this.page()
    }),
    stream: ({ params }) =>
      this.client.getUnits(params.status, params.purpose, null, null, null, null, params.search, null, params.page, null, null)
  });

  constructor() {
    inject(DestroyRef).onDestroy(() => clearTimeout(this.searchTimer));
  }

  protected setFilter(params: Params): void {
    void this.router.navigate([], { queryParams: { ...params, page: null }, queryParamsHandling: 'merge' });
  }

  protected onSearch(value: string): void {
    this.searchText.set(value);
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => this.setFilter({ search: value.trim() || null }), 400);
  }

  protected goToPage(page: number): void {
    void this.router.navigate([], { queryParams: { page }, queryParamsHandling: 'merge' });
  }
}
