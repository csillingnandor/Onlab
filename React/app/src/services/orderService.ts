import type { CustomerOrder, CustomerOrderItem, NewOrderItem } from '../util/Types.js';
import { postJson } from '../util/apiErrors.js';

const ORDERS_URL = import.meta.env.VITE_API_BASE_URL + '/customerorders';
const ORDER_ITEMS_URL = import.meta.env.VITE_API_BASE_URL + '/customerorderitems';

const orderService = {
  getAll: async (): Promise<CustomerOrder[]> => {
    const response = await fetch(ORDERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a rendelések betöltése.');
    return response.json();
  },

  getById: async (id: number): Promise<CustomerOrder> => {
    const response = await fetch(`${ORDERS_URL}/${id}`);
    if (!response.ok) throw new Error('Nem sikerült a rendelés betöltése.');
    return response.json();
  },

  // Az egységárat a backend a termék aktuális árából veszi
  create: (customerId: number, items: NewOrderItem[]): Promise<CustomerOrder> =>
    postJson(
      ORDERS_URL,
      { customerId, items: items.map(({ productId, quantity }) => ({ productId, quantity })) },
      'Nem sikerült a rendelés mentése.',
    ),

  getAllItems: async (): Promise<CustomerOrderItem[]> => {
    const response = await fetch(ORDER_ITEMS_URL);
    if (!response.ok) throw new Error('Nem sikerült a rendelési tételek betöltése.');
    return response.json();
  },

  getItemsByOrder: async (orderId: number): Promise<CustomerOrderItem[]> => {
    const response = await fetch(`${ORDER_ITEMS_URL}/by-order/${orderId}`);
    if (!response.ok) throw new Error('Nem sikerült a rendelés tételeinek betöltése.');
    return response.json();
  },
};

export default orderService;
