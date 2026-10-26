/** Badge colour per viewing request status (back-end values: 1 new, 2 contacted, 3 closed, 4 viewing done). */
export const REQUEST_STATUS_BADGE: Readonly<Record<number, string>> = {
  1: 'badge-red',
  2: 'badge-blue',
  3: 'badge-gray',
  4: 'badge-green'
};
