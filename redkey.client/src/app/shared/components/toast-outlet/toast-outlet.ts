import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ToastService } from '../../../core/ui/toast.service';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-toast-outlet',
  imports: [Icon],
  templateUrl: './toast-outlet.html',
  styleUrl: './toast-outlet.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ToastOutlet {
  protected readonly toastService = inject(ToastService);
}
