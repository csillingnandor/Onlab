import { useEffect, useState } from 'react';
import { Form } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import supplierService from '../../services/supplierService.js';
import type { Supplier, SupplierInput } from '../../util/Types.js';

type SupplierFormModalProps = {
  show: boolean;
  supplier: Supplier | null; // null: új beszállító
  onSaved: (supplier: Supplier) => void;
  onHide: () => void;
};

// Mint a vevőnél: a pattern-t a böngésző v flaggel fordítja, ezért a ( ) / - jeleket escape-elni kell
const PHONE_PATTERN = String.raw`\+?[0-9 \(\)\/\-]+`;

type SupplierForm = { name: string; email: string; phone: string };

// Beszállító felvétele és módosítása; a szabályok a backend SaveSupplierDataValidator-ával egyeznek,
// a név egyediségét a szerver ellenőrzi.
export default function SupplierFormModal({ show, supplier, onSaved, onHide }: SupplierFormModalProps) {
  const [form, setForm] = useState<SupplierForm>({ name: '', email: '', phone: '' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (show) {
      setForm({ name: supplier?.name ?? '', email: supplier?.email ?? '', phone: supplier?.phone ?? '' });
      setError(null);
    }
  }, [show, supplier]);

  const update = (patch: Partial<SupplierForm>) => setForm((prev) => ({ ...prev, ...patch }));

  const handleSubmit = async () => {
    setSaving(true);
    setError(null);
    try {
      const input: SupplierInput = {
        name: form.name.trim(),
        email: form.email.trim() || null,
        phone: form.phone.trim() || null,
      };
      onSaved(supplier ? await supplierService.update(supplier.id, input) : await supplierService.create(input));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title={supplier ? 'Beszállító módosítása' : 'Új beszállító'}
      submitLabel={supplier ? 'Módosítások mentése' : 'Beszállító mentése'}
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
    >
      <Form.Group controlId="supplier-form-name" className="mb-3">
        <Form.Label>Név</Form.Label>
        <Form.Control
          type="text"
          maxLength={100}
          placeholder="pl. TechDistri Kft."
          value={form.name}
          onChange={(e) => update({ name: e.target.value })}
          required
          autoFocus
        />
      </Form.Group>

      <Form.Group controlId="supplier-form-email" className="mb-3">
        <Form.Label>
          E-mail <span className="text-muted">(nem kötelező)</span>
        </Form.Label>
        <Form.Control
          type="email"
          maxLength={254}
          placeholder="rendeles@ceg.hu"
          value={form.email}
          onChange={(e) => update({ email: e.target.value })}
        />
      </Form.Group>

      <Form.Group controlId="supplier-form-phone">
        <Form.Label>
          Telefon <span className="text-muted">(nem kötelező)</span>
        </Form.Label>
        <Form.Control
          type="tel"
          maxLength={30}
          pattern={PHONE_PATTERN}
          title="Csak számjegyek, szóköz és + ( ) / - jelek"
          placeholder="+36 1 234 5678"
          value={form.phone}
          onChange={(e) => update({ phone: e.target.value })}
        />
      </Form.Group>
    </FormModal>
  );
}
