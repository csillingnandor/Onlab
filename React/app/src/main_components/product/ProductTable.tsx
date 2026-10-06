import type { Product, ViewMode } from '../../util/Types.js';
import ProductCard from './ProductCard.js';
import ProductStatusBadge from './ProductStatusBadge.js';
import { priceFormatter } from '../../util/format.js';
import '../../DataTable.css';

type ProductTableProps = {
  products: Product[];
  viewMode: ViewMode;
};

export default function ProductTable({ products, viewMode }: ProductTableProps) {
  if (!products || products.length === 0) {
    return (
      <div className="alert alert-info text-center my-4" role="alert">
        No products found.
      </div>
    );
  }

  if (viewMode === 'grid') {
    return (
      <div className="container-fluid p-0">
        <div className="row g-3 product-grid">
          {products.map((product) => (
            <div key={product.id} className="col-12 col-md-6 col-lg-4 col-xl-3">
              <ProductCard product={product} />
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="data-table-wrapper">
      <table className="data-table">
        <thead>
          <tr>
            <th>Név</th>
            <th>SKU</th>
            <th>Kategória</th>
            <th className="numeric">Készlet</th>
            <th className="numeric">Ár</th>
            <th>Állapot</th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td className="data-table-strong">{product.name}</td>
              <td className="data-table-mono">{product.sku}</td>
              <td>{product.category}</td>
              <td
                className={`numeric ${product.stockQuantity <= product.minStockLevel ? 'data-table-warning' : ''}`}
              >
                {product.stockQuantity} db
              </td>
              <td className="numeric">{priceFormatter.format(product.price)}</td>
              <td>
                <ProductStatusBadge status={product.status} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
