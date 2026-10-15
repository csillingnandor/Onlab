import { useRef, useState } from 'react';
import { Overlay, Spinner, Tooltip } from 'react-bootstrap';
import { exportList, type ExportKind } from '../../services/exportService.js';
import './HeaderButton.css';

type ExportButtonProps = {
  kind: ExportKind;
};

// Fejlécgomb: a lista letöltése Excelben (a szerver állítja elő, a szűrés nélküli teljes listát).
// Hiba esetén néhány másodpercre buborékban jelenik meg az üzenet.
export default function ExportButton({ kind }: ExportButtonProps) {
  const target = useRef<HTMLButtonElement>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleClick = async () => {
    setBusy(true);
    setError(null);
    try {
      await exportList(kind);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Nem sikerült az exportálás.');
      setTimeout(() => setError(null), 4000);
    } finally {
      setBusy(false);
    }
  };

  return (
    <>
      <button
        ref={target}
        type="button"
        className="header-btn"
        onClick={handleClick}
        disabled={busy}
        title="Letöltés Excelben (.xlsx)"
      >
        {busy ? <Spinner animation="border" size="sm" aria-hidden="true" /> : <i className="bi bi-file-earmark-excel" aria-hidden="true"></i>}
        Export
      </button>
      <Overlay target={target.current} show={error !== null} placement="bottom">
        <Tooltip id={`export-error-${kind}`}>{error}</Tooltip>
      </Overlay>
    </>
  );
}
