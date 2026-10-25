import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ICONS, IconName } from './icons';

@Component({
  selector: 'app-icon',
  templateUrl: './icon.html',
  styleUrl: './icon.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class.flip]': 'flip()' }
})
export class Icon {
  readonly name = input.required<IconName>();
  readonly size = input(20);
  readonly stroke = input(1.9);
  readonly filled = input(false);
  /** Mirror the icon in RTL (directional arrows / chevrons). */
  readonly flip = input(false);

  protected readonly paths = computed(() => ICONS[this.name()]);
}
