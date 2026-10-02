import type { CustomerOrder } from '../../util/Types.js';
import { dateTimeFormatter, priceFormatter } from '../../util/format.js';
import OrderStatusBadge from './OrderStatusBadge.js';

type OrderAccordionItemProps = {
  order: CustomerOrder;
  isOpen: boolean;
  onToggle: () => void;
};

export default function OrderAccordionItem({ order, isOpen, onToggle }: OrderAccordionItemProps) {
  const panelId = `order-panel-${order.id}`;
  const totalQuantity = order.items.reduce((sum, item) => sum + item.quantity, 0);

  return (
    <section className={`order-accordion ${isOpen ? 'open' : ''}`}>
      <button
        type="button"
        className="order-accordion-toggle"
        onClick={onToggle}
        aria-expanded={isOpen}
        aria-controls={panelId}
      >
        <i className="bi bi-chevron-right order-accordion-chevron" aria-hidden="true"></i>
        <span className="order-accordion-id">#{order.id}</span>
        <span className="order-accordion-customer">
          <span className="order-accordion-customer-name">{order.customerName}</span>
          <span className="order-accordion-date">{dateTimeFormatter.format(new Date(order.orderDate))}</span>
        </span>
        <span className="order-accordion-count">{order.items.length} tétel</span>
        <OrderStatusBadge status={order.status} />
        <span className="order-accordion-total">{priceFormatter.format(order.totalAmount)}</span>
      </button>

      {/* inert: becsukva a tartalom ne legyen elérhető billentyűzettel / képernyőolvasóval */}
      <div id={panelId} className="order-accordion-collapse" inert={!isOpen}>
        <div className="order-accordion-body">
          {order.items.length === 0 ? (
            <p className="order-accordion-empty">Ennek a rendelésnek nincsenek tételei.</p>
          ) : (
            <div className="data-table-wrapper">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Termék</th>
                    <th className="numeric">Egységár</th>
                    <th className="numeric">Mennyiség</th>
                    <th className="numeric">Összesen</th>
                  </tr>
                </thead>
                <tbody>
                  {order.items.map((item) => (
                    <tr key={item.id}>
                      <td className="data-table-strong">{item.productName}</td>
                      <td className="numeric">{priceFormatter.format(item.productPrice)}</td>
                      <td className="numeric">{item.quantity} db</td>
                      <td className="numeric">{priceFormatter.format(item.productPrice * item.quantity)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <td colSpan={2}>Összesen</td>
                    <td className="numeric">{totalQuantity} db</td>
                    <td className="numeric">{priceFormatter.format(order.totalAmount)}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
