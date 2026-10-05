import type { CustomerOrder, OrderStatus, Product } from './Types.js';

// A műszerfal összesítői, a már betöltött rendelésekből és termékekből (kliensoldalon) számolva.
// Csak a kiszállított (Delivered) rendelés számít eladásnak, ahogy a backend statisztikáiban is.

export type StatusCount = { status: OrderStatus; count: number };

export type StockAlert = {
  product: Product;
  fillPercent: number; // készlet a minimum szinthez képest, 0–100
};

export type DashboardStats = {
  revenue: number;
  deliveredCount: number;
  orderCount: number;
  averageBasket: number | null; // null, ha nem volt kiszállított rendelés
  statusCounts: StatusCount[];
  recentOrders: CustomerOrder[];
  stockAlerts: StockAlert[];
  outOfStockCount: number;
  lowStockCount: number;
};

const statusOrder: OrderStatus[] = ['Pending', 'Shipped', 'Delivered', 'Cancelled'];

// Eladások bontása egy szempont szerint (termék / vevő / kategória)
export type SalesBreakdownRow = {
  key: string;
  label: string;
  quantity: number;
  revenue: number;
  orderCount: number; // hány különböző rendelésben szerepelt
};

export type SalesBreakdowns = {
  byProduct: SalesBreakdownRow[];
  byCustomer: SalesBreakdownRow[];
  byCategory: SalesBreakdownRow[];
};

const NO_CATEGORY = 'Nincs kategória';

// ordersInRange: a kiválasztott időszakra már leszűrt rendelések; ebből csak a kiszállítottak számítanak.
// A kategóriát a terméklistából párosítjuk, mert a rendelési tétel csak a termék azonosítóját és nevét hordozza.
export function computeSalesBreakdowns(ordersInRange: CustomerOrder[], products: Product[]): SalesBreakdowns {
  const categoryByProductId = new Map(products.map((p) => [p.id, p.category.trim() || NO_CATEGORY]));

  const byProduct = new Map<string, SalesBreakdownRow & { orders: Set<number> }>();
  const byCustomer = new Map<string, SalesBreakdownRow & { orders: Set<number> }>();
  const byCategory = new Map<string, SalesBreakdownRow & { orders: Set<number> }>();

  const add = (
    map: Map<string, SalesBreakdownRow & { orders: Set<number> }>,
    key: string,
    label: string,
    orderId: number,
    quantity: number,
    revenue: number,
  ) => {
    const row = map.get(key) ?? { key, label, quantity: 0, revenue: 0, orderCount: 0, orders: new Set<number>() };
    row.quantity += quantity;
    row.revenue += revenue;
    row.orders.add(orderId);
    map.set(key, row);
  };

  for (const order of ordersInRange) {
    if (order.status !== 'Delivered') continue;

    for (const item of order.items) {
      // A tétel a rendeléskori egységárat hordozza (productPrice), ebből számolunk
      const revenue = item.productPrice * item.quantity;
      const category = categoryByProductId.get(item.productId) ?? NO_CATEGORY;

      add(byProduct, `p${item.productId}`, item.productName, order.id, item.quantity, revenue);
      add(byCustomer, `c${order.customerId}`, order.customerName, order.id, item.quantity, revenue);
      add(byCategory, `k${category}`, category, order.id, item.quantity, revenue);
    }
  }

  const finish = (map: Map<string, SalesBreakdownRow & { orders: Set<number> }>) =>
    [...map.values()]
      .map(({ orders, ...row }) => ({ ...row, orderCount: orders.size }))
      .sort((a, b) => b.revenue - a.revenue || a.label.localeCompare(b.label, 'hu'));

  return {
    byProduct: finish(byProduct),
    byCustomer: finish(byCustomer),
    byCategory: finish(byCategory),
  };
}

// ordersInRange: a kiválasztott időszakra már leszűrt rendelések
export function computeDashboardStats(ordersInRange: CustomerOrder[], products: Product[]): DashboardStats {
  const delivered = ordersInRange.filter((o) => o.status === 'Delivered');
  const revenue = delivered.reduce((sum, o) => sum + o.totalAmount, 0);

  const statusCounts = statusOrder.map((status) => ({
    status,
    count: ordersInRange.filter((o) => o.status === status).length,
  }));

  // A legújabb elöl; a dátum ISO szöveg, így szövegként rendezhető
  const recentOrders = [...ordersInRange]
    .sort((a, b) => b.orderDate.localeCompare(a.orderDate))
    .slice(0, 5);

  // Készletriasztás: a termék státuszát a backend számolja (Out of Stock / Low Stock).
  // Sorrend: előbb az elfogyottak, utána a minimumhoz képest legkisebb készletűek.
  const stockAlerts = products
    .filter((p) => p.status !== 'In Stock')
    .map((product) => ({
      product,
      fillPercent: product.minStockLevel > 0
        ? Math.min(100, Math.round((product.stockQuantity / product.minStockLevel) * 100))
        : 0,
    }))
    .sort((a, b) => a.fillPercent - b.fillPercent)
    .slice(0, 5);

  return {
    revenue,
    deliveredCount: delivered.length,
    orderCount: ordersInRange.length,
    averageBasket: delivered.length > 0 ? revenue / delivered.length : null,
    statusCounts,
    recentOrders,
    stockAlerts,
    outOfStockCount: products.filter((p) => p.status === 'Out of Stock').length,
    lowStockCount: products.filter((p) => p.status === 'Low Stock').length,
  };
}
