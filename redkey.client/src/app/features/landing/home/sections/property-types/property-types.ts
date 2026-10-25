import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PropertyType } from '../../../../../core/data/models';
import { PROPERTIES } from '../../../../../core/data/properties.data';
import { PROPERTY_TYPE_ICONS } from '../../../../../core/data/site.data';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { Icon } from '../../../../../shared/components/icon/icon';

const TYPES: readonly PropertyType[] = ['apartment', 'villa', 'townhouse', 'chalet', 'duplex', 'office'];

@Component({
  selector: 'app-property-types',
  imports: [RouterLink, Icon],
  templateUrl: './property-types.html',
  styleUrl: './property-types.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyTypes {
  protected readonly i18n = inject(I18nService);

  protected readonly items = TYPES.map(type => ({
    type,
    icon: PROPERTY_TYPE_ICONS[type],
    count: PROPERTIES.filter(p => p.type === type).length
  }));
}
