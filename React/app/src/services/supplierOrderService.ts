import type { NewOrderItem, SupplierOrder } from '../util/Types.js';
import { apiFetch, postJson, putJson, readValidationErrors } from '../util/apiErrors.js';

const SUPPLIER_ORDERS_URL = import.meta.env.VITE_API_BASE_URL + '/supplierorders';

const supplierOrderService = {
  getAll: async (): Promise<SupplierOrder[]> => {
    const response = await apiFetch(SUPPLIER_ORDERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszerzési rendelések betöltése.');
    return response.json();
  },

  create: (supplierId: number, warehouseId: number, items: NewOrderItem[]): Promise<SupplierOrder> =>
    postJson(SUPPLIER_ORDERS_URL, { supplierId, warehouseId, items }, 'Nem sikerült a beszerzési rendelés mentése.'),

  // Függő beszerzés módosítása (409, ha közben már nem függő)
  update: (id: number, supplierId: number, warehouseId: number, items: NewOrderItem[]): Promise<SupplierOrder> =>
    putJson(`${SUPPLIER_ORDERS_URL}/${id}`, { supplierId, warehouseId, items }, 'Nem sikerült a beszerzési rendelés módosítása.'),

  // Beérkeztetés: a tételek a célraktár készletéhez adódnak; a válasz a frissített (Delivered) rendelés
  receive: async (id: number): Promise<SupplierOrder> => {
    const response = await apiFetch(`${SUPPLIER_ORDERS_URL}/${id}/receive`, { method: 'POST' });
    if (response.status === 404) throw new Error('A rendelés már nem létezik.');
    if (response.status === 409) throw new Error(await readValidationErrors(response, 'A rendelés nem érkeztethető be.'));
    if (!response.ok) throw new Error('Nem sikerült a rendelés beérkeztetése.');
    return response.json();
  },
};

export default supplierOrderService;
