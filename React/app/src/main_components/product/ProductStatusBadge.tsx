import StatusBadge, { type StatusTone } from '../../common/badge/StatusBadge.js';
import type { Product } from '../../util/Types.js';

export const statusLabels: Record<Product['status'], string> = {
    'In Stock': 'Készleten',
    'Low Stock': 'Alacsony készlet',
    'Out of Stock': 'Elfogyott',
};

const statusTones: Record<Product['status'], StatusTone> = {
    'In Stock': 'success',
    'Low Stock': 'warning',
    'Out of Stock': 'danger',
};

type ProductStatusBadgeProps = {
    status: Product['status'];
    className?: string;
};

export default function ProductStatusBadge({ status, className }: ProductStatusBadgeProps) {
    return <StatusBadge label={statusLabels[status]} tone={statusTones[status]} className={className} />;
}
