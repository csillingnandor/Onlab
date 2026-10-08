import { useEffect, useState } from 'react';
import { Col, Form, Row } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import productService from '../../services/productService.js';
import type { Product, ProductInput } from '../../util/Types.js';

type ProductFormModalProps = {
  show: boolean;
  // null: új termék felvétele; egyébként ennek a terméknek a módosítása
  product: Product | null;
  // Kategória javaslatok a meglévő termékekből (szabadon gépelhető új is)
  categories: string[];
  onSaved: (saved: Product) => void;
  onHide: () => void;
};

// Nyers mezőértékek: a számokat is szövegként tároljuk, hogy üres / félig beírt érték is lehessen
type Fields = { name: string; sku: string; category: string; minStockLevel: string; price: string };
type FieldErrors = Partial<Record<keyof Fields, string>>;

const toFields = (p: Product): Fields => ({
  name: p.name,
  sku: p.sku,
  category: p.category,
  minStockLevel: String(p.minStockLevel),
  price: String(p.price),
});

// Új terméknél: az árat kötelező beírni, a minimális készlet javasolt alapértéke 5.
// Az új termék 0 készlettel jön létre; a készletet raktáranként a ProductStockModal állítja.
const emptyFields: Fields = { name: '', sku: '', category: '', minStockLevel: '5', price: '' };

const isNonNegativeInt = (value: string) => /^\d+$/.test(value.trim());
// Legfeljebb 2 tizedesjegy (az adatbázisban decimal(18,2)); tizedesvessző is elfogadott
const isPrice = (value: string) => /^\d+([.,]\d{1,2})?$/.test(value.trim());
const toNumber = (value: string) => Number(value.trim().replace(',', '.'));

// Ugyanazok a szabályok, mint a backend Create- és ModifyProductDataValidator-ában, hogy a hiba már küldés előtt kiderüljön
function validate(f: Fields): FieldErrors {
  const errors: FieldErrors = {};
  if (f.name.trim() === '') errors.name = 'A név megadása kötelező.';
  else if (f.name.trim().length > 100) errors.name = 'A név legfeljebb 100 karakter lehet.';

  if (f.sku.trim() === '') errors.sku = 'Az SKU megadása kötelező.';
  else if (f.sku.trim().length > 50) errors.sku = 'Az SKU legfeljebb 50 karakter lehet.';

  if (f.category.trim().length > 50) errors.category = 'A kategória legfeljebb 50 karakter lehet.';

  if (!isNonNegativeInt(f.minStockLevel)) errors.minStockLevel = 'Nemnegatív egész szám kell.';

  if (!isPrice(f.price)) errors.price = 'Nemnegatív összeg, legfeljebb 2 tizedesjeggyel.';
  else if (toNumber(f.price) > 999_999_999) errors.price = 'Az ár legfeljebb 999 999 999 lehet.';

  return errors;
}

// Termék felvétele és módosítása ugyanazzal az űrlappal és validációval.
export default function ProductFormModal({ show, product, categories, onSaved, onHide }: ProductFormModalProps) {
  const [fields, setFields] = useState<Fields | null>(null);
  // A mezőhibákat csak az első mentési kísérlet után mutatjuk, hogy gépelés közben ne villogjanak
  const [submitted, setSubmitted] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Minden megnyitáskor üres űrlappal, illetve a kiválasztott termék aktuális adataival indulunk
  useEffect(() => {
    if (!show) return;
    setFields(product ? toFields(product) : emptyFields);
    setSubmitted(false);
    setError(null);
  }, [show, product]);

  const errors = fields ? validate(fields) : {};
  const hasErrors = Object.keys(errors).length > 0;
  const fieldError = (key: keyof Fields) => (submitted ? errors[key] : undefined);

  const update = (key: keyof Fields) => (e: { target: { value: string } }) =>
    setFields((prev) => (prev ? { ...prev, [key]: e.target.value } : prev));

  const handleSubmit = async () => {
    if (!fields) return;
    setSubmitted(true);
    if (hasErrors) return;

    const input: ProductInput = {
      name: fields.name.trim(),
      sku: fields.sku.trim(),
      category: fields.category.trim(),
      minStockLevel: Number(fields.minStockLevel.trim()),
      price: toNumber(fields.price),
    };

    setSaving(true);
    setError(null);
    try {
      onSaved(product ? await productService.update(product.id, input) : await productService.create(input));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title={product ? 'Termék módosítása' : 'Új termék'}
      submitLabel={product ? 'Mentés' : 'Termék mentése'}
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
      submitDisabled={submitted && hasErrors}
      noValidate
    >
      {fields && (
        <>
          <Form.Group controlId="product-form-name" className="mb-3">
            <Form.Label>Név</Form.Label>
            <Form.Control value={fields.name} onChange={update('name')} isInvalid={!!fieldError('name')} autoFocus />
            <Form.Control.Feedback type="invalid">{fieldError('name')}</Form.Control.Feedback>
          </Form.Group>

          <Row className="g-3 mb-3">
            <Form.Group as={Col} sm={6} controlId="product-form-sku">
              <Form.Label>SKU</Form.Label>
              <Form.Control
                value={fields.sku}
                onChange={update('sku')}
                isInvalid={!!fieldError('sku')}
                className="font-monospace"
              />
              <Form.Control.Feedback type="invalid">{fieldError('sku')}</Form.Control.Feedback>
            </Form.Group>

            <Form.Group as={Col} sm={6} controlId="product-form-category">
              <Form.Label>Kategória</Form.Label>
              <Form.Control
                value={fields.category}
                onChange={update('category')}
                isInvalid={!!fieldError('category')}
                list="product-form-categories"
              />
              <datalist id="product-form-categories">
                {categories.map((c) => <option key={c} value={c} />)}
              </datalist>
              <Form.Control.Feedback type="invalid">{fieldError('category')}</Form.Control.Feedback>
            </Form.Group>
          </Row>

          <Row className="g-3">
            <Form.Group as={Col} sm={6} controlId="product-form-min-stock">
              <Form.Label>Min. készlet (db)</Form.Label>
              <Form.Control
                type="number"
                min={0}
                step={1}
                inputMode="numeric"
                value={fields.minStockLevel}
                onChange={update('minStockLevel')}
                isInvalid={!!fieldError('minStockLevel')}
              />
              <Form.Control.Feedback type="invalid">{fieldError('minStockLevel')}</Form.Control.Feedback>
            </Form.Group>

            <Form.Group as={Col} sm={6} controlId="product-form-price">
              <Form.Label>Ár (Ft)</Form.Label>
              <Form.Control
                inputMode="decimal"
                value={fields.price}
                onChange={update('price')}
                isInvalid={!!fieldError('price')}
              />
              <Form.Control.Feedback type="invalid">{fieldError('price')}</Form.Control.Feedback>
            </Form.Group>
          </Row>
        </>
      )}
    </FormModal>
  );
}
