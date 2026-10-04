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
