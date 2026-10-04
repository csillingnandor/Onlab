import type { OrderStatus } from './Types.js';

// Kliensoldali rendelésszűrő: a már betöltött listát szűri, a backendhez nem fordul.
export type OrderFilter = {
  name: string; // vevő / beszállító nevében keres, kis-nagybetűtől függetlenül
  status: OrderStatus | ''; // '' = bármelyik státusz
  from: string; // 'YYYY-MM-DD' vagy '' (nincs alsó határ)
  to: string; // 'YYYY-MM-DD' vagy '' (nincs felső határ); from === to esetén egyetlen nap
};

export const emptyOrderFilter: OrderFilter = { name: '', status: '', from: '', to: '' };

export function countActiveFilters(filter: OrderFilter): number {
  return [filter.name.trim(), filter.status, filter.from, filter.to].filter(Boolean).length;
}

export function isDateRangeInvalid(filter: OrderFilter): boolean {
  return filter.from !== '' && filter.to !== '' && filter.from > filter.to;
}

type FilterableOrder = {
  orderDate: string; // ISO szöveg, pl. '2026-09-20T11:00:00'
  status: OrderStatus;
};

export function filterOrders<T extends FilterableOrder>(
  orders: T[],
  filter: OrderFilter,
  getPartyName: (order: T) => string,
): T[] {
  const term = filter.name.trim().toLocaleLowerCase('hu-HU');

  return orders.filter((order) => {
    if (term && !getPartyName(order).toLocaleLowerCase('hu-HU').includes(term)) return false;
    if (filter.status && order.status !== filter.status) return false;

    // A dátum első 10 karaktere ('YYYY-MM-DD') ugyanaz a nap, ami a listában megjelenik;
    // így szövegként összehasonlítható, időzóna-átváltás nélkül.
    const day = order.orderDate.slice(0, 10);
    if (filter.from && day < filter.from) return false;
    if (filter.to && day > filter.to) return false;

    return true;
  });
}
