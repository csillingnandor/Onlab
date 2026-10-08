import { useEffect, useMemo, useState } from 'react';
import orderService from '../../services/orderService.js';
import customerService from '../../services/customerService.js';
import type { CustomerOrder, NewOrderItem } from '../../util/Types.js';
import { countActiveFilters, emptyOrderFilter, filterOrders, type OrderFilter } from '../../util/orderFilter.js';
import CreateOrderModal from './CreateOrderModal.js';
import OrderAccordionItem from './OrderAccordionItem.js';
import OrderFilterPanel from './OrderFilterPanel.js';
import FilterToggle from '../../common/filter/FilterToggle.js';
import '../../common/table/DataTable.css';
import './CustomerOrderPage.css';

// Stabil referencia, hogy a modal ne töltse újra a listát minden rendereléskor
const loadCustomers = () => customerService.getAll();

const FILTER_PANEL_ID = 'customer-order-filter';

export default function CustomerOrderPage() {
  const [orders, setOrders] = useState<CustomerOrder[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [openIds, setOpenIds] = useState<Set<number>>(new Set());
  const [filter, setFilter] = useState<OrderFilter>(emptyOrderFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);
  const [createOpen, setCreateOpen] = useState<boolean>(false);

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

  // Kliensoldali szűrés a betöltött listán
  const visibleOrders = useMemo(
    () => filterOrders(orders, filter, (order) => order.customerName),
    [orders, filter],
  );
  const activeFilterCount = countActiveFilters(filter);

  const toggleOrder = (id: number) => {
    setOpenIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  // Az „Összes kinyitása” csak a szűrés után látható rendelésekre vonatkozik
  const allOpen = visibleOrders.length > 0 && visibleOrders.every((o) => openIds.has(o.id));
  const toggleAll = () => {
    setOpenIds((prev) => {
      const next = new Set(prev);
      visibleOrders.forEach((o) => (allOpen ? next.delete(o.id) : next.add(o.id)));
      return next;
    });
  };

  // Az új rendelés a lista elejére kerül (legújabb elöl), kinyitva
  const handleCreate = async (partyId: number, items: NewOrderItem[]) => {
    const created = await orderService.create(partyId, items);
    setOrders((prev) => [created, ...prev]);
    setOpenIds((prev) => new Set(prev).add(created.id));
    setCreateOpen(false);
  };

  const itemCount = visibleOrders.reduce((sum, order) => sum + order.items.length, 0);

  if (loading) return <div className="p-4 text-white">Rendelések betöltése...</div>;

  return (
    <div className="order-page">
      <header className="order-page-header">
        <div>
          <h1>Vevői rendelések</h1>
          <p className="order-page-subtitle">
            {activeFilterCount > 0 ? `${visibleOrders.length} / ${orders.length}` : orders.length} rendelés,{' '}
            {itemCount} tétel
          </p>
        </div>

        <div className="order-header-actions">
          <FilterToggle
            open={filterOpen}
            activeCount={activeFilterCount}
            controls={FILTER_PANEL_ID}
            onToggle={() => setFilterOpen((prev) => !prev)}
          />
          <button
            type="button"
            className="order-expand-all"
            onClick={toggleAll}
            disabled={visibleOrders.length === 0}
          >
            <i className={`bi ${allOpen ? 'bi-arrows-collapse' : 'bi-arrows-expand'}`} aria-hidden="true"></i>
            {allOpen ? 'Összes becsukása' : 'Összes kinyitása'}
          </button>
          <button type="button" className="add-product-btn" onClick={() => setCreateOpen(true)}>
            <i className="bi bi-plus-lg me-1" aria-hidden="true"></i>
            Új rendelés
          </button>
        </div>
      </header>

      {filterOpen && (
        <OrderFilterPanel
          id={FILTER_PANEL_ID}
          value={filter}
          onChange={setFilter}
          onClear={() => setFilter(emptyOrderFilter)}
          nameLabel="Vevő"
        />
      )}

      <CreateOrderModal
        show={createOpen}
        title="Új vevői rendelés"
        partyLabel="Vevő"
        loadParties={loadCustomers}
        priceMode="catalog"
        onSubmit={handleCreate}
        onHide={() => setCreateOpen(false)}
      />

      {error ? (
        <div className="alert alert-danger" role="alert">{error}</div>
      ) : orders.length === 0 ? (
        <div className="alert alert-info text-center" role="alert">Nincsenek rendelések.</div>
      ) : visibleOrders.length === 0 ? (
        <div className="alert alert-info text-center" role="status">Nincs a szűrésnek megfelelő rendelés.</div>
      ) : (
        <div className="order-accordion-list">
          {visibleOrders.map((order) => (
            <OrderAccordionItem
              key={order.id}
              id={order.id}
              partyName={order.customerName}
              orderDate={order.orderDate}
              status={order.status}
              totalAmount={order.totalAmount}
              items={order.items.map((item) => ({
                id: item.id,
                productName: item.productName,
                unitPrice: item.productPrice,
                quantity: item.quantity,
              }))}
              isOpen={openIds.has(order.id)}
              onToggle={() => toggleOrder(order.id)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
