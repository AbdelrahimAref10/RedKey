import { ChangeDetectionStrategy, Component, ElementRef, effect, inject, viewChild } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmService } from '../../../core/ui/confirm.service';
import { Icon } from '../icon/icon';

/** The single confirmation dialog of the app; opened through `ConfirmService.ask()`. */
@Component({
  selector: 'app-confirm-dialog',
  imports: [TranslatePipe, Icon],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfirmDialog {
  protected readonly confirm = inject(ConfirmService);

  private readonly dialog = viewChild.required<ElementRef<HTMLDialogElement>>('dialog');

  constructor() {
    // Native <dialog>: modal backdrop, focus trap and Esc for free.
    effect(() => {
      const dialog = this.dialog().nativeElement;
      if (this.confirm.current()) {
        if (!dialog.open) dialog.showModal();
      } else if (dialog.open) {
        dialog.close();
      }
    });
  }

  /** Click on the backdrop (outside the panel) cancels. */
  protected backdropClick(event: MouseEvent): void {
    if (event.target === this.dialog().nativeElement) this.confirm.close(false);
  }
}
