import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'rk.favorite-units';

/** Front-end only favorites (unit ids, per browser) until the favorites API exists. */
@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly _ids = signal<ReadonlySet<string>>(readStored());

  readonly ids = this._ids.asReadonly();
  readonly count = computed(() => this._ids().size);

  toggle(id: string): void {
    this._ids.update(current => {
      const next = new Set(current);
      if (!next.delete(id)) next.add(id);
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify([...next]));
      } catch {
        /* storage unavailable */
      }
      return next;
    });
  }

  /** Drops ids of units that do not exist (or are not published) any more. */
  forget(ids: readonly string[]): void {
    if (ids.length === 0) return;
    this._ids.update(current => {
      const next = new Set(current);
      ids.forEach(id => next.delete(id));
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify([...next]));
      } catch {
        /* storage unavailable */
      }
      return next;
    });
  }
}

function readStored(): ReadonlySet<string> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return new Set(raw ? (JSON.parse(raw) as string[]) : []);
  } catch {
    return new Set();
  }
}
