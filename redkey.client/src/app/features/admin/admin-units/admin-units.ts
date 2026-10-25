import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Purpose } from '../../../core/data/models';
import { PROPERTIES } from '../../../core/data/properties.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { UNIT_REQUESTS, UNIT_STATUS, UnitStatus } from '../admin.data';

const STATUS_BADGE: Readonly<Record<UnitStatus, string>> = {
  published: 'badge-green',
  hidden: 'badge-gray',
  sold: 'badge-gold'
};

const ROWS = PROPERTIES.map(p => ({
  property: p,
  status: UNIT_STATUS[p.id] ?? ('published' as UnitStatus),
  requests: UNIT_REQUESTS[p.id] ?? 0
}));

@Component({
  selector: 'app-admin-units',
  imports: [NgOptimizedImage, RouterLink, LocPipe, MoneyPipe],
  templateUrl: './admin-units.html',
  styleUrl: './admin-units.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminUnits {
  protected readonly i18n = inject(I18nService);

  protected readonly badge = STATUS_BADGE;
  protected readonly total = ROWS.length;
  protected readonly tab = signal<Purpose | 'all'>('all');
  protected readonly query = signal('');

  protected readonly rows = computed(() => {
    const tab = this.tab();
    const q = this.query().trim().toLowerCase();
    return ROWS.filter(
      ({ property: p }) =>
        (tab === 'all' || p.purpose === tab) &&
        (!q ||
          p.code.toLowerCase().includes(q) ||
          p.compound.ar.includes(q) ||
          p.compound.en.toLowerCase().includes(q) ||
          p.city.ar.includes(q) ||
          p.city.en.toLowerCase().includes(q))
    );
  });
}
