import { useEffect, useState } from 'react';
import { Alert, Badge, Button, Form, Modal, Spinner, Table } from 'react-bootstrap';
import productService from '../../services/productService.js';
import type { ProductImportPreview, ProductImportResult, ProductImportRow } from '../../util/Types.js';
import { priceFormatter } from '../../util/format.js';
import './ProductImportModal.css';

type ProductImportModalProps = {
  show: boolean;
  onImported: (result: ProductImportResult) => void;
  onHide: () => void;
};

// Mezőnév (backend ProductImportRowData) -> oszlop a táblázatban
const columns: { field: keyof ProductImportRow & string; label: string }[] = [
  { field: 'name', label: 'Név' },
  { field: 'sku', label: 'SKU' },
  { field: 'category', label: 'Kategória' },
  { field: 'price', label: 'Ár' },
  { field: 'minStockLevel', label: 'Min. készlet' },
];

// A backend nagybetűs mezőneveket ad (Name, Sku...); a táblázat kisbetűs kulcsokkal dolgozik
const fieldErrors = (row: ProductImportRow, field: string) =>
  Object.entries(row.errors).find(([key]) => key.toLowerCase() === field.toLowerCase())?.[1] ?? [];

const rowErrors = (row: ProductImportRow) =>
  Object.entries(row.errors)
    .filter(([key]) => !columns.some((c) => c.field.toLowerCase() === key.toLowerCase()))
    .flatMap(([, messages]) => messages);

const display = (row: ProductImportRow, field: string) => {
  const value = row[field as keyof ProductImportRow];
  if (value === null || value === undefined || value === '') return <span className="text-muted">–</span>;
  return field === 'price' && typeof value === 'number' ? priceFormatter.format(value) : String(value);
};

// Termékek tömeges felvétele / frissítése Excelből. Feltöltés után előnézet: a sorok és a hibáik a mentés előtt
// látszanak (hibás cellák pirossal); mentés csak hibátlan fájllal lehetséges.
export default function ProductImportModal({ show, onImported, onHide }: ProductImportModalProps) {
  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<ProductImportPreview | null>(null);
  const [busy, setBusy] = useState<'preview' | 'import' | 'template' | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [onlyErrors, setOnlyErrors] = useState(false);

  useEffect(() => {
    if (show) {
      setFile(null);
      setPreview(null);
      setError(null);
      setOnlyErrors(false);
    }
  }, [show]);

  const run = async (kind: 'preview' | 'import' | 'template', action: () => Promise<void>) => {
    setBusy(kind);
    setError(null);
    try {
      await action();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Hiba történt.');
    } finally {
      setBusy(null);
    }
  };

  // A fájl kiválasztása után azonnal előnézet
  const handleFile = (selected: File | null) => {
    setFile(selected);
    setPreview(null);
    if (selected) void run('preview', async () => setPreview(await productService.previewImport(selected)));
  };

  const handleImport = () => {
    if (!file) return;
    void run('import', async () => onImported(await productService.importProducts(file)));
  };

  const canImport = preview !== null && preview.schemaErrors.length === 0 && preview.errorCount === 0
    && preview.rows.length > 0 && busy === null;
  const rows = preview ? (onlyErrors ? preview.rows.filter((r) => Object.keys(r.errors).length > 0) : preview.rows) : [];

  return (
    <Modal show={show} onHide={() => { if (busy !== 'import') onHide(); }} size="xl" centered scrollable>
      <Modal.Header closeButton={busy !== 'import'}>
        <Modal.Title>Termékek importálása Excelből</Modal.Title>
      </Modal.Header>

      <Modal.Body>
        <p className="mb-2">
          Az első munkalap első sora a fejléc: <strong>Név</strong>, <strong>SKU</strong>, <strong>Ár</strong> (kötelező),
          valamint <strong>Kategória</strong> és <strong>Minimális készlet</strong>. A már létező SKU-jú termékek adatai frissülnek,
          a többi új termékként jön létre (0 készlettel).
        </p>
        <div className="d-flex flex-wrap gap-2 align-items-center mb-3">
          <Form.Control
            type="file"
            accept=".xlsx,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            onChange={(e) => handleFile((e.target as HTMLInputElement).files?.[0] ?? null)}
            disabled={busy !== null}
            style={{ maxWidth: '28rem' }}
            aria-label="Excel fájl kiválasztása"
          />
          <Button
            variant="outline-secondary"
            size="sm"
            onClick={() => void run('template', productService.downloadImportTemplate)}
            disabled={busy !== null}
          >
            <i className="bi bi-download me-1" aria-hidden="true"></i>
            Sablon letöltése
          </Button>
          {busy === 'preview' && <Spinner animation="border" size="sm" role="status"><span className="visually-hidden">Feldolgozás...</span></Spinner>}
        </div>

        {error && <Alert variant="danger">{error}</Alert>}

        {preview && (
          <>
            {preview.schemaErrors.length > 0 && (
              <Alert variant="danger">
                <strong>A fájl szerkezete hibás:</strong>
                <ul className="mb-0">{preview.schemaErrors.map((e) => <li key={e}>{e}</li>)}</ul>
              </Alert>
            )}
            {preview.warnings.length > 0 && (
              <Alert variant="warning" className="py-2">
                <ul className="mb-0">{preview.warnings.map((w) => <li key={w}>{w}</li>)}</ul>
              </Alert>
            )}

            {preview.rows.length > 0 && (
              <>
                <div className="d-flex flex-wrap gap-2 align-items-center mb-2">
                  <Badge bg="success">{preview.createCount} új</Badge>
                  <Badge bg="info">{preview.updateCount} frissítés</Badge>
                  <Badge bg={preview.errorCount > 0 ? 'danger' : 'secondary'}>{preview.errorCount} hibás sor</Badge>
                  {preview.errorCount > 0 && (
                    <Form.Check
                      type="switch"
                      id="import-only-errors"
                      label="Csak a hibás sorok"
                      checked={onlyErrors}
                      onChange={(e) => setOnlyErrors(e.target.checked)}
                      className="ms-2"
                    />
                  )}
                </div>
                <div className="product-import-table-wrapper">
                  <Table size="sm" className="product-import-table align-middle mb-0">
                    <thead>
                      <tr>
                        <th>Sor</th>
                        <th>Művelet</th>
                        {columns.map((c) => <th key={c.field}>{c.label}</th>)}
                        <th>Hibák</th>
                      </tr>
                    </thead>
                    <tbody>
                      {rows.map((row) => {
                        const hasErrors = Object.keys(row.errors).length > 0;
                        const messages = [...columns.flatMap((c) => fieldErrors(row, c.field)), ...rowErrors(row)];
                        return (
                          <tr key={row.rowNumber} className={hasErrors ? 'product-import-row-error' : ''}>
                            <td className="text-muted">{row.rowNumber}</td>
                            <td>
                              {hasErrors
                                ? <Badge bg="danger">Hibás</Badge>
                                : row.action === 'Update' ? <Badge bg="info">Frissítés</Badge> : <Badge bg="success">Új</Badge>}
                            </td>
                            {columns.map((c) => (
                              <td
                                key={c.field}
                                className={fieldErrors(row, c.field).length > 0 ? 'product-import-cell-error' : ''}
                                title={fieldErrors(row, c.field).join(' ') || undefined}
                              >
                                {display(row, c.field)}
                              </td>
                            ))}
                            <td className="product-import-messages">{messages.join(' ')}</td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </Table>
                </div>
              </>
            )}
          </>
        )}
      </Modal.Body>

      <Modal.Footer>
        {preview && preview.errorCount > 0 && (
          <span className="me-auto small text-danger">Javítsd a hibás sorokat az Excelben, majd töltsd fel újra.</span>
        )}
        <Button variant="secondary" onClick={onHide} disabled={busy === 'import'}>Mégse</Button>
        <Button variant="primary" onClick={handleImport} disabled={!canImport}>
          {busy === 'import' ? (
            <><Spinner animation="border" size="sm" className="me-2" aria-hidden="true" />Importálás...</>
          ) : (
            <><i className="bi bi-upload me-1" aria-hidden="true"></i>
              {preview && preview.rows.length > 0 && preview.errorCount === 0
                ? `Importálás (${preview.createCount} új, ${preview.updateCount} frissítés)`
                : 'Importálás'}</>
          )}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
