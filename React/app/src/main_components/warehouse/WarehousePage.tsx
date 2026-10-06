import { useEffect, useMemo, useState } from 'react';
import { Spinner } from 'react-bootstrap';
import warehouseService from '../../services/warehouseService.js';
import type { Warehouse } from '../../util/Types.js';
import { countActive } from '../../util/filterUtils.js';
import { emptyWarehouseFilter, filterWarehouses, type WarehouseFilter } from '../../util/warehouseFilter.js';
import WarehouseFilterPanel from './WarehouseFilterPanel.js';
import WarehouseHeader from './WarehouseHeader.js';
import WarehouseMap from './WarehouseMap.js';
import WarehouseTable from './WarehouseTable.js';
import './WarehousePage.css';

const FILTER_PANEL_ID = 'warehouse-filter';

export default function WarehousePage() {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'table' | 'map'>('table');
  const [filter, setFilter] = useState<WarehouseFilter>(emptyWarehouseFilter);
  const [filterOpen, setFilterOpen] = useState<boolean>(false);

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

  // Kliensoldali szűrés; a táblázat és a térkép is a szűrt listát kapja
  const visibleWarehouses = useMemo(() => filterWarehouses(warehouses, filter), [warehouses, filter]);
  const activeFilterCount = countActive(filter);

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

    if (visibleWarehouses.length === 0) {
      return <div className="alert alert-info text-center" role="status">Nincs a szűrésnek megfelelő raktár.</div>;
    }

    // Feltételes renderelés (nem CSS-es elrejtés): a térkép rejtett konténerben rossz méretet venne fel.
    return viewMode === 'table'
      ? <WarehouseTable warehouses={visibleWarehouses} />
      : <WarehouseMap warehouses={visibleWarehouses} />;
  };

  return (
    <div className="warehouse-page">
      <WarehouseHeader
        subtitle={`${activeFilterCount > 0 ? `${visibleWarehouses.length} / ${warehouses.length}` : warehouses.length} raktár`}
        viewMode={viewMode}
        onViewModeChange={setViewMode}
        filterOpen={filterOpen}
        activeFilterCount={activeFilterCount}
        filterPanelId={FILTER_PANEL_ID}
        onFilterToggle={() => setFilterOpen((prev) => !prev)}
      />
      {filterOpen && (
        <WarehouseFilterPanel
          id={FILTER_PANEL_ID}
          value={filter}
          onChange={setFilter}
          onClear={() => setFilter(emptyWarehouseFilter)}
        />
      )}
      {renderContent()}
    </div>
  );
}
