/** Workflow statuses used by the dashboards (labels live in `t.admin.brokers.st`). */
export type BrokerFlowStatus = 'wait' | 'ok' | 'no' | 'off' | 'reg' | 'visit' | 'sold' | 'paid' | 'rev' | 'pub';

export const STATUS_BADGE: Readonly<Record<BrokerFlowStatus, string>> = {
  wait: 'badge-gold',
  ok: 'badge-green',
  no: 'badge-red',
  off: 'badge-red',
  reg: 'badge-gray',
  visit: 'badge-blue',
  sold: 'badge-green',
  paid: 'badge-green',
  rev: 'badge-gold',
  pub: 'badge-green'
};
