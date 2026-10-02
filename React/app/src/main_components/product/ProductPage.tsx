import { useEffect, useState } from 'react';
import productService from '../../services/productService.js';
import type { Product, ViewMode } from '../../util/Types.js';
import AddProductForm from './AddProductForm.js';
import ProductHeader from './ProductHeader.js';
import ProductTable from './ProductTable.js';

export default function ProductPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [formOpen, setFormOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(true);
  const [viewMode, setViewMode] = useState<ViewMode>('list');

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
            viewMode={viewMode}
            onViewModeChange={setViewMode}
            onAddProductClick={() => setFormOpen(true)}
          />
          <ProductTable products={products} viewMode={viewMode} />
        </>
      )}
    </div>
  );
}