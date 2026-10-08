import { useEffect, useState } from 'react';
import { Button, Form, Spinner, Table } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import productService from '../../services/productService.js';
import type { NewOrderItem, Product } from '../../util/Types.js';
import { priceFormatter } from '../../util/format.js';
import './CreateOrderModal.css';

type Party = { id: number; name: string };

type CreateOrderModalProps = {
  show: boolean;
  title: string; // pl. 'Új vevői rendelés'
  partyLabel: string; // 'Vevő' vagy 'Beszállító'
  loadParties: () => Promise<Party[]>;
  // 'catalog': a termék aktuális eladási ára (vevői rendelés); 'manual': kézzel megadott beszerzési ár
  priceMode: 'catalog' | 'manual';
  onSubmit: (partyId: number, items: NewOrderItem[]) => Promise<void>;
  onHide: () => void;
};

// Egy tételsor nyers mezőértékei; a key csak a React listához kell
type Line = { key: number; productId: string; quantity: string; unitCost: string };

let nextLineKey = 1;
const emptyLine = (): Line => ({ key: nextLineKey++, productId: '', quantity: '1', unitCost: '' });

const isPositiveInt = (value: string) => /^\d+$/.test(value) && Number(value) >= 1;
const isPrice = (value: string) => value !== '' && Number(value) >= 0;

// Vevői és beszerzési rendelés felvétele: partner kiválasztása + tetszőleges számú tételsor.
export default function CreateOrderModal({
  show,
  title,
  partyLabel,
  loadParties,
  priceMode,
  onSubmit,
  onHide,
}: CreateOrderModalProps) {
  const [parties, setParties] = useState<Party[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [listsLoading, setListsLoading] = useState(false);
  const [partyId, setPartyId] = useState('');
  const [lines, setLines] = useState<Line[]>([emptyLine()]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Minden megnyitáskor üres űrlap és friss választólisták (készlet, ár változhatott)
  useEffect(() => {
    if (!show) return;
    let ignore = false;

    setPartyId('');
    setLines([emptyLine()]);
    setError(null);
    setListsLoading(true);

    Promise.all([loadParties(), productService.getAll()])
      .then(([loadedParties, loadedProducts]) => {
        if (ignore) return;
        setParties(loadedParties);
        setProducts([...loadedProducts].sort((a, b) => a.name.localeCompare(b.name, 'hu')));
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Nem sikerült a listák betöltése.');
      })
      .finally(() => {
        if (!ignore) setListsLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, [show, loadParties]);

  const productById = new Map(products.map((p) => [p.id, p]));

  const updateLine = (key: number, patch: Partial<Line>) =>
    setLines((prev) => prev.map((line) => (line.key === key ? { ...line, ...patch } : line)));

  const unitPriceOf = (line: Line): number | null => {
    if (priceMode === 'manual') return isPrice(line.unitCost) ? Number(line.unitCost) : null;
    const product = productById.get(Number(line.productId));
    return product ? product.price : null;
  };

  const lineTotal = (line: Line): number | null => {
    const unitPrice = unitPriceOf(line);
    return unitPrice !== null && isPositiveInt(line.quantity) ? unitPrice * Number(line.quantity) : null;
  };

  const lineValid = (line: Line) =>
    line.productId !== '' && isPositiveInt(line.quantity) && (priceMode === 'catalog' || isPrice(line.unitCost));

  const canSubmit = !listsLoading && !saving && partyId !== '' && lines.every(lineValid);
  const total = lines.reduce((sum, line) => sum + (lineTotal(line) ?? 0), 0);

  const handleSubmit = async () => {
    if (!canSubmit) return;

    setSaving(true);
    setError(null);
    try {
      await onSubmit(
        Number(partyId),
        lines.map((line) => ({
          productId: Number(line.productId),
          quantity: Number(line.quantity),
          ...(priceMode === 'manual' ? { unitCost: Number(line.unitCost) } : {}),
        })),
      );
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title={title}
      submitLabel="Rendelés mentése"
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
      submitDisabled={!canSubmit}
      size="lg"
    >
      {listsLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Listák betöltése...</span>
          </Spinner>
        </div>
      ) : (
        <>
          <Form.Group controlId="create-order-party" className="mb-3">
            <Form.Label>{partyLabel}</Form.Label>
            <Form.Select value={partyId} onChange={(e) => setPartyId(e.target.value)} required>
              <option value="">Válassz…</option>
              {parties.map((party) => (
                <option key={party.id} value={party.id}>{party.name}</option>
              ))}
            </Form.Select>
          </Form.Group>

          <Table size="sm" className="create-order-lines align-middle">
            <thead>
              <tr>
                <th>Termék</th>
                <th className="create-order-qty">Mennyiség</th>
                <th className="create-order-price">{priceMode === 'manual' ? 'Beszerzési ár (Ft)' : 'Egységár'}</th>
                <th className="create-order-total">Összesen</th>
                <th aria-label="Műveletek"></th>
              </tr>
            </thead>
            <tbody>
              {lines.map((line, index) => {
                const product = productById.get(Number(line.productId));
                const lineSum = lineTotal(line);
                return (
                  <tr key={line.key}>
                    <td>
                      <Form.Select
                        size="sm"
                        aria-label={`${index + 1}. tétel terméke`}
                        value={line.productId}
                        onChange={(e) => updateLine(line.key, { productId: e.target.value })}
                        required
                      >
                        <option value="">Válassz terméket…</option>
                        {products.map((p) => (
                          <option key={p.id} value={p.id}>
                            {p.name} ({p.sku})
                          </option>
                        ))}
                      </Form.Select>
                      {product && (
                        <div className="create-order-stock">Készleten: {product.stockQuantity} db</div>
                      )}
                    </td>
                    <td>
                      <Form.Control
                        type="number"
                        size="sm"
                        min={1}
                        step={1}
                        aria-label={`${index + 1}. tétel mennyisége`}
                        value={line.quantity}
                        onChange={(e) => updateLine(line.key, { quantity: e.target.value })}
                        isInvalid={line.quantity !== '' && !isPositiveInt(line.quantity)}
                        required
                      />
                    </td>
                    <td>
                      {priceMode === 'manual' ? (
                        <Form.Control
                          type="number"
                          size="sm"
                          min={0}
                          step="0.01"
                          aria-label={`${index + 1}. tétel beszerzési ára`}
                          value={line.unitCost}
                          onChange={(e) => updateLine(line.key, { unitCost: e.target.value })}
                          required
                        />
                      ) : (
                        <span className="numeric">{product ? priceFormatter.format(product.price) : '–'}</span>
                      )}
                    </td>
                    <td className="create-order-total">
                      {lineSum !== null ? priceFormatter.format(lineSum) : '–'}
                    </td>
                    <td>
                      <Button
                        variant="link"
                        size="sm"
                        className="create-order-remove"
                        onClick={() => setLines((prev) => prev.filter((l) => l.key !== line.key))}
                        disabled={lines.length === 1}
                        aria-label={`${index + 1}. tétel törlése`}
                        title="Tétel törlése"
                      >
                        <i className="bi bi-trash" aria-hidden="true"></i>
                      </Button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
            <tfoot>
              <tr>
                <td colSpan={3}>
                  <Button
                    variant="outline-light"
                    size="sm"
                    onClick={() => setLines((prev) => [...prev, emptyLine()])}
                  >
                    <i className="bi bi-plus-lg me-1" aria-hidden="true"></i>
                    Tétel hozzáadása
                  </Button>
                </td>
                <td className="create-order-total create-order-grand-total">{priceFormatter.format(total)}</td>
                <td></td>
              </tr>
            </tfoot>
          </Table>

          {priceMode === 'catalog' && (
            <p className="create-order-hint">A rendelés a termékek mentéskori árával jön létre, Függőben státusszal.</p>
          )}
          {priceMode === 'manual' && (
            <p className="create-order-hint">A rendelés Függőben státusszal jön létre.</p>
          )}
        </>
      )}
    </FormModal>
  );
}
