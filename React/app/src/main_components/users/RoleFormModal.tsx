import { useEffect, useState } from 'react';
import { Form } from 'react-bootstrap';
import FormModal from '../../common/modal/FormModal.js';
import userService from '../../services/userService.js';
import type { Role } from '../../util/Types.js';
import { appPages, pageActions, type ActionKey, type PageKey } from '../../util/pages.js';
import { useI18n } from '../../i18n/useI18n.js';

type RoleFormModalProps = {
  show: boolean;
  role: Role | null; // null: új szerepkör
  onSaved: (role: Role) => void;
  onHide: () => void;
};

// Szerepkör (felhasználói csoport) neve, az oldalak, amelyeket a tagjai elérnek, és oldalanként az engedélyezett műveletek.
// Művelet csak bejelölt oldalhoz adható; az oldal önmagában csak megtekintést jelent.
export default function RoleFormModal({ show, role, onSaved, onHide }: RoleFormModalProps) {
  const { t } = useI18n();
  const [name, setName] = useState('');
  const [pages, setPages] = useState<PageKey[]>([]);
  const [actions, setActions] = useState<ActionKey[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (show) {
      setName(role?.name ?? '');
      setPages(role?.pages ?? []);
      setActions(role?.actions ?? []);
      setError(null);
    }
  }, [show, role]);

  // Az oldal kivételével a műveletei is törlődnek
  const togglePage = (key: PageKey, checked: boolean) => {
    setPages((prev) => (checked ? [...prev, key] : prev.filter((p) => p !== key)));
    if (!checked) {
      const ofPage = new Set((pageActions[key] ?? []).map((a) => a.key));
      setActions((prev) => prev.filter((a) => !ofPage.has(a)));
    }
  };

  const toggleAction = (key: ActionKey, checked: boolean) =>
    setActions((prev) => (checked ? [...prev, key] : prev.filter((a) => a !== key)));

  const handleSubmit = async () => {
    setSaving(true);
    setError(null);
    try {
      const input = { name: name.trim(), pages, actions };
      onSaved(role ? await userService.updateRole(role.id, input) : await userService.createRole(input));
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a mentés során.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <FormModal
      show={show}
      title={role ? 'Szerepkör módosítása' : 'Új szerepkör'}
      submitLabel={role ? 'Módosítások mentése' : 'Szerepkör mentése'}
      saving={saving}
      error={error}
      onSubmit={handleSubmit}
      onHide={onHide}
      size="lg"
    >
      <Form.Group controlId="role-form-name" className="mb-3">
        <Form.Label>Név</Form.Label>
        <Form.Control
          type="text"
          maxLength={50}
          placeholder="pl. Könyvelő"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          autoFocus
        />
      </Form.Group>

      <fieldset>
        <legend className="form-label fs-6">Elérhető oldalak és műveletek</legend>
        <Form.Text muted className="d-block mb-2">
          Az oldal bejelölése megtekintést ad; alatta választható ki, mit végezhet el rajta a szerepkör.
        </Form.Text>
        {appPages.map((page) => {
          const enabled = pages.includes(page.key);
          return (
            <div key={page.key} className="role-form-page">
              <Form.Check
                id={`role-form-page-${page.key}`}
                type="checkbox"
                label={
                  <>
                    <i className={`bi ${page.icon} me-2`} aria-hidden="true"></i>
                    {t(page.label)}
                  </>
                }
                checked={enabled}
                onChange={(e) => togglePage(page.key, e.target.checked)}
              />
              {(pageActions[page.key] ?? []).length > 0 && (
                <div className="role-form-actions">
                  {(pageActions[page.key] ?? []).map((action) => (
                    <Form.Check
                      key={action.key}
                      id={`role-form-action-${action.key}`}
                      type="checkbox"
                      inline
                      label={t(action.label)}
                      checked={actions.includes(action.key)}
                      disabled={!enabled}
                      onChange={(e) => toggleAction(action.key, e.target.checked)}
                    />
                  ))}
                </div>
              )}
            </div>
          );
        })}
      </fieldset>
    </FormModal>
  );
}
