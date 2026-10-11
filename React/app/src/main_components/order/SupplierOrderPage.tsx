import { useEffect, useMemo, useState } from 'react';
import { Button } from 'react-bootstrap';
import supplierOrderService from '../../services/supplierOrderService.js';
import type { NewOrderItem, SupplierOrder } from '../../util/Types.js';
import { countActiveFilters, emptyOrderFilter, filterOrders, type OrderFilter } from '../../util/orderFilter.js';
import CreateOrderModal from './CreateOrderModal.js';
import OrderAccordionItem from './OrderAccordionItem.js';
import OrderFilterPanel from './OrderFilterPanel.js';
import FilterToggle from '../../common/filter/FilterToggle.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import '../../common/table/DataTable.css';
import './CustomerOrderPage.css';

// Stabil referencia, hogy a modal ne töltse újra a listát minden rendereléskor
const loadSuppliers = () => supplierOrderService.getSuppliers();

const FILTER_PANEL_ID = 'supplier-order-filter';

// A vevői rendelések oldalának párja: ugyanaz az elrendezés és harmonika, beszállítóval és beszerzési árral.
export default function SupplierOrderPage() {
  const [orders, setOrders] = useState<SupplierOrder[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [openIds, setOpenIds] = useState<Set<number>>(new Set());
  const [filter, setFilter] = useState<OrderFilter>(emptyOrderFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);
  const [createOpen, setCreateOpen] = useState<boolean>(false);
  const [receivingOrder, setReceivingOrder] = useState<SupplierOrder | null>(null);

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    supplierOrderService.getAll()
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
    () => filterOrders(orders, filter, (order) => order.supplierName),
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
    const created = await supplierOrderService.create(partyId, warehouseId!, items);
    setOrders((prev) => [created, ...prev]);
    setOpenIds((prev) => new Set(prev).add(created.id));
    setCreateOpen(false);
  };

  // Csak a még meg nem érkezett (függő vagy úton lévő) rendelés érkeztethető be
  const canReceive = (order: SupplierOrder) => order.status === 'Pending' || order.status === 'Shipped';

  // Ha a backend elutasítja (pl. 409: már beérkezett), a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmReceive = async () => {
    if (!receivingOrder) return;
    const received = await supplierOrderService.receive(receivingOrder.id);
    setOrders((prev) => prev.map((o) => (o.id === received.id ? received : o)));
    setReceivingOrder(null);
  };

  const itemCount = visibleOrders.reduce((sum, order) => sum + order.items.length, 0);

  if (loading) return <div className="p-4 text-white">Beszerzési rendelések betöltése...</div>;

  return (
    <div className="order-page">
      <header className="order-page-header">
        <div>
          <h1>Beszerzési rendelések</h1>
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
          nameLabel="Beszállító"
        />
      )}

      <CreateOrderModal
        show={createOpen}
        title="Új beszerzési rendelés"
        partyLabel="Beszállító"
        loadParties={loadSuppliers}
        priceMode="manual"
        warehouseLabel="Célraktár"
        onSubmit={handleCreate}
        onHide={() => setCreateOpen(false)}
      />

      {error ? (
        <div className="alert alert-danger" role="alert">{error}</div>
      ) : orders.length === 0 ? (
        <div className="alert alert-info text-center" role="alert">Nincsenek beszerzési rendelések.</div>
      ) : visibleOrders.length === 0 ? (
        <div className="alert alert-info text-center" role="status">Nincs a szűrésnek megfelelő rendelés.</div>
      ) : (
        <div className="order-accordion-list">
          {visibleOrders.map((order) => (
            <OrderAccordionItem
              key={order.id}
              id={order.id}
              partyName={order.supplierName}
              warehouseName={order.warehouseName}
              orderDate={order.orderDate}
              status={order.status}
              totalAmount={order.totalCost}
              items={order.items.map((item) => ({
                id: item.id,
                productName: item.productName,
                unitPrice: item.unitCost,
                quantity: item.quantity,
              }))}
              isOpen={openIds.has(order.id)}
              onToggle={() => toggleOrder(order.id)}
              actions={
                canReceive(order) && (
                  <Button variant="success" size="sm" onClick={() => setReceivingOrder(order)}>
                    <i className="bi bi-box-arrow-in-down me-1" aria-hidden="true"></i>
                    Beérkeztetés
                  </Button>
                )
              }
            />
          ))}
        </div>
      )}

      <ConfirmModal
        show={receivingOrder !== null}
        title="Rendelés beérkeztetése"
        confirmLabel="Beérkeztetés"
        confirmIcon="bi-box-arrow-in-down"
        busyLabel="Beérkeztetés..."
        variant="primary"
        onConfirm={handleConfirmReceive}
        onHide={() => setReceivingOrder(null)}
      >
        {receivingOrder && (
          <>
            <p className="mb-2">
              A <strong>#{receivingOrder.id}</strong> rendelés ({receivingOrder.supplierName}) tételei a{' '}
              <strong>{receivingOrder.warehouseName}</strong> készletéhez adódnak:
            </p>
            <ul className="mb-2">
              {receivingOrder.items.map((item) => (
                <li key={item.id}>
                  {item.productName}: <strong>+{item.quantity} db</strong>
                </li>
              ))}
            </ul>
            <p className="mb-0 small text-secondary">A rendelés Kézbesítve státuszba kerül; a művelet nem vonható vissza.</p>
          </>
        )}
      </ConfirmModal>
    </div>
  );
}
