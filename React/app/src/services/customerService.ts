import type { Customer, NewCustomer } from '../util/Types.js';
import { postJson } from '../util/apiErrors.js';

const CUSTOMERS_URL = import.meta.env.VITE_API_BASE_URL + '/customers';

const customerService = {
  getAll: async (): Promise<Customer[]> => {
    const response = await fetch(CUSTOMERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a vevők betöltése.');
    return response.json();
  },

  create: (customer: NewCustomer): Promise<Customer> =>
    postJson(CUSTOMERS_URL, customer, 'Nem sikerült a vevő mentése.'),
};

export default customerService;
