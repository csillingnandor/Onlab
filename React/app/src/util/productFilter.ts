import type { Product } from './Types.js';
import { inRange, matchesText, normalizeText } from '../common/filter/filterUtils.js';

// Kliensoldali termékszűrő: a már betöltött listát szűri, a backendhez nem fordul.
export type ProductFilter = {
  search: string; // névben vagy SKU-ban keres
  category: string; // '' = bármelyik kategória
  status: Product['status'] | ''; // '' = bármelyik állapot
  stockMin: string;
  stockMax: string;
  priceMin: string;
  priceMax: string;
};

export const emptyProductFilter: ProductFilter = {
  search: '',
  category: '',
  status: '',
  stockMin: '',
  stockMax: '',
  priceMin: '',
  priceMax: '',
};

export function filterProducts(products: Product[], filter: ProductFilter): Product[] {
  const term = normalizeText(filter.search);

  return products.filter((product) =>
    matchesText(term, product.name, product.sku)
    && (!filter.category || product.category === filter.category)
    && (!filter.status || product.status === filter.status)
    && inRange(product.stockQuantity, filter.stockMin, filter.stockMax)
    && inRange(product.price, filter.priceMin, filter.priceMax));
}
