import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, linkedSignal, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../../../core/auth/auth.service';
import { IMAGES } from '../../../../../core/data/images';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { UnitsCatalog } from '../../../../../core/units/units-catalog.service';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-hero-search',
  imports: [TranslatePipe, NgOptimizedImage, Icon],
  templateUrl: './hero-search.html',
  styleUrl: './hero-search.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeroSearch {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  private readonly catalog = inject(UnitsCatalog);
  protected readonly i18n = inject(I18nService);

  /** First name of the signed-in user (greeting), empty for visitors. */
  protected readonly firstName = computed(() => this.auth.user()?.fullName.split(' ')[0] ?? '');

  protected readonly image = IMAGES.hero;
  protected readonly prices = [5_000_000, 10_000_000, 20_000_000] as const;

  // Dropdown data comes from the API (translated by `lang`).
  private readonly lookups = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.lookups(params) });
  private readonly types = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.unitTypes(params) });
  private readonly cityList = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.cities(params) });

  protected readonly purposes = computed(() => this.lookups.value()?.unitPurposes ?? []);
  protected readonly unitTypes = computed(() => this.types.value() ?? []);
  protected readonly cities = computed(() => this.cityList.value() ?? []);

  /** First purpose (sale) selected by default. */
  protected readonly purpose = linkedSignal(() => this.purposes()[0]?.value ?? null);
  protected readonly cityId = signal('');
  protected readonly typeId = signal('');
  protected readonly maxPrice = signal('');

  protected search(event: Event): void {
    event.preventDefault();
    void this.router.navigate(['/properties'], {
      queryParams: {
        purpose: this.purpose(),
        city: this.cityId() || null,
        type: this.typeId() || null,
        maxPrice: this.maxPrice() || null
      }
    });
  }
}
