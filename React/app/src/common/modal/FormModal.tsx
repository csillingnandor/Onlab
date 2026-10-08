import type { FormEvent, ReactNode } from 'react';
import { Alert, Button, Form, Modal, Spinner } from 'react-bootstrap';

type FormModalProps = {
  show: boolean;
  title: string;
  submitLabel: string; // pl. 'Vevő mentése'
  saving: boolean;
  error: string | null; // a mezők fölött megjelenő hibaüzenet (pl. a backend validációja)
  onSubmit: () => void;
  onHide: () => void;
  submitDisabled?: boolean;
  size?: 'sm' | 'lg' | 'xl';
  // true: a böngésző beépített ellenőrzése helyett a hívó validál (pl. mezőnkénti hibaüzenetekkel)
  noValidate?: boolean;
  children: ReactNode; // a mezők
};

// Űrlapos modal közös váza: fejléc, hibasáv, Mégse / mentés gombok. Mentés közben nem zárható be.
export default function FormModal({
  show,
  title,
  submitLabel,
  saving,
  error,
  onSubmit,
  onHide,
  submitDisabled = false,
  size,
  noValidate,
  children,
}: FormModalProps) {
  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    if (!saving) onSubmit();
  };

  return (
    <Modal show={show} onHide={() => { if (!saving) onHide(); }} {...(size ? { size } : {})} centered>
      <Form onSubmit={handleSubmit} noValidate={noValidate}>
        <Modal.Header closeButton={!saving}>
          <Modal.Title>{title}</Modal.Title>
        </Modal.Header>

        <Modal.Body>
          {error && <Alert variant="danger">{error}</Alert>}
          {children}
        </Modal.Body>

        <Modal.Footer>
          <Button variant="secondary" onClick={onHide} disabled={saving}>
            Mégse
          </Button>
          <Button type="submit" variant="primary" disabled={saving || submitDisabled}>
            {saving ? (
              <>
                <Spinner animation="border" size="sm" className="me-2" aria-hidden="true" />
                Mentés...
              </>
            ) : (
              submitLabel
            )}
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
}
