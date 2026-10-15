import { useEffect, useMemo, useState } from 'react';
import { Button } from 'react-bootstrap';
import orderService, { type OrderAction } from '../../services/orderService.js';
import customerService from '../../services/customerService.js';
import type { CustomerOrder, NewOrderItem } from '../../util/Types.js';
import { countActiveFilters, emptyOrderFilter, filterOrders, type OrderFilter } from '../../util/orderFilter.js';
import CreateOrderModal from './CreateOrderModal.js';
import OrderAccordionItem from './OrderAccordionItem.js';
import OrderFilterPanel from './OrderFilterPanel.js';
import FilterToggle from '../../common/filter/FilterToggle.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import { useAuth } from '../../auth/useAuth.js';
import '../../common/table/DataTable.css';
import './CustomerOrderPage.css';

// Stabil referencia, hogy a modal ne töltse újra a listát minden rendereléskor
const loadCustomers = () => customerService.getAll();

const FILTER_PANEL_ID = 'customer-order-filter';

// A státuszváltó műveletek megjelenése (gomb és megerősítő modal)
const actionConfig: Record<OrderAction, {
  label: string;
  title: string;
  icon: string;
  buttonVariant: string;
  confirmVariant: 'primary' | 'danger';
  busyLabel: string;
}> = {
  ship: { label: 'Kiszállítás', title: 'Rendelés kiszállítása', icon: 'bi-truck', buttonVariant: 'primary', confirmVariant: 'primary', busyLabel: 'Kiszállítás...' },
  deliver: { label: 'Kézbesítve', title: 'Rendelés kézbesítése', icon: 'bi-check2-circle', buttonVariant: 'success', confirmVariant: 'primary', busyLabel: 'Mentés...' },
  cancel: { label: 'Lemondás', title: 'Rendelés lemondása', icon: 'bi-x-circle', buttonVariant: 'outline-danger', confirmVariant: 'danger', busyLabel: 'Lemondás...' },
};

// Állapotonként elérhető műveletek: függő -> kiszállítás / lemondás, szállítás alatt -> kézbesítve
const actionsFor = (order: CustomerOrder): OrderAction[] =>
  order.status === 'Pending' ? ['cancel', 'ship'] : order.status === 'Shipped' ? ['deliver'] : [];

export default function CustomerOrderPage() {
  const { hasAction } = useAuth();
  const [orders, setOrders] = useState<CustomerOrder[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [openIds, setOpenIds] = useState<Set<number>>(new Set());
  const [filter, setFilter] = useState<OrderFilter>(emptyOrderFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);
  const [createOpen, setCreateOpen] = useState<boolean>(false);
  const [pending, setPending] = useState<{ order: CustomerOrder; action: OrderAction } | null>(null);

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
  // A modal warehouseLabel-lel kötelezővé teszi a raktárt, így itt mindig van értéke
  const handleCreate = async (partyId: number, items: NewOrderItem[], warehouseId: number | null) => {
    const created = await orderService.create(partyId, warehouseId!, items);
    setOrders((prev) => [created, ...prev]);
    setOpenIds((prev) => new Set(prev).add(created.id));
    setCreateOpen(false);
  };

  // Ha a backend elutasítja (pl. 409: nincs elég készlet), a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmAction = async () => {
    if (!pending) return;
    const updated = await orderService.changeStatus(pending.order.id, pending.action);
    setOrders((prev) => prev.map((o) => (o.id === updated.id ? updated : o)));
    setPending(null);
  };

  const itemCount = visibleOrders.reduce((sum, order) => sum + order.items.length, 0);

  if (loading) return <div className="p-4">Rendelések betöltése...</div>;

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
            className="header-btn"
            onClick={toggleAll}
            disabled={visibleOrders.length === 0}
          >
            <i className={`bi ${allOpen ? 'bi-arrows-collapse' : 'bi-arrows-expand'}`} aria-hidden="true"></i>
            {allOpen ? 'Összes becsukása' : 'Összes kinyitása'}
          </button>
          {hasAction('customer-orders.create') && (
            <button type="button" className="header-btn header-btn-primary" onClick={() => setCreateOpen(true)}>
              <i className="bi bi-plus-lg" aria-hidden="true"></i>
              Új rendelés
            </button>
          )}
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
        warehouseLabel="Kiszolgáló raktár"
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
              warehouseName={order.warehouseName}
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
              actions={actionsFor(order).filter((action) => hasAction(`customer-orders.${action}` as const)).map((action) => (
                <Button
                  key={action}
                  variant={actionConfig[action].buttonVariant}
                  size="sm"
                  onClick={() => setPending({ order, action })}
                >
                  <i className={`bi ${actionConfig[action].icon} me-1`} aria-hidden="true"></i>
                  {actionConfig[action].label}
                </Button>
              ))}
            />
          ))}
        </div>
      )}

      <ConfirmModal
        show={pending !== null}
        title={pending ? actionConfig[pending.action].title : ''}
        confirmLabel={pending ? actionConfig[pending.action].label : ''}
        confirmIcon={pending ? actionConfig[pending.action].icon : 'bi-check'}
        busyLabel={pending ? actionConfig[pending.action].busyLabel : 'Folyamatban...'}
        variant={pending ? actionConfig[pending.action].confirmVariant : 'primary'}
        onConfirm={handleConfirmAction}
        onHide={() => setPending(null)}
      >
        {pending && (
          <>
            <p className="mb-2">
              <strong>#{pending.order.id}</strong> rendelés – {pending.order.customerName}
            </p>
            {pending.action === 'ship' && (
              <>
                <p className="mb-2">
                  A tételek a <strong>{pending.order.warehouseName}</strong> készletéből fogynak:
                </p>
                <ul className="mb-2">
                  {pending.order.items.map((item) => (
                    <li key={item.id}>
                      {item.productName}: <strong>−{item.quantity} db</strong>
                    </li>
                  ))}
                </ul>
                <p className="mb-0 small text-secondary">
                  Ha valamelyik termékből nincs elég, a kiszállítás nem történik meg. A kiszállított rendelés már nem mondható le.
                </p>
              </>
            )}
            {pending.action === 'deliver' && (
              <p className="mb-0">A rendelés Kézbesítve státuszba kerül, és ettől kezdve eladásként számít a statisztikákban.</p>
            )}
            {pending.action === 'cancel' && (
              <p className="mb-0">A rendelés Törölve státuszba kerül; a készlet nem változik. A művelet nem vonható vissza.</p>
            )}
          </>
        )}
      </ConfirmModal>
    </div>
  );
}
