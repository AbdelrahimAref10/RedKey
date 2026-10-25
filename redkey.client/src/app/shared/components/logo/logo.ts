import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { IMAGES } from '../../../core/data/images';

@Component({
  selector: 'app-logo',
  templateUrl: './logo.html',
  styleUrl: './logo.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class.light]': "tone() === 'light'" }
})
export class Logo {
  /** `light` = white text (dark / red backgrounds). */
  readonly tone = input<'dark' | 'light'>('dark');
  readonly size = input(36);
  /** Optional suffix such as "Admin" / "Broker". */
  readonly suffix = input<string | null>(null);

  protected readonly mark = IMAGES.logoMark;
}
