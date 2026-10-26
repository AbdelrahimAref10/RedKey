import { Injectable, signal } from '@angular/core';

export interface ConfirmOptions {
  /** Defaults to "Are you sure?". */
  title?: string;
  message?: string;
  /** Defaults to "Confirm". */
  confirmText?: string;
  /** Defaults to "Cancel". */
  cancelText?: string;
  /** `danger` paints the confirm button red (deletes). */
  tone?: 'danger' | 'default';
}

interface ConfirmRequest extends ConfirmOptions {
  resolve: (confirmed: boolean) => void;
}

/**
 * App-wide confirmation dialog (replaces the browser `confirm()`):
 * `if (!(await confirm.ask({ message, tone: 'danger' }))) return;`
 * Rendered once by `<app-confirm-dialog />` in the root component.
 */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly _current = signal<ConfirmRequest | null>(null);

  readonly current = this._current.asReadonly();

  ask(options: ConfirmOptions = {}): Promise<boolean> {
    // A new question cancels the one still open.
    this._current()?.resolve(false);
    return new Promise(resolve => this._current.set({ ...options, resolve }));
  }

  close(confirmed: boolean): void {
    const request = this._current();
    if (!request) return;
    this._current.set(null);
    request.resolve(confirmed);
  }
}
