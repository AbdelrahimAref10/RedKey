import { ChangeDetectionStrategy, Component, OnInit, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PROPERTIES, findProperty } from '../../../core/data/properties.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-client-form',
  imports: [ReactiveFormsModule, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './client-form.html',
  styleUrl: './client-form.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientForm implements OnInit {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  /** ?unit=1042 preselects the unit. */
  readonly unit = input<string>();

  protected readonly units = PROPERTIES;

  protected readonly form = inject(NonNullableFormBuilder).group({
    unitId: [PROPERTIES[0].id, Validators.required],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    phone: ['', [Validators.required, Validators.pattern(/^\+?[0-9 ]{8,18}$/)]],
    notes: ['', Validators.maxLength(500)]
  });

  private readonly unitId = toSignal(this.form.controls.unitId.valueChanges, { initialValue: PROPERTIES[0].id });
  protected readonly selected = computed(() => findProperty(Number(this.unitId())));

  ngOnInit(): void {
    const id = Number(this.unit());
    if (findProperty(id)) this.form.controls.unitId.setValue(id);
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
    this.toast.show(this.i18n.t().common.savedLocally);
    void this.router.navigateByUrl('/broker/clients');
  }
}
