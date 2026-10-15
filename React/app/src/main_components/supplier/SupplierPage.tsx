import { useEffect, useMemo, useState } from 'react';
import { Form, Spinner } from 'react-bootstrap';
import supplierService from '../../services/supplierService.js';
import type { Supplier } from '../../util/Types.js';
import ActionButtons from '../../common/button/ActionButtons.js';
import ExportButton from '../../common/button/ExportButton.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import { useAuth } from '../../auth/useAuth.js';
import { useQueryFlag } from '../../util/useQueryFlag.js';
import SupplierFormModal from './SupplierFormModal.js';
import '../../common/table/DataTable.css';
import '../customer/CustomerPage.css';

const missing = <span className="text-muted">–</span>;
const byName = (a: Supplier, b: Supplier) => a.name.localeCompare(b.name, 'hu');

// Beszállítók törzsadatai (a beszerzési rendelések partnerei)
export default function SupplierPage() {
  const { hasAction } = useAuth();
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  // undefined: zárva, null: új beszállító, egyébként a szerkesztett
  const [editing, setEditing] = useState<Supplier | null | undefined>(undefined);
  const [deleting, setDeleting] = useState<Supplier | null>(null);
  const [newRequested, consumeNew] = useQueryFlag('new');

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    supplierService.getAll()
      .then((data) => {
        if (!ignore) setSuppliers(data);
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.');
      })
      .finally(() => {
        if (!ignore) setLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, []);

  useEffect(() => {
    if (!newRequested) return;
    if (hasAction('suppliers.create')) setEditing(null);
    consumeNew();
  }, [newRequested, consumeNew, hasAction]);

  // Keresés név, e-mail és telefon szerint
  const visible = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('hu');
    if (!term) return suppliers;
    return suppliers.filter((s) =>
      [s.name, s.email ?? '', s.phone ?? ''].some((field) => field.toLocaleLowerCase('hu').includes(term)));
  }, [suppliers, search]);

  const canEdit = hasAction('suppliers.edit');
  const canDelete = hasAction('suppliers.delete');

  const handleSaved = (supplier: Supplier) => {
    setSuppliers((prev) => [...prev.filter((s) => s.id !== supplier.id), supplier].sort(byName));
    setEditing(undefined);
  };

  // 409 (vannak rendelései) esetén a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmDelete = async () => {
    if (!deleting) return;
    await supplierService.delete(deleting.id);
    setSuppliers((prev) => prev.filter((s) => s.id !== deleting.id));
    setDeleting(null);
  };

  const renderContent = () => {
    if (loading) {
      return (
        <div className="text-center py-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Beszállítók betöltése...</span>
          </Spinner>
        </div>
      );
    }
    if (error) return <div className="alert alert-danger" role="alert">{error}</div>;
    if (suppliers.length === 0) return <div className="alert alert-info text-center" role="alert">Még nincsenek beszállítók.</div>;
    if (visible.length === 0) return <div className="alert alert-info text-center" role="status">Nincs a keresésnek megfelelő beszállító.</div>;

    return (
      <div className="data-table-wrapper">
        <table className="data-table">
          <thead>
            <tr>
              <th>Név</th>
              <th>E-mail</th>
              <th>Telefon</th>
              <th className="numeric">Rendelések</th>
              {(canEdit || canDelete) && <th className="numeric">Műveletek</th>}
            </tr>
          </thead>
          <tbody>
            {visible.map((supplier) => (
              <tr key={supplier.id}>
                <td className="data-table-strong">{supplier.name}</td>
                <td>
                  {supplier.email
                    ? <a href={`mailto:${supplier.email}`} className="customer-email">{supplier.email}</a>
                    : missing}
                </td>
                <td className="data-table-mono">{supplier.phone ?? missing}</td>
                <td className="numeric">{supplier.orderCount} db</td>
                {(canEdit || canDelete) && (
                  <td className="numeric">
                    <ActionButtons
                      itemLabel={supplier.name}
                      onEdit={canEdit ? () => setEditing(supplier) : undefined}
                      onDelete={canDelete ? () => setDeleting(supplier) : undefined}
                    />
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  };

  return (
    <div className="customer-page">
      <header className="customer-page-header">
        <div>
          <h1>Beszállítók</h1>
          <p className="customer-page-subtitle">
            {search.trim() ? `${visible.length} / ${suppliers.length}` : suppliers.length} beszállító
          </p>
        </div>
        <div className="customer-header-actions">
          <Form.Control
            type="search"
            placeholder="Keresés…"
            aria-label="Beszállító keresése"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            style={{ width: '14rem' }}
          />
          <ExportButton kind="suppliers" />
          {hasAction('suppliers.create') && (
            <button type="button" className="header-btn header-btn-primary" onClick={() => setEditing(null)}>
              <i className="bi bi-plus-lg" aria-hidden="true"></i>
              Új beszállító
            </button>
          )}
        </div>
      </header>

      {renderContent()}

      <SupplierFormModal
        show={editing !== undefined}
        supplier={editing ?? null}
        onSaved={handleSaved}
        onHide={() => setEditing(undefined)}
      />
      <ConfirmModal
        show={deleting !== null}
        title="Beszállító törlése"
        confirmLabel="Törlés"
        confirmIcon="bi-trash"
        busyLabel="Törlés..."
        onConfirm={handleConfirmDelete}
        onHide={() => setDeleting(null)}
      >
        {deleting && (
          <>
            <p className="mb-2">Biztosan törlöd ezt a beszállítót?</p>
            <p className="mb-2"><strong>{deleting.name}</strong></p>
            <p className="mb-0 small text-secondary">Csak rendelés nélküli beszállító törölhető. A művelet nem vonható vissza.</p>
          </>
        )}
      </ConfirmModal>
    </div>
  );
}
