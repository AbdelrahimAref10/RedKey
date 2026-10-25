import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PROPERTIES } from '../../../../../core/data/properties.data';
import { AREAS } from '../../../../../core/data/site.data';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { LocPipe } from '../../../../../core/i18n/loc.pipe';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-areas-section',
  imports: [NgOptimizedImage, RouterLink, Icon, LocPipe],
  templateUrl: './areas-section.html',
  styleUrl: './areas-section.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AreasSection {
  protected readonly i18n = inject(I18nService);

  protected readonly areas = AREAS.map(area => ({
    ...area,
    count: PROPERTIES.filter(p => p.areaId === area.id).length
  }));
}
