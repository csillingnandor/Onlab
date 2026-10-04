import type { OrderStatus } from '../../util/Types.js';
import './OrderStatusBadge.css';

export const statusLabels: Record<OrderStatus, string> = {
  Pending: 'Függőben',
  Shipped: 'Szállítás alatt',
  Delivered: 'Kézbesítve',
  Cancelled: 'Törölve',
};

export default function OrderStatusBadge({ status }: { status: OrderStatus }) {
  return (
    <span className={`order-status order-status-${status.toLowerCase()}`}>
      {statusLabels[status]}
    </span>
  );
}
