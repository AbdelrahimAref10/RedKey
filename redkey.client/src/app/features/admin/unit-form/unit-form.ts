import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, input, numberAttribute, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { IMAGES } from '../../../core/data/images';
import { PropertyType, Purpose } from '../../../core/data/models';
import { findProperty } from '../../../core/data/properties.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../../../shared/components/icon/icon';

@Component({
  selector: 'app-unit-form',
  imports: [NgOptimizedImage, ReactiveFormsModule, RouterLink, Icon],
  templateUrl: './unit-form.html',
  styleUrl: './unit-form.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UnitForm implements OnInit {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  protected readonly i18n = inject(I18nService);

  /** Route param (`/admin/units/:id/edit`); 0 = new unit. */
  readonly id = input(0, { transform: numberAttribute });

  protected readonly property = computed(() => findProperty(this.id()));
  protected readonly images = computed(() =>
    this.property()?.images.slice(0, 2) ?? [IMAGES.properties.apartmentLandscape, IMAGES.properties.apartmentFurnished]
  );
  protected readonly types: readonly PropertyType[] = ['apartment', 'villa', 'townhouse', 'chalet', 'duplex', 'office', 'studio'];
  protected readonly published = signal(true);
  protected readonly uploaded = signal<string[]>([]);

  protected readonly form = inject(NonNullableFormBuilder).group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    purpose: ['sale' as Purpose],
    type: ['apartment' as PropertyType],
    location: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(1)]],
    rooms: [3],
    baths: [2],
    size: [0, [Validators.required, Validators.min(10)]],
    finishing: [0],
    payment: [''],
    description: ['']
  });

  ngOnInit(): void {
    const p = this.property();
    const lang = this.i18n.lang();
    if (p) {
      this.form.patchValue({
        title: p.title[lang],
        purpose: p.purpose,
        type: p.type,
        location: `${p.compound[lang]} · ${p.city[lang]}`,
        price: p.price,
        rooms: p.beds,
        baths: p.baths,
        size: p.size,
        payment: p.paymentPlans.map(plan => `${plan.label[lang]}: ${plan.value[lang]}`).join(' · '),
        description: p.description[lang]
      });
    }
  }

  protected pick(event: Event): void {
    const files = Array.from((event.target as HTMLInputElement).files ?? []).map(f => f.name);
    this.uploaded.update(list => [...list, ...files]);
  }

  protected invalid(name: 'title' | 'location' | 'price' | 'size'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toast.show(this.i18n.t().common.savedLocally);
    void this.router.navigateByUrl('/admin/units');
  }
}
