import { Form } from 'react-bootstrap';
import type { Product } from '../../util/Types.js';

type ProductSelectProps = {
  products: Product[];
  value: number | null;
  onChange: (productId: number) => void;
  disabled?: boolean;
};

export default function ProductSelect({ products, value, onChange, disabled = false }: ProductSelectProps) {
  return (
    <Form.Select
      size="sm"
      className="product-select"
      aria-label="Termék kiválasztása"
      value={value ?? ''}
      onChange={(e) => onChange(Number(e.target.value))}
      disabled={disabled || products.length === 0}
    >
      {value === null && (
        <option value="" disabled>
          {products.length === 0 ? 'Nincs termék' : 'Válassz terméket…'}
        </option>
      )}
      {products.map((product) => (
        <option key={product.id} value={product.id}>
          {product.name} ({product.sku})
        </option>
      ))}
    </Form.Select>
  );
}
