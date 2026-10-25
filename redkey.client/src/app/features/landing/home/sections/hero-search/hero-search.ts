import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { IMAGES } from '../../../../../core/data/images';
import { AreaId, PropertyType, Purpose } from '../../../../../core/data/models';
import { AREAS } from '../../../../../core/data/site.data';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { LocPipe } from '../../../../../core/i18n/loc.pipe';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-hero-search',
  imports: [NgOptimizedImage, Icon, LocPipe],
  templateUrl: './hero-search.html',
  styleUrl: './hero-search.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeroSearch {
  private readonly router = inject(Router);
  protected readonly i18n = inject(I18nService);

  protected readonly image = IMAGES.hero;
  protected readonly areas = AREAS;
  protected readonly types: readonly PropertyType[] = ['apartment', 'villa', 'townhouse', 'chalet', 'duplex', 'office', 'studio'];
  protected readonly prices = [5_000_000, 10_000_000, 20_000_000] as const;

  protected readonly purpose = signal<Purpose>('sale');
  protected readonly area = signal<AreaId | ''>('');
  protected readonly type = signal<PropertyType | ''>('');
  protected readonly maxPrice = signal('');

  protected search(event: Event): void {
    event.preventDefault();
    void this.router.navigate(['/properties'], {
      queryParams: {
        purpose: this.purpose(),
        area: this.area() || null,
        type: this.type() || null,
        maxPrice: this.purpose() === 'sale' ? this.maxPrice() || null : null
      }
    });
  }
}
