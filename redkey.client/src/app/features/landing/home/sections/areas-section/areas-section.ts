import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { IMAGES } from '../../../../../core/data/images';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { UnitsCatalog } from '../../../../../core/units/units-catalog.service';
import { Icon } from '../../../../../shared/components/icon/icon';

/** Cities have no picture in the database yet: these decorate the cards by position. */
const BACKGROUNDS = [IMAGES.areas.newCairo, IMAGES.areas.sheikhZayed, IMAGES.areas.northCoast, IMAGES.areas.newCapital];

@Component({
  selector: 'app-areas-section',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon],
  templateUrl: './areas-section.html',
  styleUrl: './areas-section.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AreasSection {
  protected readonly i18n = inject(I18nService);
  private readonly catalog = inject(UnitsCatalog);

  private readonly list = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.cities(params) });

  protected readonly areas = computed(() =>
    (this.list.value() ?? []).slice(0, BACKGROUNDS.length).map((city, i) => ({ ...city, image: BACKGROUNDS[i] }))
  );
}
