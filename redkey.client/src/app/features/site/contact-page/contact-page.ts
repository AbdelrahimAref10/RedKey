import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, signal, untracked } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { apiErrorCode } from '../../../core/api/api-error';
import { CustomerContactMessagesClient } from '../../../core/api/redkey-api.generated';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginRequired } from '../../../core/auth/login-required.service';
import { COMPANY } from '../../../core/data/company';
import { DEFAULT_COUNTRY, findCountry, parseInternational } from '../../../core/data/countries';
import { IMAGES } from '../../../core/data/images';
import { I18nService } from '../../../core/i18n/i18n.service';
import { ToastService } from '../../../core/ui/toast.service';
import { UnitsCatalog } from '../../../core/units/units-catalog.service';
import { Icon } from '../../../shared/components/icon/icon';
import { PhoneInput, PhoneValue, nationalDigits } from '../../../shared/components/phone-input/phone-input';

/** "Send us a message": goes straight to the super admin inbox (live, with a sound). */
@Component({
  selector: 'app-contact-page',
  imports: [TranslatePipe, NgOptimizedImage, ReactiveFormsModule, Icon, PhoneInput],
  templateUrl: './contact-page.html',
  styleUrl: './contact-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContactPage {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly loginRequired = inject(LoginRequired);
  private readonly client = inject(CustomerContactMessagesClient);
  private readonly catalog = inject(UnitsCatalog);
  private readonly destroyRef = inject(DestroyRef);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(NonNullableFormBuilder);
  protected readonly i18n = inject(I18nService);

  protected readonly company = COMPANY;
  protected readonly image = IMAGES.aboutTeam;
  protected readonly sending = signal(false);

  /** Subjects come translated from the back-end (lookups). */
  private readonly lookups = rxResource({ params: () => this.i18n.lang(), stream: ({ params }) => this.catalog.lookups(params) });
  protected readonly subjects = computed(() => this.lookups.value()?.contactSubjects ?? []);

  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    /** Validated by the phone field itself (required + length). */
    phone: this.fb.control<PhoneValue | null>(this.phoneOf(null)),
    subject: this.fb.control<number | null>(null, Validators.required),
    message: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  constructor() {
    // Signed-in customer: name + phone are filled in for him.
    effect(() => {
      const user = this.auth.user();
      if (!user) return;
      untracked(() => {
        if (!this.form.controls.name.value) this.form.controls.name.setValue(user.fullName);
        if (!this.form.controls.phone.value?.number) this.form.controls.phone.setValue(this.phoneOf(user.phoneNumber));
      });
    });

    // First subject selected once they load.
    effect(() => {
      const first = this.subjects()[0];
      if (first) untracked(() => this.form.controls.subject.value ?? this.form.controls.subject.setValue(first.value));
    });
  }

  protected invalid(name: 'name' | 'phone' | 'message'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }

  protected submit(): void {
    if (!this.loginRequired.check()) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const phone = v.phone!;
    this.sending.set(true);
    this.client
      .send({
        name: v.name.trim(),
        countryIso: phone.countryIso,
        dialCode: phone.dialCode,
        phoneNumber: phone.number,
        subject: v.subject!,
        message: v.message.trim()
      })
      .pipe(
        finalize(() => this.sending.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.toast.show(this.i18n.instant('site.contact.sent'));
          void this.router.navigateByUrl('/request-sent');
        },
        error: error => this.toast.show(this.i18n.error(apiErrorCode(error)), 'error')
      });
  }

  /** The account phone ("+2010…" or "010…") as a phone field value. */
  private phoneOf(phone: string | null | undefined): PhoneValue {
    const parsed = parseInternational(phone);
    const iso = parsed?.countryIso ?? DEFAULT_COUNTRY;
    return { countryIso: iso, dialCode: findCountry(iso)!.dial, number: parsed?.number ?? nationalDigits(phone ?? '') };
  }
}
