import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { LookupAdmin, LookupRow } from '../lookup-admin/lookup-admin';
import { AreasPanel } from './areas-panel/areas-panel';

/** Cities on top; clicking a city opens its areas underneath. */
@Component({
  selector: 'app-cities-admin',
  imports: [LookupAdmin, AreasPanel],
  templateUrl: './cities-admin.html',
  styleUrl: './cities-admin.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CitiesAdmin {
  protected readonly city = signal<LookupRow | null>(null);
  /** Bumped when areas change, so the cities table refreshes its areas count. */
  protected readonly reloadKey = signal(0);
}
