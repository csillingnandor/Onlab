import { Card } from 'react-bootstrap';
import type { OrderStatus } from '../../util/Types.js';
import type { StatusCount } from '../../util/dashboardStats.js';
import { statusLabels } from '../order/OrderStatusBadge.js';

// Ugyanazok a színek, mint a rendelés státuszjelvényein (common/badge/StatusBadge.css)
const statusColors: Record<OrderStatus, string> = {
  Pending: '#ffc107',
  Shipped: '#0d6efd',
  Delivered: '#198754',
  Cancelled: '#6c757d',
};

type OrderStatusChartProps = {
  counts: StatusCount[];
};

// Fánkdiagram conic-gradienttel: nem kell hozzá grafikon-könyvtár
export default function OrderStatusChart({ counts }: OrderStatusChartProps) {
  const total = counts.reduce((sum, c) => sum + c.count, 0);

  let start = 0;
  const segments = counts
    .filter((c) => c.count > 0)
    .map((c) => {
      const end = start + (c.count / total) * 100;
      const segment = `${statusColors[c.status]} ${start}% ${end}%`;
      start = end;
      return segment;
    });
  const background = total > 0 ? `conic-gradient(${segments.join(', ')})` : 'var(--bs-border-color)';

  return (
    <Card className="dashboard-card h-100">
      <Card.Header className="dashboard-card-header">
        <span className="dashboard-card-title">Rendelések státusz szerint</span>
      </Card.Header>
      <Card.Body className="d-flex flex-column gap-4">
        <div className="status-donut" style={{ background }} role="img" aria-label={`${total} rendelés az időszakban`}>
          <div className="status-donut-hole">
            <span className="status-donut-total">{total}</span>
            <span className="status-donut-caption">rendelés</span>
          </div>
        </div>

        <ul className="status-legend">
          {counts.map((c) => (
            <li key={c.status}>
              <span className="status-legend-swatch" style={{ background: statusColors[c.status] }}></span>
              <span className="status-legend-label">{statusLabels[c.status]}</span>
              <strong>{c.count}</strong>
            </li>
          ))}
        </ul>
      </Card.Body>
    </Card>
  );
}
