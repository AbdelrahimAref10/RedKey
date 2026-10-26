import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { OPPORTUNITIES } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-invest-list',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, LocPipe],
  templateUrl: './invest-list.html',
  styleUrl: './invest-list.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvestList {
  protected readonly i18n = inject(I18nService);

  protected readonly opportunities = OPPORTUNITIES;
  protected readonly fmt = new Intl.NumberFormat('en-US');
}
