import { useEffect, useMemo, useState } from 'react';
import { Alert, Spinner } from 'react-bootstrap';
import orderService from '../../services/orderService.js';
import productService from '../../services/productService.js';
import type { CustomerOrder, Product } from '../../util/Types.js';
import { computeDashboardStats, computeSalesBreakdowns } from '../../util/dashboardStats.js';
import { presetRange, type DateRange } from '../../util/dateRange.js';
import { priceFormatter } from '../../util/format.js';
import { emptyOrderFilter, filterOrders } from '../../util/orderFilter.js';
import DateRangeMenu from './DateRangeMenu.js';
import KpiCard from './KpiCard.js';
import OrderStatusChart from './OrderStatusChart.js';
import ProductSalesPanel from './ProductSalesPanel.js';
import RecentOrdersCard from './RecentOrdersCard.js';
import SalesBreakdownCard from './SalesBreakdownCard.js';
import StockAlertsCard from './StockAlertsCard.js';
import './DashboardPage.css';

export default function DashboardPage() {
  const [range, setRange] = useState<DateRange>(() => presetRange('month'));
  const [orders, setOrders] = useState<CustomerOrder[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Rendelések és termékek egyszer; az időszak váltásakor csak újraszámolunk, nem töltünk újra.
  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    Promise.all([orderService.getAll(), productService.getAll()])
      .then(([orderData, productData]) => {
        if (ignore) return;
        setOrders(orderData);
        setProducts([...productData].sort((a, b) => a.name.localeCompare(b.name, 'hu')));
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.');
      })
      .finally(() => {
        if (!ignore) setLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, []);

  // Ugyanaz a dátumszűrés, mint a rendelésoldalakon (zárt intervallum, napra pontosan)
  const { stats, breakdowns } = useMemo(() => {
    const ordersInRange = filterOrders(
      orders,
      { ...emptyOrderFilter, from: range.from, to: range.to },
      (order) => order.customerName,
    );
    return {
      stats: computeDashboardStats(ordersInRange, products),
      breakdowns: computeSalesBreakdowns(ordersInRange, products),
    };
  }, [orders, products, range.from, range.to]);

  const pending = stats.statusCounts.find((c) => c.status === 'Pending')?.count ?? 0;
  const shipped = stats.statusCounts.find((c) => c.status === 'Shipped')?.count ?? 0;

  return (
    <div className="dashboard-page">
      <header className="dashboard-page-header">
        <div>
          <h1>Műszerfal</h1>
          <p className="dashboard-page-subtitle">
            Csak a kiszállított rendelések számítanak eladásnak.
          </p>
        </div>
        <DateRangeMenu value={range} onChange={setRange} />
      </header>

      {loading ? (
        <div className="text-center py-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Műszerfal betöltése...</span>
          </Spinner>
        </div>
      ) : error ? (
        <Alert variant="danger">{error}</Alert>
      ) : (
        <>
          <section className="dashboard-kpis" aria-label="Fő mutatók">
            <KpiCard
              label="Bevétel"
              value={priceFormatter.format(stats.revenue)}
              sub={`${stats.deliveredCount} kiszállított rendelésből`}
            />
            <KpiCard
              label="Rendelések"
              value={String(stats.orderCount)}
              sub={`${pending} függőben · ${shipped} szállítás alatt`}
            />
            <KpiCard
              label="Átlagos kosárérték"
              value={stats.averageBasket === null ? '–' : priceFormatter.format(stats.averageBasket)}
              sub="kiszállított rendelésenként"
            />
            <KpiCard
              label="Készletriasztás"
              value={`${stats.outOfStockCount + stats.lowStockCount} termék`}
              sub={`${stats.outOfStockCount} elfogyott · ${stats.lowStockCount} alacsony`}
              tone={stats.outOfStockCount + stats.lowStockCount > 0 ? 'warning' : 'default'}
            />
          </section>

          <div className="dashboard-row-main">
            <ProductSalesPanel products={products} productsLoading={loading} range={range} />
            <OrderStatusChart counts={stats.statusCounts} />
          </div>

          <SalesBreakdownCard breakdowns={breakdowns} />

          <div className="dashboard-row-bottom">
            <RecentOrdersCard orders={stats.recentOrders} />
            <StockAlertsCard alerts={stats.stockAlerts} />
          </div>
        </>
      )}
    </div>
  );
}
