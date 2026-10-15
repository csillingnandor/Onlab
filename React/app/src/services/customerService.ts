import type { Customer, NewCustomer } from '../util/Types.js';
import { apiFetch, deleteRequest, postJson, putJson } from '../util/apiErrors.js';

const CUSTOMERS_URL = import.meta.env.VITE_API_BASE_URL + '/customers';

const customerService = {
  getAll: async (): Promise<Customer[]> => {
    const response = await apiFetch(CUSTOMERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a vevők betöltése.');
    return response.json();
  },

  create: (customer: NewCustomer): Promise<Customer> =>
    postJson(CUSTOMERS_URL, customer, 'Nem sikerült a vevő mentése.'),

  update: (id: number, customer: NewCustomer): Promise<Customer> =>
    putJson(`${CUSTOMERS_URL}/${id}`, customer, 'Nem sikerült a vevő módosítása.'),

  // 409: a vevőnek vannak rendelései
  delete: (id: number): Promise<void> =>
    deleteRequest(`${CUSTOMERS_URL}/${id}`, 'Nem sikerült a vevő törlése.'),
};

export default customerService;
