import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TESTIMONIALS } from '../../../../../core/data/site.data';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { LocPipe } from '../../../../../core/i18n/loc.pipe';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-testimonials-section',
  imports: [TranslatePipe, Icon, LocPipe],
  templateUrl: './testimonials-section.html',
  styleUrl: './testimonials-section.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestimonialsSection {
  protected readonly i18n = inject(I18nService);
  protected readonly items = TESTIMONIALS;
  protected readonly stars = [1, 2, 3, 4, 5];
}
