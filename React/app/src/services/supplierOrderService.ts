import type { SupplierOrder } from '../util/Types.js';

const SUPPLIER_ORDERS_URL = import.meta.env.VITE_API_BASE_URL + '/supplierorders';

const supplierOrderService = {
  getAll: async (): Promise<SupplierOrder[]> => {
    const response = await fetch(SUPPLIER_ORDERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a beszerzési rendelések betöltése.');
    return response.json();
  },
};

export default supplierOrderService;
