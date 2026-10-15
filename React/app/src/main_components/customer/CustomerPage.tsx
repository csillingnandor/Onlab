import { useEffect, useMemo, useState } from 'react';
import { Spinner } from 'react-bootstrap';
import customerService from '../../services/customerService.js';
import type { Customer } from '../../util/Types.js';
import { emptyCustomerFilter, filterCustomers, type CustomerFilter } from '../../util/customerFilter.js';
import { countActive } from '../../common/filter/filterUtils.js';
import FilterToggle from '../../common/filter/FilterToggle.js';
import CustomerFormModal from './CustomerFormModal.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import ExportButton from '../../common/button/ExportButton.js';
import { useQueryFlag } from '../../util/useQueryFlag.js';
import CustomerFilterPanel from './CustomerFilterPanel.js';
import CustomerTable from './CustomerTable.js';
import { useAuth } from '../../auth/useAuth.js';
import './CustomerPage.css';

const FILTER_PANEL_ID = 'customer-filter';

export default function CustomerPage() {
  const { hasAction } = useAuth();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<CustomerFilter>(emptyCustomerFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);
  // undefined: zárva, null: új vevő, egyébként a szerkesztett vevő
  const [editing, setEditing] = useState<Customer | null | undefined>(undefined);
  const [deleting, setDeleting] = useState<Customer | null>(null);
  const [newRequested, consumeNew] = useQueryFlag('new');

  // A műszerfal gyorsgombja (?new=1) rögtön az új vevő űrlapot nyitja
  useEffect(() => {
    if (!newRequested) return;
    if (hasAction('customers.create')) setEditing(null);
    consumeNew();
  }, [newRequested, consumeNew, hasAction]);

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    customerService.getAll()
      .then((data) => {
        if (!ignore) setCustomers(data);
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

  // Kliensoldali szűrés a betöltött listán
  const visibleCustomers = useMemo(() => filterCustomers(customers, filter), [customers, filter]);
  const activeFilterCount = countActive(filter);

  // A lista név szerint rendezett (mint a backendnél), az új / átnevezett vevő is a helyére kerül
  const handleSaved = (customer: Customer) => {
    setCustomers((prev) =>
      [...prev.filter((c) => c.id !== customer.id), customer].sort((a, b) => a.name.localeCompare(b.name, 'hu')),
    );
    setEditing(undefined);
  };

  // 409 (vannak rendelései) esetén a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmDelete = async () => {
    if (!deleting) return;
    await customerService.delete(deleting.id);
    setCustomers((prev) => prev.filter((c) => c.id !== deleting.id));
    setDeleting(null);
  };

  const renderContent = () => {
    if (loading) {
      return (
        <div className="text-center py-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Vevők betöltése...</span>
          </Spinner>
        </div>
      );
    }

    if (error) return <div className="alert alert-danger" role="alert">{error}</div>;

    if (customers.length === 0) {
      return <div className="alert alert-info text-center" role="alert">Még nincsenek vevők.</div>;
    }

    if (visibleCustomers.length === 0) {
      return <div className="alert alert-info text-center" role="status">Nincs a szűrésnek megfelelő vevő.</div>;
    }

    return (
      <CustomerTable
        customers={visibleCustomers}
        onEdit={hasAction('customers.edit') ? setEditing : undefined}
        onDelete={hasAction('customers.delete') ? setDeleting : undefined}
      />
    );
  };

  return (
    <div className="customer-page">
      <header className="customer-page-header">
        <div>
          <h1>Vevők</h1>
          <p className="customer-page-subtitle">
            {activeFilterCount > 0 ? `${visibleCustomers.length} / ${customers.length}` : customers.length} vevő
          </p>
        </div>
        <div className="customer-header-actions">
          <FilterToggle
            open={filterOpen}
            activeCount={activeFilterCount}
            controls={FILTER_PANEL_ID}
            onToggle={() => setFilterOpen((prev) => !prev)}
          />
          <ExportButton kind="customers" />
          {hasAction('customers.create') && (
            <button type="button" className="header-btn header-btn-primary" onClick={() => setEditing(null)}>
              <i className="bi bi-plus-lg" aria-hidden="true"></i>
              Új vevő
            </button>
          )}
        </div>
      </header>
      <CustomerFormModal
        show={editing !== undefined}
        customer={editing ?? null}
        onSaved={handleSaved}
        onHide={() => setEditing(undefined)}
      />
      <ConfirmModal
        show={deleting !== null}
        title="Vevő törlése"
        confirmLabel="Törlés"
        confirmIcon="bi-trash"
        busyLabel="Törlés..."
        onConfirm={handleConfirmDelete}
        onHide={() => setDeleting(null)}
      >
        {deleting && (
          <>
            <p className="mb-2">Biztosan törlöd ezt a vevőt?</p>
            <p className="mb-2"><strong>{deleting.name}</strong> <span className="text-secondary ms-2">{deleting.email}</span></p>
            <p className="mb-0 small text-secondary">Csak rendelés nélküli vevő törölhető. A művelet nem vonható vissza.</p>
          </>
        )}
      </ConfirmModal>
      {filterOpen && (
        <CustomerFilterPanel
          id={FILTER_PANEL_ID}
          value={filter}
          onChange={setFilter}
          onClear={() => setFilter(emptyCustomerFilter)}
        />
      )}
      {renderContent()}
    </div>
  );
}
