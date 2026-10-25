import { useEffect, useMemo, useState } from 'react';
import { Spinner } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import warehouseService from '../../services/warehouseService.js';
import type { Warehouse } from '../../util/Types.js';
import { countActive } from '../../common/filter/filterUtils.js';
import { emptyWarehouseFilter, filterWarehouses, type WarehouseFilter } from '../../util/warehouseFilter.js';
import WarehouseFilterPanel from './WarehouseFilterPanel.js';
import WarehouseHeader from './WarehouseHeader.js';
import WarehouseMap from './WarehouseMap.js';
import WarehouseTable from './WarehouseTable.js';
import WarehouseFormModal from './WarehouseFormModal.js';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import { useAuth } from '../../auth/useAuth.js';
import './WarehousePage.css';

const FILTER_PANEL_ID = 'warehouse-filter';

const byName = (a: Warehouse, b: Warehouse) => a.name.localeCompare(b.name, 'hu');

export default function WarehousePage() {
  const { hasAction } = useAuth();
  const navigate = useNavigate();
  // undefined: zárva, null: új raktár, egyébként a szerkesztett
  const [editing, setEditing] = useState<Warehouse | null | undefined>(undefined);
  const [deleting, setDeleting] = useState<Warehouse | null>(null);
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

  const handleSaved = (warehouse: Warehouse) => {
    setWarehouses((prev) => [...prev.filter((w) => w.id !== warehouse.id), warehouse].sort(byName));
    setEditing(undefined);
  };

  // 409 (készlet / rendelés hivatkozik rá) esetén a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmDelete = async () => {
    if (!deleting) return;
    await warehouseService.delete(deleting.id);
    setWarehouses((prev) => prev.filter((w) => w.id !== deleting.id));
    setDeleting(null);
  };

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
      ? (
        <WarehouseTable
          warehouses={visibleWarehouses}
          onEdit={hasAction('warehouses.edit') ? setEditing : undefined}
          onDelete={hasAction('warehouses.delete') ? setDeleting : undefined}
          onShowLayout={(warehouse) => navigate(`/warehouse/${warehouse.id}/layout`)}
        />
      )
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
        onAddClick={hasAction('warehouses.create') ? () => setEditing(null) : undefined}
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

      <WarehouseFormModal
        show={editing !== undefined}
        warehouse={editing ?? null}
        onSaved={handleSaved}
        onHide={() => setEditing(undefined)}
      />
      <ConfirmModal
        show={deleting !== null}
        title="Raktár törlése"
        confirmLabel="Törlés"
        confirmIcon="bi-trash"
        busyLabel="Törlés..."
        onConfirm={handleConfirmDelete}
        onHide={() => setDeleting(null)}
      >
        {deleting && (
          <>
            <p className="mb-2">Biztosan törlöd ezt a raktárat?</p>
            <p className="mb-2"><strong>{deleting.name}</strong></p>
            <p className="mb-0 small text-secondary">
              Csak olyan raktár törölhető, amelyben nincs készlet, és amelyre rendelés vagy készletmozgás sem hivatkozik.
            </p>
          </>
        )}
      </ConfirmModal>
    </div>
  );
}
