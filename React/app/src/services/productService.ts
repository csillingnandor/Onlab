import type { Product } from '../util/Types.js';

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
};

export default productService;