import { useEffect, useState } from 'react';
import { Alert, Modal, Spinner, Table } from 'react-bootstrap';
import Button from '../../common/button/Button.js';
import productService from '../../services/productService.js';
import type { Product, StockMovement, StockMovementType } from '../../util/Types.js';
import { dateTimeFormatter } from '../../util/format.js';
import StockAdjustmentForm from './StockAdjustmentForm.js';
import './ProductStockModal.css';

type ProductStockModalProps = {
  // null: zárva; egyébként ennek a terméknek a készletét mutatjuk
  product: Product | null;
  onHide: () => void;
  // Megadva (van hozzá jogosultság): kézi korrekció is rögzíthető; a frissített terméket kapja meg
  onAdjusted?: ((product: Product) => void) | undefined;
};

const typeLabels: Record<StockMovementType, string> = {
  Opening: 'Nyitókészlet',
  Receipt: 'Beérkezés',
  Shipment: 'Kiszállítás',
  Adjustment: 'Korrekció',
};

// Mozgás + a mozgás utáni készlet abban a raktárban (a naplóból visszaszámolva)
type HistoryRow = StockMovement & { balance: number };

// A napló legújabb elöl érkezik; az egyenleget raktáronként a legrégebbitől számoljuk
function withBalances(movements: StockMovement[]): HistoryRow[] {
  const running = new Map<number, number>();
  const rows = [...movements].reverse().map((m) => {
    const balance = (running.get(m.warehouseId) ?? 0) + m.quantityChange;
    running.set(m.warehouseId, balance);
    return { ...m, balance };
  });
  return rows.reverse();
}

// Raktárak, ahol a napló szerinti készlet eltér a nyilvántartottól (rendben működésnél üres)
function mismatchedWarehouses(product: Product, rows: HistoryRow[]): string[] {
  const fromLog = new Map<number, { name: string; quantity: number }>();
  // rows legújabb elöl: raktáronként az első előfordulás a végső egyenleg
  for (const r of rows) {
    if (!fromLog.has(r.warehouseId)) fromLog.set(r.warehouseId, { name: r.warehouseName, quantity: r.balance });
  }
  const names = new Set<string>();
  for (const [id, log] of fromLog) {
    const current = product.stocks.find((s) => s.warehouseId === id)?.quantity ?? 0;
    if (current !== log.quantity) names.add(log.name);
  }
  for (const s of product.stocks) {
    if (!fromLog.has(s.warehouseId) && s.quantity !== 0) names.add(s.warehouseName);
  }
  return [...names];
}

// Rendelés, illetve kézi korrekciónál az indoklás
const reference = (m: StockMovement) =>
  m.supplierOrderId !== null ? `#${m.supplierOrderId} beszerzés`
  : m.customerOrderId !== null ? `#${m.customerOrderId} rendelés`
  : m.note ?? '–';

// Egy termék készlete raktáranként és a készlettörténete. A készletet a beérkezés növeli, a kiszállítás csökkenti;
// jogosultsággal kézi korrekció (leltáreltérés) is rögzíthető, ami szintén a naplóba kerül.
export default function ProductStockModal({ product, onHide, onAdjusted }: ProductStockModalProps) {
  const [history, setHistory] = useState<HistoryRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Megnyitáskor friss napló
  useEffect(() => {
    if (!product) return;
    let ignore = false;

    setHistory([]);
    setError(null);
    setLoading(true);

    productService.getMovements(product.id)
      .then((movements) => {
        if (!ignore) setHistory(withBalances(movements));
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Nem sikerült a készlettörténet betöltése.');
      })
      .finally(() => {
        if (!ignore) setLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, [product]);

  const mismatched = product && !loading && !error ? mismatchedWarehouses(product, history) : [];

  return (
    <Modal show={product !== null} onHide={onHide} size="lg" centered scrollable>
      <Modal.Header closeButton>
        <Modal.Title>{product ? `Készlet – ${product.name}` : 'Készlet'}</Modal.Title>
      </Modal.Header>

      {product && (
        <Modal.Body>
          <h2 className="product-stock-section">Raktáronként</h2>
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
          <p className="product-stock-hint mb-3">
            Minimális készlet: {product.minStockLevel} db. A készletet a beérkezés növeli, a kiszállítás csökkenti.
          </p>

          {onAdjusted && (
            <>
              <h2 className="product-stock-section">Készletkorrekció</h2>
              <StockAdjustmentForm product={product} onAdjusted={onAdjusted} />
            </>
          )}

          <h2 className="product-stock-section">Készlettörténet</h2>
          {loading ? (
            <div className="text-center py-3">
              <Spinner animation="border" size="sm" role="status">
                <span className="visually-hidden">Készlettörténet betöltése...</span>
              </Spinner>
            </div>
          ) : error ? (
            <Alert variant="danger" className="mb-0">{error}</Alert>
          ) : history.length === 0 ? (
            <p className="mb-0">Ennek a terméknek még nincs készletmozgása.</p>
          ) : (
            <>
              {mismatched.length > 0 && (
                <Alert variant="warning" className="py-2">
                  A napló szerinti készlet eltér a nyilvántartottól: {mismatched.join(', ')}.
                </Alert>
              )}
              <Table size="sm" className="product-stock-table product-stock-history align-middle mb-0">
                <thead>
                  <tr>
                    <th>Időpont</th>
                    <th>Raktár</th>
                    <th>Típus</th>
                    <th>Hivatkozás</th>
                    <th className="product-stock-qty">Változás</th>
                    <th className="product-stock-qty">Egyenleg</th>
                  </tr>
                </thead>
                <tbody>
                  {history.map((m) => (
                    <tr key={m.id}>
                      <td className="product-stock-date">{dateTimeFormatter.format(new Date(m.createdAt))}</td>
                      <td>{m.warehouseName}</td>
                      <td>{typeLabels[m.type]}</td>
                      <td className="product-stock-ref">{reference(m)}</td>
                      <td className={`product-stock-qty ${m.quantityChange > 0 ? 'product-stock-in' : 'product-stock-out'}`}>
                        {m.quantityChange > 0 ? `+${m.quantityChange}` : `−${-m.quantityChange}`} db
                      </td>
                      <td className="product-stock-qty">{m.balance} db</td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </>
          )}
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
