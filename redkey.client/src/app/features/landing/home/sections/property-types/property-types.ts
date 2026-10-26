import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { UnitsCatalog } from '../../../../../core/units/units-catalog.service';
import { Icon } from '../../../../../shared/components/icon/icon';

/** "Browse by type": the unit types the admin added, with the icons he uploaded. */
@Component({
  selector: 'app-property-types',
  imports: [TranslatePipe, RouterLink, Icon],
  templateUrl: './property-types.html',
  styleUrl: './property-types.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyTypes {
  protected readonly i18n = inject(I18nService);
  private readonly catalog = inject(UnitsCatalog);

  private readonly types = rxResource({
    params: () => this.i18n.lang(),
    stream: ({ params }) => this.catalog.unitTypes(params)
  });

  protected readonly items = computed(() => this.types.value() ?? []);
}
