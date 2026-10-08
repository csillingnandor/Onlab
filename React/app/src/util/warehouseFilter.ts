import type { Warehouse } from './Types.js';
import { inRange, matchesText, normalizeText } from '../common/filter/filterUtils.js';

// Kliensoldali raktárszűrő: a táblázat és a térkép nézetre is érvényes.
export type WarehouseFilter = {
  search: string; // névben vagy címben keres
  capacityMin: string;
  capacityMax: string;
  location: '' | 'with' | 'without'; // van-e megadott koordináta ('' = mindegy)
};

export const emptyWarehouseFilter: WarehouseFilter = {
  search: '',
  capacityMin: '',
  capacityMax: '',
  location: '',
};

const hasLocation = (warehouse: Warehouse) => warehouse.latitude !== null && warehouse.longitude !== null;

export function filterWarehouses(warehouses: Warehouse[], filter: WarehouseFilter): Warehouse[] {
  const term = normalizeText(filter.search);

  return warehouses.filter((warehouse) =>
    matchesText(term, warehouse.name, warehouse.address)
    && inRange(warehouse.capacity, filter.capacityMin, filter.capacityMax)
    && (filter.location === '' || hasLocation(warehouse) === (filter.location === 'with')));
}
