import type { Product } from '../../util/Types.js';
import './ProductStatusBadge.css';

export const statusLabels: Record<Product['status'], string> = {
    'In Stock': 'Készleten',
    'Low Stock': 'Alacsony készlet',
    'Out of Stock': 'Elfogyott',
};

const statusClasses: Record<Product['status'], string> = {
    'In Stock': 'in-stock',
    'Low Stock': 'low-stock',
    'Out of Stock': 'out-of-stock',
};

type ProductStatusBadgeProps = {
    status: Product['status'];
    className?: string;
};

export default function ProductStatusBadge({ status, className = '' }: ProductStatusBadgeProps) {
    return (
        <span className={`product-status ${statusClasses[status]} ${className}`}>
            {statusLabels[status]}
        </span>
    );
}
