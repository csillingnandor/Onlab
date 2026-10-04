import { Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import type { CustomerOrder } from '../../util/Types.js';
import { dateFormatter, priceFormatter } from '../../util/format.js';
import OrderStatusBadge from '../order/OrderStatusBadge.js';
import '../../DataTable.css';

type RecentOrdersCardProps = {
  orders: CustomerOrder[];
};

export default function RecentOrdersCard({ orders }: RecentOrdersCardProps) {
  return (
    <Card className="dashboard-card h-100">
      <Card.Header className="dashboard-card-header">
        <span className="dashboard-card-title">Legutóbbi rendelések</span>
        <Link to="/customer-orders" className="dashboard-card-link">Összes rendelés</Link>
      </Card.Header>
      <Card.Body>
        {orders.length === 0 ? (
          <p className="dashboard-empty">Ebben az időszakban nem volt rendelés.</p>
        ) : (
          <div className="data-table-wrapper">
            <table className="data-table">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Vevő</th>
                  <th>Státusz</th>
                  <th className="numeric">Összeg</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order) => (
                  <tr key={order.id}>
                    <td className="data-table-mono">{order.id}</td>
                    <td>
                      <div className="data-table-strong">{order.customerName}</div>
                      <div className="dashboard-muted">{dateFormatter.format(new Date(order.orderDate))}</div>
                    </td>
                    <td><OrderStatusBadge status={order.status} /></td>
                    <td className="numeric">{priceFormatter.format(order.totalAmount)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Card.Body>
    </Card>
  );
}
