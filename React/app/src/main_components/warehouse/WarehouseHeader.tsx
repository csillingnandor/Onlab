import type { WarehouseViewMode } from '../../util/Types.js';
import FilterToggle from '../filter/FilterToggle.js';

type WarehouseHeaderProps = {
  subtitle: string; // pl. '3 / 5 raktár'
  viewMode: WarehouseViewMode;
  onViewModeChange: (mode: WarehouseViewMode) => void;
  filterOpen: boolean;
  activeFilterCount: number;
  filterPanelId: string;
  onFilterToggle: () => void;
};

export default function WarehouseHeader({
  subtitle,
  viewMode,
  onViewModeChange,
  filterOpen,
  activeFilterCount,
  filterPanelId,
  onFilterToggle,
}: WarehouseHeaderProps) {
  return (
    <header className="warehouse-page-header">
      <div>
        <h1>Raktárak</h1>
        <p className="warehouse-page-subtitle">{subtitle}</p>
      </div>

      <div className="warehouse-header-actions">
        <FilterToggle
          open={filterOpen}
          activeCount={activeFilterCount}
          controls={filterPanelId}
          onToggle={onFilterToggle}
        />

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
      </div>
    </header>
  );
}
