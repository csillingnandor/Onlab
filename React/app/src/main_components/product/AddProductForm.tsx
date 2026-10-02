import React, { useState } from 'react';
import productService from '../../services/productService.js'; // Igazítsd a saját elérési utadhoz
import type { Product } from '../../util/Types.js';

type AddProductFormProps = {
  onProductAdded: (newProduct: Product) => void;
  onCancel: () => void;
};

export default function AddProductForm({ onProductAdded, onCancel }: AddProductFormProps) {
  const [formData, setFormData] = useState<Omit<Product, 'id'>>({
    name: '',
    sku: '',
    category: 'Perifériák',
    stockQuantity: 0,
    minStockLevel: 5,
    price: 0,
    status: 'In Stock',
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const createdProduct = await productService.create(formData);
      
      onProductAdded(createdProduct);
    } catch (err: any) {
      setError(err.message || 'Hiba történt a mentés során.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="p-3 border rounded bg-dark text-white">
      {error && <div className="alert alert-danger">{error}</div>}
      
      <div className="mb-3">
        <label className="form-label">Termék neve</label>
        <input
          type="text"
          className="form-control"
          value={formData.name}
          onChange={(e) => setFormData({ ...formData, name: e.target.value })}
          required
        />
      </div>

      <div className="mb-3">
        <label className="form-label">SKU</label>
        <input
          type="text"
          className="form-control"
          value={formData.sku}
          onChange={(e) => setFormData({ ...formData, sku: e.target.value })}
          required
        />
      </div>

      <div className="mb-3">
        <label className="form-label">Kategória</label>
        <select
          className="form-control"
          value={formData.category}
          onChange={(e) => setFormData({ ...formData, category: e.target.value })}
          required
        >
          <option value="Perifériák">Perifériák</option>
          <option value="Tárolóeszközök">Tárolóeszközök</option>
          <option value="Hálózati eszközök">Hálózati eszközök</option>
        </select>
      </div>


      <div className="d-flex gap-2">
        <button type="button" className="btn btn-secondary" onClick={onCancel} disabled={loading}>
          Mégse
        </button>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Mentés...' : 'Termék mentése'}
        </button>
      </div>
    </form>
  );
}