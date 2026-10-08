import StatusBadge, { type StatusTone } from '../../common/badge/StatusBadge.js';
import type { OrderStatus } from '../../util/Types.js';

export const statusLabels: Record<OrderStatus, string> = {
  Pending: 'Függőben',
  Shipped: 'Szállítás alatt',
  Delivered: 'Kézbesítve',
  Cancelled: 'Törölve',
};

const statusTones: Record<OrderStatus, StatusTone> = {
  Pending: 'warning',
  Shipped: 'info',
  Delivered: 'success',
  Cancelled: 'neutral',
};

export default function OrderStatusBadge({ status }: { status: OrderStatus }) {
  return <StatusBadge label={statusLabels[status]} tone={statusTones[status]} />;
}
