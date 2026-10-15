import type { CustomerOrder, CustomerOrderItem, NewOrderItem } from '../util/Types.js';
import { apiFetch, postJson, readValidationErrors } from '../util/apiErrors.js';

const ORDERS_URL = import.meta.env.VITE_API_BASE_URL + '/customerorders';
const ORDER_ITEMS_URL = import.meta.env.VITE_API_BASE_URL + '/customerorderitems';

// ship: Pending -> Shipped (készlet csökken), deliver: Shipped -> Delivered, cancel: Pending -> Cancelled
export type OrderAction = 'ship' | 'deliver' | 'cancel';

const orderService = {
  getAll: async (): Promise<CustomerOrder[]> => {
    const response = await apiFetch(ORDERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a rendelések betöltése.');
    return response.json();
  },

  getById: async (id: number): Promise<CustomerOrder> => {
    const response = await apiFetch(`${ORDERS_URL}/${id}`);
    if (!response.ok) throw new Error('Nem sikerült a rendelés betöltése.');
    return response.json();
  },

  // Az egységárat a backend a termék aktuális árából veszi
  create: (customerId: number, warehouseId: number, items: NewOrderItem[]): Promise<CustomerOrder> =>
    postJson(
      ORDERS_URL,
      { customerId, warehouseId, items: items.map(({ productId, quantity }) => ({ productId, quantity })) },
      'Nem sikerült a rendelés mentése.',
    ),

  // Státuszváltás (ship / deliver / cancel); a válasz a frissített rendelés.
  // 409: a rendelés nem a megfelelő állapotban van, vagy kiszállításnál nincs elég készlet (az üzenetet a backend adja).
  changeStatus: async (id: number, action: OrderAction): Promise<CustomerOrder> => {
    const response = await apiFetch(`${ORDERS_URL}/${id}/${action}`, { method: 'POST' });
    if (response.status === 404) throw new Error('A rendelés már nem létezik.');
    if (response.status === 409) throw new Error(await readValidationErrors(response, 'A művelet nem hajtható végre.'));
    if (!response.ok) throw new Error('Nem sikerült a rendelés állapotának módosítása.');
    return response.json();
  },

  getAllItems: async (): Promise<CustomerOrderItem[]> => {
    const response = await apiFetch(ORDER_ITEMS_URL);
    if (!response.ok) throw new Error('Nem sikerült a rendelési tételek betöltése.');
    return response.json();
  },

  getItemsByOrder: async (orderId: number): Promise<CustomerOrderItem[]> => {
    const response = await apiFetch(`${ORDER_ITEMS_URL}/by-order/${orderId}`);
    if (!response.ok) throw new Error('Nem sikerült a rendelés tételeinek betöltése.');
    return response.json();
  },
};

export default orderService;
