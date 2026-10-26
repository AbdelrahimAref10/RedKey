import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { MY_UNITS } from '../account.data';

@Component({
  selector: 'app-my-units',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './my-units.html',
  styleUrl: './my-units.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MyUnits {
  protected readonly i18n = inject(I18nService);
  protected readonly items = MY_UNITS;
}
