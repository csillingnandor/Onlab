import type { Supplier, SupplierInput } from '../util/Types.js';
import { apiFetch, deleteRequest, postJson, putJson } from '../util/apiErrors.js';

const SUPPLIERS_URL = import.meta.env.VITE_API_BASE_URL + '/suppliers';

const supplierService = {
  getAll: async (): Promise<Supplier[]> => {
    const response = await apiFetch(SUPPLIERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszállítók betöltése.');
    return response.json();
  },

  create: (supplier: SupplierInput): Promise<Supplier> =>
    postJson(SUPPLIERS_URL, supplier, 'Nem sikerült a beszállító mentése.'),

  update: (id: number, supplier: SupplierInput): Promise<Supplier> =>
    putJson(`${SUPPLIERS_URL}/${id}`, supplier, 'Nem sikerült a beszállító módosítása.'),

  // 409: a beszállítónak vannak rendelései
  delete: (id: number): Promise<void> =>
    deleteRequest(`${SUPPLIERS_URL}/${id}`, 'Nem sikerült a beszállító törlése.'),
};

export default supplierService;
