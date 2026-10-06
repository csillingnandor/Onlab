import type { NewOrderItem, Supplier, SupplierOrder } from '../util/Types.js';
import { postJson } from '../util/apiErrors.js';

const SUPPLIER_ORDERS_URL = import.meta.env.VITE_API_BASE_URL + '/supplierorders';
const SUPPLIERS_URL = import.meta.env.VITE_API_BASE_URL + '/suppliers';

const supplierOrderService = {
  getAll: async (): Promise<SupplierOrder[]> => {
    const response = await fetch(SUPPLIER_ORDERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszerzési rendelések betöltése.');
    return response.json();
  },

  create: (supplierId: number, items: NewOrderItem[]): Promise<SupplierOrder> =>
    postJson(SUPPLIER_ORDERS_URL, { supplierId, items }, 'Nem sikerült a beszerzési rendelés mentése.'),

  // A beszerzési rendelés űrlap választólistájához
  getSuppliers: async (): Promise<Supplier[]> => {
    const response = await fetch(SUPPLIERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszállítók betöltése.');
    return response.json();
  },
};

export default supplierOrderService;
