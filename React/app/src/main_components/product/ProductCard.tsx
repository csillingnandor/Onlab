import type { Product } from '../../util/Types.js'
import ActionButtons from '../../common/button/ActionButtons.js';
import ProductStatusBadge from './ProductStatusBadge.js';
import { stockAction } from './stockAction.js';
import { priceFormatter } from '../../util/format.js';
import './ProductCard.css';

type ProductCardProps = {
    product: Product;
    onEdit: (product: Product) => void;
    onDelete: (product: Product) => void;
    onStock: (product: Product) => void;
};

export default function ProductCard({ product, onEdit, onDelete, onStock }: ProductCardProps) {
    return (
        <article className="product-card">
            <div className="product-card-image">
                <i className="bi bi-box-seam" aria-hidden="true"></i>
                <ProductStatusBadge status={product.status} className="product-card-status" />
                <ActionButtons
                    itemLabel={product.name}
                    onEdit={() => onEdit(product)}
                    onDelete={() => onDelete(product)}
                    extraActions={[stockAction(product, onStock)]}
                    className="product-card-actions"
                />
            </div>

            <div className="product-card-body">
                <span className="product-card-category">{product.category}</span>
                <h2 className="product-card-name" title={product.name}>{product.name}</h2>
                <p className="product-card-sku">SKU: {product.sku}</p>
            </div>

            <div className="product-card-footer">
                <span className="product-card-price">{priceFormatter.format(product.price)}</span>
                <span className="product-card-stock">
                    <i className="bi bi-stack" aria-hidden="true"></i>
                    {product.stockQuantity} db
                </span>
            </div>
        </article>
    )
}
