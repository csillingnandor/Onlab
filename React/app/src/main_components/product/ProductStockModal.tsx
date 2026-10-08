import { Button, Modal, Table } from 'react-bootstrap';
import type { Product } from '../../util/Types.js';
import './ProductStockModal.css';

type ProductStockModalProps = {
  // null: zárva; egyébként ennek a terméknek a készletét mutatjuk
  product: Product | null;
  onHide: () => void;
};

// Egy termék készlete raktáranként, csak megtekintésre.
// A készletet nem lehet kézzel módosítani: a beszállítói rendelések beérkezése növeli.
export default function ProductStockModal({ product, onHide }: ProductStockModalProps) {
  return (
    <Modal show={product !== null} onHide={onHide} centered>
      <Modal.Header closeButton>
        <Modal.Title>{product ? `Készlet – ${product.name}` : 'Készlet'}</Modal.Title>
      </Modal.Header>

      {product && (
        <Modal.Body>
          {product.stocks.length === 0 ? (
            <p className="mb-2">Ebből a termékből egyik raktárban sincs készlet.</p>
          ) : (
            <Table size="sm" className="product-stock-table align-middle mb-2">
              <thead>
                <tr>
                  <th>Raktár</th>
                  <th className="product-stock-qty">Mennyiség</th>
                </tr>
              </thead>
              <tbody>
                {product.stocks.map((stock) => (
                  <tr key={stock.warehouseId}>
                    <td className="product-stock-name">{stock.warehouseName}</td>
                    <td className="product-stock-qty">{stock.quantity} db</td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr>
                  <td>Összesen</td>
                  <td className="product-stock-qty">{product.stockQuantity} db</td>
                </tr>
              </tfoot>
            </Table>
          )}
          <p className="product-stock-hint">
            Minimális készlet: {product.minStockLevel} db. A készletet a beszállítói rendelések beérkezése növeli.
          </p>
        </Modal.Body>
      )}

      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          Bezárás
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
