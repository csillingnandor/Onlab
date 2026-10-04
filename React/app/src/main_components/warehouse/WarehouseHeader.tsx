import type { WarehouseViewMode } from '../../util/Types.js';

type WarehouseHeaderProps = {
  warehouseCount: number;
  viewMode: WarehouseViewMode;
  onViewModeChange: (mode: WarehouseViewMode) => void;
};

export default function WarehouseHeader({ warehouseCount, viewMode, onViewModeChange }: WarehouseHeaderProps) {
  return (
    <header className="warehouse-page-header">
      <div>
        <h1>Raktárak</h1>
        <p className="warehouse-page-subtitle">{warehouseCount} raktár</p>
      </div>

      {/* Ugyanaz a nézetváltó, mint a termékoldalon (MainContent.css) */}
      <div className="view-mode-toggle">
        <button
          type="button"
          className={viewMode === 'table' ? 'active' : ''}
          onClick={() => onViewModeChange('table')}
          aria-pressed={viewMode === 'table'}
          aria-label="Táblázat nézet"
          title="Táblázat nézet"
        >
          <i className="bi bi-table"></i>
        </button>
        <button
          type="button"
          className={viewMode === 'map' ? 'active' : ''}
          onClick={() => onViewModeChange('map')}
          aria-pressed={viewMode === 'map'}
          aria-label="Térkép nézet"
          title="Térkép nézet"
        >
          <i className="bi bi-map"></i>
        </button>
      </div>
    </header>
  );
}
