import type { Product } from '../../util/Types.js'
import ProductStatusBadge from './ProductStatusBadge.js';
import { priceFormatter } from '../../util/format.js';
import './ProductCard.css';

export default function ProductCard({ product }: { product: Product }) {
    return (
        <article className="product-card">
            <div className="product-card-image">
                <i className="bi bi-box-seam" aria-hidden="true"></i>
                <ProductStatusBadge status={product.status} className="product-card-status" />
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
