import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { UserRole } from '../../../../../core/api/redkey-api.generated';
import { AuthService } from '../../../../../core/auth/auth.service';
import { I18nService } from '../../../../../core/i18n/i18n.service';
import { Icon } from '../../../../../shared/components/icon/icon';

@Component({
  selector: 'app-broker-cta',
  imports: [TranslatePipe, Icon],
  templateUrl: './broker-cta.html',
  styleUrl: './broker-cta.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BrokerCta {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly i18n = inject(I18nService);

  /**
   * Brokers go to their dashboard. Anyone else lands on the register page with the broker
   * account preselected — a signed-in customer is signed out first, otherwise the guest-only
   * register page would bounce them back to the home page.
   */
  protected join(): void {
    if (this.auth.role() === UserRole.Broker) {
      void this.router.navigateByUrl('/broker');
      return;
    }
    if (this.auth.isAuthenticated()) this.auth.logout(false);
    void this.router.navigate(['/register'], { queryParams: { role: 'broker' } });
  }
}
