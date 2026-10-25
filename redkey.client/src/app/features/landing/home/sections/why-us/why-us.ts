import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { IMAGES } from '../../../../../core/data/images';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-why-us',
  imports: [NgOptimizedImage, RouterLink, Icon],
  templateUrl: './why-us.html',
  styleUrl: './why-us.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WhyUs {
  protected readonly i18n = inject(I18nService);
  protected readonly image = IMAGES.aboutTeam;
}
