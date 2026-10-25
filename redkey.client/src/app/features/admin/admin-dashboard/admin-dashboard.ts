import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { JOIN_REQUESTS, REQUESTS } from '../admin.data';
import { REQUEST_STATUS_BADGE, requestStatusLabel, requestTypeLabel } from '../request-status';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink, Icon, LocPipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminDashboard {
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  protected readonly firstName = computed(() => this.auth.user()?.fullName.split(' ')[0] ?? '');
  protected readonly rows = REQUESTS.slice(0, 5);
  protected readonly pending = JOIN_REQUESTS;
  protected readonly badge = REQUEST_STATUS_BADGE;
  protected readonly statusLabel = requestStatusLabel;
  protected readonly typeLabel = requestTypeLabel;
}
