import type { Warehouse, WarehouseInput, WarehouseLayout } from '../util/Types.js';
import { apiFetch, deleteRequest, postJson, putJson } from '../util/apiErrors.js';

const WAREHOUSES_URL = import.meta.env.VITE_API_BASE_URL + '/warehouses';

const warehouseService = {
  getAll: async (): Promise<Warehouse[]> => {
    const response = await apiFetch(WAREHOUSES_URL);
    if (!response.ok) throw new Error('Nem sikerült a raktárak betöltése.');
    return response.json();
  },

  // Polcok, tárolóhelyek és a helyekhez rendelt készlet (3D nézet)
  getLayout: async (id: number): Promise<WarehouseLayout> => {
    const response = await apiFetch(`${WAREHOUSES_URL}/${id}/layout`);
    if (response.status === 404) throw new Error('Nincs ilyen raktár.');
    if (!response.ok) throw new Error('Nem sikerült a raktár elrendezésének betöltése.');
    return response.json();
  },

  create: (warehouse: WarehouseInput): Promise<Warehouse> =>
    postJson(WAREHOUSES_URL, warehouse, 'Nem sikerült a raktár mentése.'),

  update: (id: number, warehouse: WarehouseInput): Promise<Warehouse> =>
    putJson(`${WAREHOUSES_URL}/${id}`, warehouse, 'Nem sikerült a raktár módosítása.'),

  // 409: készlet, rendelés vagy készletmozgás hivatkozik rá
  delete: (id: number): Promise<void> =>
    deleteRequest(`${WAREHOUSES_URL}/${id}`, 'Nem sikerült a raktár törlése.'),
};

export default warehouseService;
