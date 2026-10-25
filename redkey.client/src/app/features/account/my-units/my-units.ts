import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { findProperty } from '../../../core/data/properties.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { MY_UNITS } from '../account.data';

const UNITS = MY_UNITS.flatMap(unit => {
  const property = findProperty(unit.propertyId);
  return property ? [{ unit, property }] : [];
});

@Component({
  selector: 'app-my-units',
  imports: [NgOptimizedImage, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './my-units.html',
  styleUrl: './my-units.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MyUnits {
  protected readonly i18n = inject(I18nService);
  protected readonly items = UNITS;
}
