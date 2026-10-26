import { ChangeDetectionStrategy, Component, computed, effect, inject, input, untracked } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { BrokerUnitsClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

/** Register a client on a published unit (the client list itself is still front-end only). */
@Component({
  selector: 'app-client-form',
  imports: [TranslatePipe, ReactiveFormsModule, RouterLink, Icon, MoneyPipe],
  templateUrl: './client-form.html',
  styleUrl: './client-form.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientForm {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly client = inject(BrokerUnitsClient);
  private readonly auth = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  /** ?unit=<id> preselects the unit. */
  readonly unit = input<string>();

  private readonly catalog = rxResource({
    params: () => this.auth.language(),
    stream: () => this.client.getCatalog(null, null, null, null, null, null, null, 1, 50, null)
  });
  protected readonly units = computed(() => this.catalog.value()?.items ?? []);

  protected readonly form = inject(NonNullableFormBuilder).group({
    unitId: ['', Validators.required],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    phone: ['', [Validators.required, Validators.pattern(/^\+?[0-9 ]{8,18}$/)]],
    notes: ['', Validators.maxLength(500)]
  });

  private readonly unitId = toSignal(this.form.controls.unitId.valueChanges, { initialValue: '' });
  protected readonly selected = computed(() => this.units().find(u => u.id === this.unitId()));

  constructor() {
    // Preselect the unit from the query string, otherwise the first one.
    effect(() => {
      const units = this.units();
      if (!units.length || this.form.controls.unitId.value) return;
      const wanted = this.unit();
      untracked(() => this.form.controls.unitId.setValue(units.some(u => u.id === wanted) ? wanted! : units[0].id));
    });
  }

  protected invalid(name: 'name' | 'phone'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toast.show(this.i18n.instant('common.savedLocally'));
    void this.router.navigateByUrl('/broker/clients');
  }
}
