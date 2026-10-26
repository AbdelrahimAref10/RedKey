import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { IMAGES } from '../../../core/data/images';
import { I18nService } from '../../../core/i18n/i18n.service';
import { Icon } from '../../../shared/components/icon/icon';

/** Split card (image side + form side) shared by login and register. */
@Component({
  selector: 'app-auth-shell',
  imports: [TranslatePipe, NgOptimizedImage, Icon],
  templateUrl: './auth-shell.html',
  styleUrl: './auth-shell.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthShell {
  protected readonly i18n = inject(I18nService);
  protected readonly image = IMAGES.authSide;
}
