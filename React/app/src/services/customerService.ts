import type { Customer } from '../util/Types.js';

const CUSTOMERS_URL = import.meta.env.VITE_API_BASE_URL + '/customers';

const customerService = {
  getAll: async (): Promise<Customer[]> => {
    const response = await fetch(CUSTOMERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a vevők betöltése.');
    return response.json();
  },
};

export default customerService;
