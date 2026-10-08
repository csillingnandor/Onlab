import { useEffect, useState, type ReactNode } from 'react';
import { Alert, Button, Modal, Spinner } from 'react-bootstrap';

type ConfirmModalProps = {
  show: boolean;
  title: string; // pl. 'Termék törlése'
  confirmLabel: string; // pl. 'Törlés'
  confirmIcon?: string; // Bootstrap Icons osztály, pl. 'bi-trash'
  busyLabel?: string; // a megerősítő gomb felirata a művelet közben
  variant?: 'danger' | 'primary';
  // Ha hibát dob, az üzenete a modalban jelenik meg, és a megerősítő gomb letiltódik
  // (pl. a backend 409-cel elutasította a törlést; újrapróbálni ugyanígy nincs értelme).
  onConfirm: () => Promise<void>;
  onHide: () => void;
  children: ReactNode; // a kérdés és a részletek
};

// Megerősítő modal aszinkron művelethez (törlés, lemondás, ...); a művelet alatt nem zárható be.
export default function ConfirmModal({
  show,
  title,
  confirmLabel,
  confirmIcon,
  busyLabel = 'Folyamatban...',
  variant = 'danger',
  onConfirm,
  onHide,
  children,
}: ConfirmModalProps) {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Minden megnyitáskor tiszta állapot
  useEffect(() => {
    if (show) setError(null);
  }, [show]);

  const handleConfirm = async () => {
    setBusy(true);
    setError(null);
    try {
      await onConfirm();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt a művelet során.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <Modal show={show} onHide={() => { if (!busy) onHide(); }} centered>
      <Modal.Header closeButton={!busy}>
        <Modal.Title>{title}</Modal.Title>
      </Modal.Header>

      <Modal.Body>
        {error && <Alert variant="danger">{error}</Alert>}
        {children}
      </Modal.Body>

      <Modal.Footer>
        <Button variant="secondary" onClick={onHide} disabled={busy}>
          Mégse
        </Button>
        <Button variant={variant} onClick={handleConfirm} disabled={busy || error !== null}>
          {busy ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" aria-hidden="true" />
              {busyLabel}
            </>
          ) : (
            <>
              {confirmIcon && <i className={`bi ${confirmIcon} me-1`} aria-hidden="true"></i>}
              {confirmLabel}
            </>
          )}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
