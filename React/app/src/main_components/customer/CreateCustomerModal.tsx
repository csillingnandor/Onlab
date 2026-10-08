import { useEffect, useState } from 'react';
import { Form } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import customerService from '../../services/customerService.js';
import type { Customer, NewCustomer } from '../../util/Types.js';

type CreateCustomerModalProps = {
  show: boolean;
  onCreated: (customer: Customer) => void;
  onHide: () => void;
};

// Ugyanaz a szabály, mint a backend validátorban. A böngésző a pattern-t v flaggel fordítja,
// ezért a karakterosztályban a ( ) / - jeleket escape-elni kell, különben a minta érvénytelen és figyelmen kívül marad.
const PHONE_PATTERN = String.raw`\+?[0-9 \(\)\/\-]+`;

const emptyCustomer: NewCustomer = { name: '', email: '', phone: '' };

// A szabályok a backend CreateCustomerDataValidator-ával egyeznek; az e-mail egyediségét csak a szerver tudja ellenőrizni.
export default function CreateCustomerModal({ show, onCreated, onHide }: CreateCustomerModalProps) {
  const [form, setForm] = useState<NewCustomer>(emptyCustomer);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (show) {
      setForm(emptyCustomer);
      setError(null);
    }
  }, [show]);

  const update = (patch: Partial<NewCustomer>) => setForm((prev) => ({ ...prev, ...patch }));

  const handleSubmit = async () => {
    setSaving(true);
    setError(null);
    try {
      const phone = form.phone?.trim();
      const created = await customerService.create({
        name: form.name.trim(),
        email: form.email.trim(),
        phone: phone ? phone : null,
      });
      onCreated(created);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title="Új vevő"
      submitLabel="Vevő mentése"
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
    >
      <Form.Group controlId="create-customer-name" className="mb-3">
        <Form.Label>Név</Form.Label>
        <Form.Control
          type="text"
          maxLength={100}
          placeholder="pl. Minta Kft."
          value={form.name}
          onChange={(e) => update({ name: e.target.value })}
          required
          autoFocus
        />
      </Form.Group>

      <Form.Group controlId="create-customer-email" className="mb-3">
        <Form.Label>E-mail</Form.Label>
        <Form.Control
          type="email"
          maxLength={254}
          placeholder="nev@ceg.hu"
          value={form.email}
          onChange={(e) => update({ email: e.target.value })}
          required
        />
      </Form.Group>

      <Form.Group controlId="create-customer-phone">
        <Form.Label>
          Telefon <span className="text-muted">(nem kötelező)</span>
        </Form.Label>
        <Form.Control
          type="tel"
          maxLength={30}
          pattern={PHONE_PATTERN}
          title="Csak számjegyek, szóköz és + ( ) / - jelek"
          placeholder="+36 1 234 5678"
          value={form.phone ?? ''}
          onChange={(e) => update({ phone: e.target.value })}
        />
      </Form.Group>
    </FormModal>
  );
}
