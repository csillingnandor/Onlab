import { useEffect, useMemo, useState } from 'react';
import productService from '../../services/productService.js';
import type { Product, ViewMode } from '../../util/Types.js';
import { countActive } from '../../common/filter/filterUtils.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import { emptyProductFilter, filterProducts, type ProductFilter } from '../../util/productFilter.js';
import ProductFormModal from './ProductFormModal.js';
import ProductFilterPanel from './ProductFilterPanel.js';
import ProductHeader from './ProductHeader.js';
import { useAuth } from '../../auth/useAuth.js';
import ProductStockModal from './ProductStockModal.js';
import ProductTable from './ProductTable.js';
import ProductImportModal from './ProductImportModal.js';
import { useQueryFlag } from '../../util/useQueryFlag.js';
import type { ProductImportResult } from '../../util/Types.js';

const FILTER_PANEL_ID = 'product-filter';

export default function ProductPage() {
  const { hasAction } = useAuth();
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
  const [importOpen, setImportOpen] = useState<boolean>(false);
  const [importMessage, setImportMessage] = useState<string | null>(null);
  const [newRequested, consumeNew] = useQueryFlag('new');
  const [importRequested, consumeImport] = useQueryFlag('import');

  // Kliensoldali szűrés a betöltött listán; lista és rács nézetre egyaránt érvényes
  const filteredProducts = useMemo(() => filterProducts(products, filter), [products, filter]);
  const categories = useMemo(
    () => [...new Set(products.map((p) => p.category).filter(Boolean))].sort((a, b) => a.localeCompare(b, 'hu')),
    [products],
  );
  const activeFilterCount = countActive(filter);

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

  // A műszerfal gyorsgombjai: ?new=1 az új termék, ?import=1 az importálás ablakát nyitja
  useEffect(() => {
    if (!newRequested) return;
    if (hasAction('products.create')) {
      setEditingProduct(null);
      setFormOpen(true);
    }
    consumeNew();
  }, [newRequested, consumeNew, hasAction]);

  useEffect(() => {
    if (!importRequested) return;
    if (hasAction('products.import')) setImportOpen(true);
    consumeImport();
  }, [importRequested, consumeImport, hasAction]);

  // Import után a teljes lista újratöltése (új és frissített termékek is lehetnek)
  const handleImported = async (result: ProductImportResult) => {
    setImportOpen(false);
    setImportMessage(`Importálás kész: ${result.created} új, ${result.updated} frissített termék.`);
    setProducts(await productService.getAll());
  };

  // Korrekció után a lista és a nyitott készletablak is a friss terméket mutatja
  const handleAdjusted = (updated: Product) => {
    setProducts((prev) => prev.map((p) => (p.id === updated.id ? updated : p)));
    setStockProduct(updated);
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

  if (loading) return <div className="p-4">Termékek betöltése...</div>;

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
        onAddProductClick={hasAction('products.create') ? openCreateForm : undefined}
        onImportClick={hasAction('products.import') ? () => setImportOpen(true) : undefined}
      />
      {importMessage && (
        <div className="alert alert-success alert-dismissible mb-0" role="status">
          {importMessage}
          <button type="button" className="btn-close" aria-label="Bezárás" onClick={() => setImportMessage(null)}></button>
        </div>
      )}
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
          onEdit={hasAction('products.edit') ? openEditForm : undefined}
          onDelete={hasAction('products.delete') ? setDeletingProduct : undefined}
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
        onAdjusted={hasAction('products.adjust-stock') ? handleAdjusted : undefined}
      />
      <ProductImportModal
        show={importOpen}
        onImported={handleImported}
        onHide={() => setImportOpen(false)}
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