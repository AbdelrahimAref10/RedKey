import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { IMAGES } from '../../../core/data/images';
import { AreaId, Purpose } from '../../../core/data/models';
import { PROPERTIES } from '../../../core/data/properties.data';
import { AREAS } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { PropertyCard } from '../../../shared/components/property-card/property-card';

/** Customer home — prototype screen "Web-Home". */
@Component({
  selector: 'app-customer-home',
  imports: [NgOptimizedImage, RouterLink, Icon, LocPipe, PropertyCard],
  templateUrl: './customer-home.html',
  styleUrl: './customer-home.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CustomerHome {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  protected readonly image = IMAGES.hero;
  protected readonly areas = AREAS;
  protected readonly firstName = computed(() => this.auth.user()?.fullName.split(' ')[0] ?? '');
  protected readonly selected = PROPERTIES.filter(p => p.featured).slice(0, 4);

  protected readonly area = signal<AreaId | ''>('');
  protected readonly purpose = signal<Purpose>('sale');

  protected search(event: Event): void {
    event.preventDefault();
    void this.router.navigate(['/customer/properties'], {
      queryParams: { purpose: this.purpose(), area: this.area() || null }
    });
  }
}
