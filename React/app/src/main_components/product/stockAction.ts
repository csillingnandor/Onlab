import type { ExtraAction } from '../../common/button/ActionButtons.js';
import type { Product } from '../../util/Types.js';

// A "Készlet raktáranként" ikongomb a kártyán és a táblázat sorában
export const stockAction = (product: Product, onStock: (product: Product) => void): ExtraAction => ({
  icon: 'bi-boxes',
  label: 'Készlet raktáranként',
  ariaLabel: `${product.name} készlete`,
  onClick: () => onStock(product),
});
