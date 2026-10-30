import { Injectable, computed, effect, inject, signal, untracked } from '@angular/core';
import { catchError, concatMap, from, of, toArray } from 'rxjs';
import { CustomerFavoritesClient, UserRole } from '../api/redkey-api.generated';
import { AuthService } from '../auth/auth.service';

/** Ids saved in this browser before favorites lived on the server (moved to the account on login). */
const LEGACY_STORAGE_KEY = 'rk.favorite-units';

/**
 * The signed-in customer's favorites, stored on the server (web + mobile share them):
 * `ids` lights the hearts, `toggle` saves / removes optimistically, `version` tells pages to reload.
 */
@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly client = inject(CustomerFavoritesClient);
  private readonly auth = inject(AuthService);

  private readonly _ids = signal<ReadonlySet<string>>(new Set());
  private readonly _version = signal(0);

  readonly ids = this._ids.asReadonly();
  readonly count = computed(() => this._ids().size);
  /** Bumped after every saved change (the favorites page reloads on it). */
  readonly version = this._version.asReadonly();

  constructor() {
    // Customer signed in → move the browser's old favorites to his account, then load them; anyone else → none.
    effect(() => {
      const isCustomer = this.auth.role() === UserRole.Customer;
      const userId = this.auth.user()?.id;
      untracked(() => {
        if (isCustomer && userId) this.loadForCustomer();
        else this._ids.set(new Set());
      });
    });
  }

  toggle(id: string): void {
    const saved = this._ids().has(id);
    this.patch(id, !saved);

    (saved ? this.client.remove(id) : this.client.add(id)).subscribe({
      next: () => this._version.update(v => v + 1),
      error: () => this.patch(id, saved)
    });
  }

  private loadForCustomer(): void {
    const legacy = readLegacy();
    const merge$ = legacy.length
      ? from(legacy).pipe(
          // A unit that is gone / unpublished is simply skipped.
          concatMap(id => this.client.add(id).pipe(catchError(() => of(undefined)))),
          toArray()
        )
      : of([]);

    merge$.subscribe(() => {
      if (legacy.length) clearLegacy();
      this.client.getFavoriteIds().subscribe({
        next: ids => {
          this._ids.set(new Set(ids));
          this._version.update(v => v + 1);
        },
        error: () => undefined
      });
    });
  }

  private patch(id: string, saved: boolean): void {
    this._ids.update(current => {
      const next = new Set(current);
      if (saved) next.add(id);
      else next.delete(id);
      return next;
    });
  }
}

function readLegacy(): string[] {
  try {
    const raw = localStorage.getItem(LEGACY_STORAGE_KEY);
    return raw ? (JSON.parse(raw) as string[]) : [];
  } catch {
    return [];
  }
}

function clearLegacy(): void {
  try {
    localStorage.removeItem(LEGACY_STORAGE_KEY);
  } catch {
    /* storage unavailable */
  }
}
