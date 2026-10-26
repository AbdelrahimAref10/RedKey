import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { BrokerFlowStatus, STATUS_BADGE } from '../../../core/data/status';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { BROKERS, BROKER_CLIENTS_ADMIN, BROKER_UNITS_ADMIN, FlowRow, JOIN_REQUESTS } from '../admin.data';

type Tab = 'join' | 'brokers' | 'clients' | 'units';

@Component({
  selector: 'app-admin-brokers',
  imports: [TranslatePipe, Icon, LocPipe],
  templateUrl: './admin-brokers.html',
  styleUrl: './admin-brokers.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminBrokers {
  protected readonly i18n = inject(I18nService);

  protected readonly tab = signal<Tab>('join');
  protected readonly badge = STATUS_BADGE;

  protected readonly brokers = BROKERS;
  /** Actions taken on rows (front-end only). */
  private readonly done = signal<ReadonlyMap<string, BrokerFlowStatus>>(new Map());

  protected readonly join = computed(() => this.apply(JOIN_REQUESTS));
  protected readonly clients = computed(() => this.apply(BROKER_CLIENTS_ADMIN));
  protected readonly units = computed(() => this.apply(BROKER_UNITS_ADMIN));

  protected readonly tabs = computed(() => {
    const waiting = this.join().filter(r => r.canAct).length;
    const reviewing = this.units().filter(r => r.canAct).length;
    return [
      { id: 'join' as Tab, label: this.i18n.instant('admin.brokers.tabJoin'), count: waiting },
      { id: 'brokers' as Tab, label: this.i18n.instant('admin.brokers.tabBrokers'), count: 0 },
      { id: 'clients' as Tab, label: this.i18n.instant('admin.brokers.tabClients'), count: 0 },
      { id: 'units' as Tab, label: this.i18n.instant('admin.brokers.tabUnits'), count: reviewing }
    ];
  });

  protected act(row: FlowRow): void {
    if (!row.next) return;
    this.done.update(map => new Map(map).set(row.id, row.next!));
  }

  private apply<T extends FlowRow>(rows: readonly T[]): (T & { canAct: boolean })[] {
    const done = this.done();
    return rows.map(row => {
      const changed = done.get(row.id);
      return { ...row, status: changed ?? row.status, canAct: !changed && !!row.action };
    });
  }
}
