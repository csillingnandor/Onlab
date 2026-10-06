import { useEffect, useMemo, useState } from 'react';
import productService from '../../services/productService.js';
import type { Product, ViewMode } from '../../util/Types.js';
import { countActive } from '../../util/filterUtils.js';
import { emptyProductFilter, filterProducts, type ProductFilter } from '../../util/productFilter.js';
import AddProductForm from './AddProductForm.js';
import ProductFilterPanel from './ProductFilterPanel.js';
import ProductHeader from './ProductHeader.js';
import ProductTable from './ProductTable.js';

const FILTER_PANEL_ID = 'product-filter';

export default function ProductPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [formOpen, setFormOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(true);
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  const [filter, setFilter] = useState<ProductFilter>(emptyProductFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);

  // Kliensoldali szűrés a betöltött listán; lista és rács nézetre egyaránt érvényes
  const filteredProducts = useMemo(() => filterProducts(products, filter), [products, filter]);
  const categories = useMemo(
    () => [...new Set(products.map((p) => p.category).filter(Boolean))].sort((a, b) => a.localeCompare(b, 'hu')),
    [products],
  );
  const activeFilterCount = countActive(filter);

  // 1. Kezdeti adatbetöltés a backendről
  useEffect(() => {
    productService.getAll()
      .then((data) => {
        setProducts(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setLoading(false);
      });
  }, []);

  const handleProductAdded = (newProduct: Product) => {
    setProducts((prevProducts) => [...prevProducts, newProduct]);
    setFormOpen(false);
  };

  if (loading) return <div className="p-4 text-white">Termékek betöltése...</div>;

  return (
    <div className="product-page">
      {formOpen ? (
        <AddProductForm
          onProductAdded={handleProductAdded}
          onCancel={() => setFormOpen(false)}
        />
      ) : (
        <>
          <ProductHeader
            subtitle={`${activeFilterCount > 0 ? `${filteredProducts.length} / ${products.length}` : products.length} termék`}
            viewMode={viewMode}
            onViewModeChange={setViewMode}
            filterOpen={filterOpen}
            activeFilterCount={activeFilterCount}
            filterPanelId={FILTER_PANEL_ID}
            onFilterToggle={() => setFilterOpen((prev) => !prev)}
            onAddProductClick={() => setFormOpen(true)}
          />
          {filterOpen && (
            <ProductFilterPanel
              id={FILTER_PANEL_ID}
              value={filter}
              categories={categories}
              onChange={setFilter}
              onClear={() => setFilter(emptyProductFilter)}
            />
          )}
          {products.length > 0 && filteredProducts.length === 0 ? (
            <div className="alert alert-info text-center" role="status">Nincs a szűrésnek megfelelő termék.</div>
          ) : (
            <ProductTable products={filteredProducts} viewMode={viewMode} />
          )}
        </>
      )}
    </div>
  );
}