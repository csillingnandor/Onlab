import type { NewOrderItem, Supplier, SupplierOrder } from '../util/Types.js';
import { postJson, readValidationErrors } from '../util/apiErrors.js';

const SUPPLIER_ORDERS_URL = import.meta.env.VITE_API_BASE_URL + '/supplierorders';
const SUPPLIERS_URL = import.meta.env.VITE_API_BASE_URL + '/suppliers';

const supplierOrderService = {
  getAll: async (): Promise<SupplierOrder[]> => {
    const response = await fetch(SUPPLIER_ORDERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszerzési rendelések betöltése.');
    return response.json();
  },

  create: (supplierId: number, warehouseId: number, items: NewOrderItem[]): Promise<SupplierOrder> =>
    postJson(SUPPLIER_ORDERS_URL, { supplierId, warehouseId, items }, 'Nem sikerült a beszerzési rendelés mentése.'),

  // Beérkeztetés: a tételek a célraktár készletéhez adódnak; a válasz a frissített (Delivered) rendelés
  receive: async (id: number): Promise<SupplierOrder> => {
    const response = await fetch(`${SUPPLIER_ORDERS_URL}/${id}/receive`, { method: 'POST' });
    if (response.status === 404) throw new Error('A rendelés már nem létezik.');
    if (response.status === 409) throw new Error(await readValidationErrors(response, 'A rendelés nem érkeztethető be.'));
    if (!response.ok) throw new Error('Nem sikerült a rendelés beérkeztetése.');
    return response.json();
  },

  // A beszerzési rendelés űrlap választólistájához
  getSuppliers: async (): Promise<Supplier[]> => {
    const response = await fetch(SUPPLIERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszállítók betöltése.');
    return response.json();
  },
};

export default supplierOrderService;
