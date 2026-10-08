import type { Product, ProductInput, ProductSaleStatistics } from '../util/Types.js';
import { readValidationErrors } from '../util/apiErrors.js';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL + '/products';

const productService = {
  getAll: async (): Promise<Product[]> => {
    const response = await fetch(API_BASE_URL);
    if (!response.ok) throw new Error('Nem sikerült a termékek betöltése.');
    return response.json();
  },

  create: async (product: ProductInput): Promise<Product> => {
    const response = await fetch(API_BASE_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(product),
    });
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'Hibás termékadatok.'));
    if (!response.ok) throw new Error('Nem sikerült a termék mentése.');
    return response.json();
  },

  // A válasz a mentett termék a frissen számolt állapottal (status) együtt
  update: async (id: number, product: ProductInput): Promise<Product> => {
    const response = await fetch(`${API_BASE_URL}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(product),
    });
    if (response.status === 404) throw new Error('A termék már nem létezik.');
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'Hibás termékadatok.'));
    if (!response.ok) throw new Error('Nem sikerült a termék módosítása.');
    return response.json();
  },

  // 409: rendelés hivatkozik a termékre, ezért nem törölhető (az üzenetet a backend adja)
  delete: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/${id}`, { method: 'DELETE' });
    if (response.status === 404) throw new Error('A termék már nem létezik.');
    if (response.status === 409) throw new Error(await readValidationErrors(response, 'A termék nem törölhető.'));
    if (!response.ok) throw new Error('Nem sikerült a termék törlése.');
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
