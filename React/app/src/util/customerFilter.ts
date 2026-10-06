import type { Customer } from './Types.js';
import { inRange, matchesText, normalizeText } from './filterUtils.js';

// Kliensoldali vevőszűrő: a már betöltött listát szűri, a backendhez nem fordul.
export type CustomerFilter = {
  search: string; // névben, e-mailben vagy telefonszámban keres
  ordersMin: string;
  ordersMax: string;
  spentMin: string;
  spentMax: string;
  lastOrderFrom: string; // 'YYYY-MM-DD' vagy ''
  lastOrderTo: string;
};

export const emptyCustomerFilter: CustomerFilter = {
  search: '',
  ordersMin: '',
  ordersMax: '',
  spentMin: '',
  spentMax: '',
  lastOrderFrom: '',
  lastOrderTo: '',
};

export function isLastOrderRangeInvalid(filter: CustomerFilter): boolean {
  return filter.lastOrderFrom !== '' && filter.lastOrderTo !== '' && filter.lastOrderFrom > filter.lastOrderTo;
}

// Dátumhatár megadásakor a még nem rendelt vevők kiesnek. A nap ugyanúgy szövegként
// hasonlítható össze, mint a rendelésszűrőben (orderFilter.ts).
function lastOrderInRange(customer: Customer, filter: CustomerFilter): boolean {
  if (!filter.lastOrderFrom && !filter.lastOrderTo) return true;
  if (!customer.lastOrderDate) return false;

  const day = customer.lastOrderDate.slice(0, 10);
  return (!filter.lastOrderFrom || day >= filter.lastOrderFrom) && (!filter.lastOrderTo || day <= filter.lastOrderTo);
}

export function filterCustomers(customers: Customer[], filter: CustomerFilter): Customer[] {
  const term = normalizeText(filter.search);

  return customers.filter((customer) =>
    matchesText(term, customer.name, customer.email, customer.phone)
    && inRange(customer.orderCount, filter.ordersMin, filter.ordersMax)
    && inRange(customer.totalSpent, filter.spentMin, filter.spentMax)
    && lastOrderInRange(customer, filter));
}
