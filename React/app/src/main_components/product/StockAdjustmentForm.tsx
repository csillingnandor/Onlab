import { useEffect, useState, type FormEvent } from 'react';
import { Alert, Col, Form, Row, Spinner } from 'react-bootstrap';
import Button from '../../common/button/Button.js';
import productService from '../../services/productService.js';
import warehouseService from '../../services/warehouseService.js';
import type { Product, Warehouse } from '../../util/Types.js';

type StockAdjustmentFormProps = {
  product: Product;
  onAdjusted: (product: Product) => void;
};

// Kézi készletkorrekció (leltár, sérült áru...): a raktárban ténylegesen megszámolt mennyiséget kell megadni,
// a nyilvántartotthoz képesti különbség indoklással kerül a készletnaplóba.
export default function StockAdjustmentForm({ product, onAdjusted }: StockAdjustmentFormProps) {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [warehouseId, setWarehouseId] = useState('');
  const [counted, setCounted] = useState('');
  const [note, setNote] = useState('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;
    warehouseService.getAll()
      .then((data) => {
        if (!ignore) setWarehouses([...data].sort((a, b) => a.name.localeCompare(b.name, 'hu')));
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Nem sikerült a raktárak betöltése.');
      });
    return () => {
      ignore = true;
    };
  }, []);

  const currentQuantity = (id: string) =>
    product.stocks.find((s) => String(s.warehouseId) === id)?.quantity ?? 0;

  // Raktárválasztáskor a nyilvántartott mennyiség az alapérték, azt kell átírni a megszámoltra
  const selectWarehouse = (id: string) => {
    setWarehouseId(id);
    setCounted(id ? String(currentQuantity(id)) : '');
    setSuccess(null);
  };

  const countedValid = /^\d+$/.test(counted);
  const difference = warehouseId && countedValid ? Number(counted) - currentQuantity(warehouseId) : 0;
  const canSubmit = !saving && warehouseId !== '' && countedValid && difference !== 0 && note.trim() !== '';

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!canSubmit) return;
    setSaving(true);
    setError(null);
    setSuccess(null);
    try {
      const updated = await productService.adjustStock(product.id, {
        warehouseId: Number(warehouseId),
        countedQuantity: Number(counted),
        note: note.trim(),
      });
      setSuccess(`Korrekció rögzítve: ${difference > 0 ? '+' : '−'}${Math.abs(difference)} db.`);
      setNote('');
      onAdjusted(updated);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Nem sikerült a korrekció.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Form onSubmit={handleSubmit} className="product-stock-adjust mb-3">
      {error && <Alert variant="danger" className="py-2">{error}</Alert>}
      {success && <Alert variant="success" className="py-2">{success}</Alert>}
      <Row className="g-2 align-items-end">
        <Form.Group as={Col} md={4} controlId="stock-adjust-warehouse">
          <Form.Label className="small mb-1">Raktár</Form.Label>
          <Form.Select size="sm" value={warehouseId} onChange={(e) => selectWarehouse(e.target.value)} required>
            <option value="">Válassz…</option>
            {warehouses.map((w) => (
              <option key={w.id} value={w.id}>{w.name} ({currentQuantity(String(w.id))} db)</option>
            ))}
          </Form.Select>
        </Form.Group>
        <Form.Group as={Col} md={2} controlId="stock-adjust-counted">
          <Form.Label className="small mb-1">Megszámolt</Form.Label>
          <Form.Control
            size="sm"
            type="number"
            min={0}
            step={1}
            value={counted}
            onChange={(e) => setCounted(e.target.value)}
            disabled={!warehouseId}
            required
          />
        </Form.Group>
        <Form.Group as={Col} md={4} controlId="stock-adjust-note">
          <Form.Label className="small mb-1">Indoklás</Form.Label>
          <Form.Control
            size="sm"
            type="text"
            maxLength={200}
            placeholder="pl. Leltár 2026/10, sérült áru"
            value={note}
            onChange={(e) => setNote(e.target.value)}
            required
          />
        </Form.Group>
        <Col md={2}>
          <Button type="submit" size="sm" className="w-100" disabled={!canSubmit}>
            {saving ? <Spinner animation="border" size="sm" aria-hidden="true" /> : 'Rögzítés'}
          </Button>
        </Col>
      </Row>
      {warehouseId && countedValid && (
        <Form.Text muted>
          {difference === 0
            ? 'Nincs eltérés a nyilvántartott készlettől.'
            : `Eltérés: ${difference > 0 ? '+' : '−'}${Math.abs(difference)} db (nyilvántartott: ${currentQuantity(warehouseId)} db).`}
        </Form.Text>
      )}
    </Form>
  );
}
