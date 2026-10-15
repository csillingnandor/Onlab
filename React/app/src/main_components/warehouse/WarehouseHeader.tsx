import type { WarehouseViewMode } from '../../util/Types.js';
import ViewModeToggle, { type ViewModeOption } from '../../common/button/ViewModeToggle.js';
import FilterToggle from '../../common/filter/FilterToggle.js';
import ExportButton from '../../common/button/ExportButton.js';

const viewModeOptions: ViewModeOption<WarehouseViewMode>[] = [
  { value: 'table', label: 'Táblázat nézet', icon: 'bi-table' },
  { value: 'map', label: 'Térkép nézet', icon: 'bi-map' },
];

type WarehouseHeaderProps = {
  subtitle: string; // pl. '3 / 5 raktár'
  viewMode: WarehouseViewMode;
  onViewModeChange: (mode: WarehouseViewMode) => void;
  filterOpen: boolean;
  activeFilterCount: number;
  filterPanelId: string;
  onFilterToggle: () => void;
  onAddClick?: (() => void) | undefined; // nincs megadva: nincs létrehozási jog, a gomb nem látszik
};

export default function WarehouseHeader({
  subtitle,
  viewMode,
  onViewModeChange,
  filterOpen,
  activeFilterCount,
  filterPanelId,
  onFilterToggle,
  onAddClick,
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

        <ViewModeToggle options={viewModeOptions} value={viewMode} onChange={onViewModeChange} />
        <ExportButton kind="warehouses" />
        {onAddClick && (
          <button type="button" className="header-btn header-btn-primary" onClick={onAddClick}>
            <i className="bi bi-plus-lg" aria-hidden="true"></i>
            Új raktár
          </button>
        )}
      </div>
    </header>
  );
}
