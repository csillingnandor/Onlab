import type { OrderStatus } from '../../util/Types.js';
import { dateTimeFormatter, priceFormatter } from '../../util/format.js';
import OrderStatusBadge from './OrderStatusBadge.js';

// Közös tételalak, hogy a vevői és a beszerzési rendelés is ugyanazt a komponenst használja
export type OrderLine = {
  id: number;
  productName: string;
  unitPrice: number;
  quantity: number;
};

type OrderAccordionItemProps = {
  id: number;
  partyName: string; // vevő vagy beszállító neve
  orderDate: string;
  status: OrderStatus;
  totalAmount: number;
  items: OrderLine[];
  isOpen: boolean;
  onToggle: () => void;
};

export default function OrderAccordionItem({
  id,
  partyName,
  orderDate,
  status,
  totalAmount,
  items,
  isOpen,
  onToggle,
}: OrderAccordionItemProps) {
  const panelId = `order-panel-${id}`;
  const totalQuantity = items.reduce((sum, item) => sum + item.quantity, 0);

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
        <span className="order-accordion-id">#{id}</span>
        <span className="order-accordion-customer">
          <span className="order-accordion-customer-name">{partyName}</span>
          <span className="order-accordion-date">{dateTimeFormatter.format(new Date(orderDate))}</span>
        </span>
        <span className="order-accordion-count">{items.length} tétel</span>
        <OrderStatusBadge status={status} />
        <span className="order-accordion-total">{priceFormatter.format(totalAmount)}</span>
      </button>

      {/* inert: becsukva a tartalom ne legyen elérhető billentyűzettel / képernyőolvasóval */}
      <div id={panelId} className="order-accordion-collapse" inert={!isOpen}>
        <div className="order-accordion-body">
          {items.length === 0 ? (
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
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td className="data-table-strong">{item.productName}</td>
                      <td className="numeric">{priceFormatter.format(item.unitPrice)}</td>
                      <td className="numeric">{item.quantity} db</td>
                      <td className="numeric">{priceFormatter.format(item.unitPrice * item.quantity)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <td colSpan={2}>Összesen</td>
                    <td className="numeric">{totalQuantity} db</td>
                    <td className="numeric">{priceFormatter.format(totalAmount)}</td>
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
