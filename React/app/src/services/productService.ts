import type { Product, ProductSaleStatistics } from '../util/Types.js';
import { readValidationErrors } from '../util/apiErrors.js';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL + '/products';

const productService = {
  getAll: async (): Promise<Product[]> => {
    const response = await fetch(API_BASE_URL);
    if (!response.ok) throw new Error('Nem sikerült a termékek betöltése.');
    return response.json();
  },

  create: async (product: Omit<Product, 'id'>): Promise<Product> => {
    const response = await fetch(API_BASE_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(product),
    });
    if (!response.ok) throw new Error('Nem sikerült a termék mentése.');
    return response.json();
  },

  // from / to: 'YYYY-MM-DD'; ha elhagyjuk, a backend az utolsó 30 napot adja
  getSaleStatistics: async (productId: number, from?: string, to?: string): Promise<ProductSaleStatistics> => {
    const params = new URLSearchParams();
    if (from) params.set('from', from);
    if (to) params.set('to', to);
    const query = params.size > 0 ? `?${params}` : '';

    const response = await fetch(`${API_BASE_URL}/${productId}/sales${query}`);
    if (response.status === 404) throw new Error('Nincs ilyen termék.');
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'Hibás lekérdezési paraméterek.'));
    if (!response.ok) throw new Error('Nem sikerült a termék eladási statisztikáinak lekérése.');
    return response.json();
  },
};

export default productService;
