import { useEffect, useState } from 'react';
import { Form } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import userService from '../../services/userService.js';
import type { Role, User } from '../../util/Types.js';

type UserFormModalProps = {
  show: boolean;
  user: User | null; // null: új felhasználó
  roles: Role[];
  onSaved: (user: User) => void;
  onHide: () => void;
};

type UserForm = {
  userName: string;
  displayName: string;
  password: string;
  roles: string[];
};

// Új felhasználó felvétele, illetve meglévő módosítása (név, szerepkörök, opcionálisan új jelszó).
// A jelszóerősséget (min. 8 karakter, kis- és nagybetű, számjegy) a backend ellenőrzi, a hibáit itt jelenítjük meg.
export default function UserFormModal({ show, user, roles, onSaved, onHide }: UserFormModalProps) {
  const [form, setForm] = useState<UserForm>({ userName: '', displayName: '', password: '', roles: [] });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEdit = user !== null;

  useEffect(() => {
    if (show) {
      setForm({
        userName: user?.userName ?? '',
        displayName: user?.displayName ?? '',
        password: '',
        roles: user?.roles ?? [],
      });
      setError(null);
    }
  }, [show, user]);

  const update = (patch: Partial<UserForm>) => setForm((prev) => ({ ...prev, ...patch }));

  const toggleRole = (name: string, checked: boolean) =>
    update({ roles: checked ? [...form.roles, name] : form.roles.filter((r) => r !== name) });

  const handleSubmit = async () => {
    setSaving(true);
    setError(null);
    try {
      const saved = user
        ? await userService.update(user.id, {
            displayName: form.displayName.trim(),
            newPassword: form.password ? form.password : null,
            roles: form.roles,
          })
        : await userService.create({
            userName: form.userName.trim(),
            displayName: form.displayName.trim(),
            password: form.password,
            roles: form.roles,
          });
      onSaved(saved);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title={isEdit ? 'Felhasználó módosítása' : 'Új felhasználó'}
      submitLabel={isEdit ? 'Módosítások mentése' : 'Felhasználó mentése'}
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
    >
      <Form.Group controlId="user-form-username" className="mb-3">
        <Form.Label>Felhasználónév</Form.Label>
        <Form.Control
          type="text"
          maxLength={50}
          autoComplete="off"
          placeholder="pl. kovacs.janos"
          value={form.userName}
          onChange={(e) => update({ userName: e.target.value })}
          disabled={isEdit}
          required
          autoFocus={!isEdit}
        />
        {isEdit && <Form.Text muted>A bejelentkezési név nem módosítható.</Form.Text>}
      </Form.Group>

      <Form.Group controlId="user-form-displayname" className="mb-3">
        <Form.Label>Teljes név</Form.Label>
        <Form.Control
          type="text"
          maxLength={100}
          placeholder="pl. Kovács János"
          value={form.displayName}
          onChange={(e) => update({ displayName: e.target.value })}
          required
          autoFocus={isEdit}
        />
      </Form.Group>

      <Form.Group controlId="user-form-password" className="mb-3">
        <Form.Label>
          {isEdit ? <>Új jelszó <span className="text-muted">(üresen hagyva marad a régi)</span></> : 'Jelszó'}
        </Form.Label>
        <Form.Control
          type="password"
          autoComplete="new-password"
          value={form.password}
          onChange={(e) => update({ password: e.target.value })}
          required={!isEdit}
        />
        <Form.Text muted>Legalább 8 karakter, kis- és nagybetűvel, számjeggyel.</Form.Text>
      </Form.Group>

      <fieldset>
        <legend className="form-label fs-6">Szerepkörök</legend>
        {roles.map((role) => (
          <Form.Check
            key={role.id}
            id={`user-form-role-${role.id}`}
            type="checkbox"
            label={role.name}
            checked={form.roles.includes(role.name)}
            onChange={(e) => toggleRole(role.name, e.target.checked)}
          />
        ))}
        {form.roles.length === 0 && (
          <Form.Text className="text-warning">Szerepkör nélkül a felhasználó egyetlen oldalt sem ér el.</Form.Text>
        )}
      </fieldset>
    </FormModal>
  );
}
