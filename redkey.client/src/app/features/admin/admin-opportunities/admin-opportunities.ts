import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { OPPORTUNITIES } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-admin-opportunities',
  imports: [NgOptimizedImage, ReactiveFormsModule, RouterLink, Icon, LocPipe],
  templateUrl: './admin-opportunities.html',
  styleUrl: './admin-opportunities.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminOpportunities {
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  protected readonly opportunities = OPPORTUNITIES;
  protected readonly fmt = new Intl.NumberFormat('en-US');
  protected readonly showForm = signal(false);

  protected readonly form = inject(NonNullableFormBuilder).group({
    name: ['', Validators.required],
    area: ['', Validators.required],
    yield: [0, [Validators.required, Validators.min(0.1)]],
    sharePrice: [0, [Validators.required, Validators.min(1)]],
    duration: ['', Validators.required],
    description: ['']
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.form.reset();
    this.showForm.set(false);
    this.toast.show(this.i18n.t().common.savedLocally);
  }
}
