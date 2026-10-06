import { useEffect, useMemo, useState } from 'react';
import { Spinner } from 'react-bootstrap';
import customerService from '../../services/customerService.js';
import type { Customer } from '../../util/Types.js';
import { emptyCustomerFilter, filterCustomers, type CustomerFilter } from '../../util/customerFilter.js';
import { countActive } from '../../util/filterUtils.js';
import FilterToggle from '../filter/FilterToggle.js';
import CreateCustomerModal from './CreateCustomerModal.js';
import CustomerFilterPanel from './CustomerFilterPanel.js';
import CustomerTable from './CustomerTable.js';
import './CustomerPage.css';

const FILTER_PANEL_ID = 'customer-filter';

export default function CustomerPage() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<CustomerFilter>(emptyCustomerFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);
  const [createOpen, setCreateOpen] = useState<boolean>(false);

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

  // A lista név szerint rendezett (mint a backendnél), az új vevő is a helyére kerül
  const handleCreated = (customer: Customer) => {
    setCustomers((prev) => [...prev, customer].sort((a, b) => a.name.localeCompare(b.name, 'hu')));
    setCreateOpen(false);
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

    return <CustomerTable customers={visibleCustomers} />;
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
          <button type="button" className="add-product-btn" onClick={() => setCreateOpen(true)}>
            <i className="bi bi-plus-lg me-1" aria-hidden="true"></i>
            Új vevő
          </button>
        </div>
      </header>
      <CreateCustomerModal show={createOpen} onCreated={handleCreated} onHide={() => setCreateOpen(false)} />
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
