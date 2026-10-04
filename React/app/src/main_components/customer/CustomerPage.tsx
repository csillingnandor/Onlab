import { useEffect, useState } from 'react';
import { Spinner } from 'react-bootstrap';
import customerService from '../../services/customerService.js';
import type { Customer } from '../../util/Types.js';
import CustomerTable from './CustomerTable.js';
import './CustomerPage.css';

export default function CustomerPage() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

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

    return <CustomerTable customers={customers} />;
  };

  return (
    <div className="customer-page">
      <header className="customer-page-header">
        <h1>Vevők</h1>
        <p className="customer-page-subtitle">{customers.length} vevő</p>
      </header>
      {renderContent()}
    </div>
  );
}
