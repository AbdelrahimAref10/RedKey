import { Translations } from '../../core/i18n/translations/ar';
import { RequestStatus, RequestType } from './admin.data';

export const REQUEST_STATUS_BADGE: Readonly<Record<RequestStatus, string>> = {
  0: 'badge-red',
  1: 'badge-blue',
  2: 'badge-green'
};

export function requestStatusLabel(t: Translations, status: RequestStatus): string {
  const r = t.admin.requests;
  return status === 0 ? r.new : status === 1 ? r.contacted : r.closed;
}

export function requestTypeLabel(t: Translations, type: RequestType): string {
  return t.admin.requests[type];
}
