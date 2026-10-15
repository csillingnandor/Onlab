import { useEffect, useState } from 'react';
import { Col, Form, Row } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import warehouseService from '../../services/warehouseService.js';
import type { Warehouse, WarehouseInput } from '../../util/Types.js';

type WarehouseFormModalProps = {
  show: boolean;
  warehouse: Warehouse | null; // null: új raktár
  onSaved: (warehouse: Warehouse) => void;
  onHide: () => void;
};

// A mezők nyers szövege; a számmá alakítás mentéskor történik
type WarehouseForm = { name: string; address: string; capacity: string; latitude: string; longitude: string };

const toText = (value: number | null | undefined) => (value === null || value === undefined ? '' : String(value));
const toNumber = (text: string) => (text.trim() === '' ? null : Number(text.replace(',', '.')));

// Raktár felvétele és módosítása; a szabályok a backend SaveWarehouseDataValidator-ával egyeznek.
// A koordinátát együtt kell megadni (vagy egyiket sem), különben a raktár nem jelenik meg a térképen.
export default function WarehouseFormModal({ show, warehouse, onSaved, onHide }: WarehouseFormModalProps) {
  const [form, setForm] = useState<WarehouseForm>({ name: '', address: '', capacity: '', latitude: '', longitude: '' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (show) {
      setForm({
        name: warehouse?.name ?? '',
        address: warehouse?.address ?? '',
        capacity: toText(warehouse?.capacity),
        latitude: toText(warehouse?.latitude),
        longitude: toText(warehouse?.longitude),
      });
      setError(null);
    }
  }, [show, warehouse]);

  const update = (patch: Partial<WarehouseForm>) => setForm((prev) => ({ ...prev, ...patch }));
  const coordinatesIncomplete = (form.latitude.trim() === '') !== (form.longitude.trim() === '');

  const handleSubmit = async () => {
    setSaving(true);
    setError(null);
    try {
      const input: WarehouseInput = {
        name: form.name.trim(),
        address: form.address.trim(),
        capacity: Number(form.capacity),
        latitude: toNumber(form.latitude),
        longitude: toNumber(form.longitude),
      };
      onSaved(warehouse ? await warehouseService.update(warehouse.id, input) : await warehouseService.create(input));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title={warehouse ? 'Raktár módosítása' : 'Új raktár'}
      submitLabel={warehouse ? 'Módosítások mentése' : 'Raktár mentése'}
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
      submitDisabled={coordinatesIncomplete}
    >
      <Form.Group controlId="warehouse-form-name" className="mb-3">
        <Form.Label>Név</Form.Label>
        <Form.Control
          type="text"
          maxLength={100}
          placeholder="pl. Győri raktár"
          value={form.name}
          onChange={(e) => update({ name: e.target.value })}
          required
          autoFocus
        />
      </Form.Group>

      <Form.Group controlId="warehouse-form-address" className="mb-3">
        <Form.Label>Cím</Form.Label>
        <Form.Control
          type="text"
          maxLength={200}
          placeholder="pl. 9024 Győr, Minta utca 5."
          value={form.address}
          onChange={(e) => update({ address: e.target.value })}
        />
      </Form.Group>

      <Form.Group controlId="warehouse-form-capacity" className="mb-3">
        <Form.Label>Kapacitás (hely)</Form.Label>
        <Form.Control
          type="number"
          min={0}
          step={1}
          value={form.capacity}
          onChange={(e) => update({ capacity: e.target.value })}
          required
        />
      </Form.Group>

      <Row className="g-3">
        <Form.Group as={Col} sm={6} controlId="warehouse-form-latitude">
          <Form.Label>Szélesség <span className="text-muted">(nem kötelező)</span></Form.Label>
          <Form.Control
            type="number"
            min={-90}
            max={90}
            step="any"
            placeholder="pl. 47.6875"
            value={form.latitude}
            onChange={(e) => update({ latitude: e.target.value })}
          />
        </Form.Group>
        <Form.Group as={Col} sm={6} controlId="warehouse-form-longitude">
          <Form.Label>Hosszúság <span className="text-muted">(nem kötelező)</span></Form.Label>
          <Form.Control
            type="number"
            min={-180}
            max={180}
            step="any"
            placeholder="pl. 17.6504"
            value={form.longitude}
            onChange={(e) => update({ longitude: e.target.value })}
          />
        </Form.Group>
      </Row>
      {coordinatesIncomplete && (
        <Form.Text className="text-warning">A szélességet és a hosszúságot együtt kell megadni (vagy egyiket sem).</Form.Text>
      )}
    </FormModal>
  );
}
