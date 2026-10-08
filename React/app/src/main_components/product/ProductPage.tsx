import { useEffect, useMemo, useState } from 'react';
import productService from '../../services/productService.js';
import type { Product, ViewMode } from '../../util/Types.js';
import { countActive } from '../../common/filter/filterUtils.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import { emptyProductFilter, filterProducts, type ProductFilter } from '../../util/productFilter.js';
import ProductFormModal from './ProductFormModal.js';
import ProductFilterPanel from './ProductFilterPanel.js';
import ProductHeader from './ProductHeader.js';
import ProductStockModal from './ProductStockModal.js';
import ProductTable from './ProductTable.js';

const FILTER_PANEL_ID = 'product-filter';

export default function ProductPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [formOpen, setFormOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(true);
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  const [filter, setFilter] = useState<ProductFilter>(emptyProductFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);
  // A formOpen mellett: null = új termék, egyébként a módosított termék
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [deletingProduct, setDeletingProduct] = useState<Product | null>(null);
  const [stockProduct, setStockProduct] = useState<Product | null>(null);

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

  const openCreateForm = () => {
    setEditingProduct(null);
    setFormOpen(true);
  };

  const openEditForm = (product: Product) => {
    setEditingProduct(product);
    setFormOpen(true);
  };

  // A mentett termék a backendről jön (frissen számolt állapottal): módosításnál cseréljük, újnál hozzáadjuk
  const handleProductSaved = (saved: Product) => {
    setProducts((prevProducts) =>
      prevProducts.some((p) => p.id === saved.id)
        ? prevProducts.map((p) => (p.id === saved.id ? saved : p))
        : [...prevProducts, saved],
    );
    setFormOpen(false);
  };

  // Ha a backend elutasítja (pl. 409: rendelés hivatkozik rá), a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmDelete = async () => {
    if (!deletingProduct) return;
    await productService.delete(deletingProduct.id);
    setProducts((prevProducts) => prevProducts.filter((p) => p.id !== deletingProduct.id));
    setDeletingProduct(null);
  };

  if (loading) return <div className="p-4 text-white">Termékek betöltése...</div>;

  return (
    <div className="product-page">
      <ProductHeader
        subtitle={`${activeFilterCount > 0 ? `${filteredProducts.length} / ${products.length}` : products.length} termék`}
        viewMode={viewMode}
        onViewModeChange={setViewMode}
        filterOpen={filterOpen}
        activeFilterCount={activeFilterCount}
        filterPanelId={FILTER_PANEL_ID}
        onFilterToggle={() => setFilterOpen((prev) => !prev)}
        onAddProductClick={openCreateForm}
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
        <ProductTable
          products={filteredProducts}
          viewMode={viewMode}
          onEdit={openEditForm}
          onDelete={setDeletingProduct}
          onStock={setStockProduct}
        />
      )}

      <ProductFormModal
        show={formOpen}
        product={editingProduct}
        categories={categories}
        onSaved={handleProductSaved}
        onHide={() => setFormOpen(false)}
      />
      <ProductStockModal
        product={stockProduct}
        onHide={() => setStockProduct(null)}
      />
      <ConfirmModal
        show={deletingProduct !== null}
        title="Termék törlése"
        confirmLabel="Törlés"
        confirmIcon="bi-trash"
        busyLabel="Törlés..."
        onConfirm={handleConfirmDelete}
        onHide={() => setDeletingProduct(null)}
      >
        {deletingProduct && (
          <>
            <p className="mb-2">Biztosan törlöd ezt a terméket?</p>
            <p className="mb-2">
              <strong>{deletingProduct.name}</strong>
              <span className="text-secondary font-monospace ms-2">{deletingProduct.sku}</span>
            </p>
            <p className="mb-0 small text-secondary">A művelet nem vonható vissza.</p>
          </>
        )}
      </ConfirmModal>
    </div>
  );
}