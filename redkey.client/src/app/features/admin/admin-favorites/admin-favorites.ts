import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { FAVORITES_BY_UNIT, FAVORITES_BY_USER } from '../admin.data';

const MAX = Math.max(...FAVORITES_BY_UNIT.map(r => r.count));

@Component({
  selector: 'app-admin-favorites',
  imports: [TranslatePipe, Icon, LocPipe],
  templateUrl: './admin-favorites.html',
  styleUrl: './admin-favorites.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminFavorites {
  protected readonly i18n = inject(I18nService);

  protected readonly tab = signal<'user' | 'unit'>('user');
  protected readonly byUser = FAVORITES_BY_USER;
  protected readonly byUnit = FAVORITES_BY_UNIT.map(r => ({ ...r, bar: Math.round((r.count / MAX) * 120) }));
}
