import { useEffect, useState } from 'react';
import { Spinner } from 'react-bootstrap';
import warehouseService from '../../services/warehouseService.js';
import type { Warehouse } from '../../util/Types.js';
import WarehouseHeader from './WarehouseHeader.js';
import WarehouseMap from './WarehouseMap.js';
import WarehouseTable from './WarehouseTable.js';
import './WarehousePage.css';

export default function WarehousePage() {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'table' | 'map'>('table');

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    warehouseService.getAll()
      .then((data) => {
        if (!ignore) setWarehouses(data);
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
            <span className="visually-hidden">Raktárak betöltése...</span>
          </Spinner>
        </div>
      );
    }

    if (error) return <div className="alert alert-danger" role="alert">{error}</div>;

    if (warehouses.length === 0) {
      return <div className="alert alert-info text-center" role="alert">Nincsenek raktárak.</div>;
    }

    // Feltételes renderelés (nem CSS-es elrejtés): a térkép rejtett konténerben rossz méretet venne fel.
    return viewMode === 'table'
      ? <WarehouseTable warehouses={warehouses} />
      : <WarehouseMap warehouses={warehouses} />;
  };

  return (
    <div className="warehouse-page">
      <WarehouseHeader
        warehouseCount={warehouses.length}
        viewMode={viewMode}
        onViewModeChange={setViewMode}
      />
      {renderContent()}
    </div>
  );
}
