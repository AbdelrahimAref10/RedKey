import { Injectable, computed, signal } from '@angular/core';
import { PROPERTIES } from './properties.data';

const STORAGE_KEY = 'rk.favorites';

/** Front-end only favorites (per browser) until the favorites API exists. */
@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly _ids = signal<ReadonlySet<number>>(readStored());

  readonly ids = this._ids.asReadonly();
  readonly count = computed(() => this._ids().size);
  readonly properties = computed(() => PROPERTIES.filter(p => this._ids().has(p.id)));

  has(id: number): boolean {
    return this._ids().has(id);
  }

  toggle(id: number): void {
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
}

function readStored(): ReadonlySet<number> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return new Set(raw ? (JSON.parse(raw) as number[]) : [1042, 997]);
  } catch {
    return new Set();
  }
}
