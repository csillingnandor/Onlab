import type { Warehouse } from '../util/Types.js';

const WAREHOUSES_URL = import.meta.env.VITE_API_BASE_URL + '/warehouses';

const warehouseService = {
  getAll: async (): Promise<Warehouse[]> => {
    const response = await fetch(WAREHOUSES_URL);
    if (!response.ok) throw new Error('Nem sikerült a raktárak betöltése.');
    return response.json();
  },
};

export default warehouseService;
