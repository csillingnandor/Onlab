import { useEffect, useState } from 'react';
import orderService from '../../services/orderService.js';
import type { CustomerOrder } from '../../util/Types.js';
import OrderAccordionItem from './OrderAccordionItem.js';
import '../../DataTable.css';
import './CustomerOrderPage.css';

export default function CustomerOrderPage() {
  const [orders, setOrders] = useState<CustomerOrder[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [openIds, setOpenIds] = useState<Set<number>>(new Set());

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    orderService.getAll()
      .then((data) => {
        if (!ignore) setOrders(data);
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

  const toggleOrder = (id: number) => {
    setOpenIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const allOpen = orders.length > 0 && openIds.size === orders.length;
  const toggleAll = () => {
    setOpenIds(allOpen ? new Set() : new Set(orders.map((o) => o.id)));
  };

  const itemCount = orders.reduce((sum, order) => sum + order.items.length, 0);

  if (loading) return <div className="p-4 text-white">Rendelések betöltése...</div>;

  return (
    <div className="order-page">
      <header className="order-page-header">
        <div>
          <h1>Vevői rendelések</h1>
          <p className="order-page-subtitle">
            {orders.length} rendelés, {itemCount} tétel
          </p>
        </div>

        <button
          type="button"
          className="order-expand-all"
          onClick={toggleAll}
          disabled={orders.length === 0}
        >
          <i className={`bi ${allOpen ? 'bi-arrows-collapse' : 'bi-arrows-expand'}`} aria-hidden="true"></i>
          {allOpen ? 'Összes becsukása' : 'Összes kinyitása'}
        </button>
      </header>

      {error ? (
        <div className="alert alert-danger" role="alert">{error}</div>
      ) : orders.length === 0 ? (
        <div className="alert alert-info text-center" role="alert">Nincsenek rendelések.</div>
      ) : (
        <div className="order-accordion-list">
          {orders.map((order) => (
            <OrderAccordionItem
              key={order.id}
              order={order}
              isOpen={openIds.has(order.id)}
              onToggle={() => toggleOrder(order.id)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
