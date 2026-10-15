import type { ViewMode } from '../../util/Types.js';
import ViewModeToggle, { type ViewModeOption } from '../../common/button/ViewModeToggle.js';
import FilterToggle from '../../common/filter/FilterToggle.js';
import ExportButton from '../../common/button/ExportButton.js';

const viewModeOptions: ViewModeOption<ViewMode>[] = [
  { value: 'list', label: 'Lista nézet', icon: 'bi-list' },
  { value: 'grid', label: 'Rács nézet', icon: 'bi-grid' },
];

type ProductHeaderProps = {
  subtitle: string; // pl. '12 / 20 termék'
  viewMode: ViewMode;
  onViewModeChange: (mode: ViewMode) => void;
  filterOpen: boolean;
  activeFilterCount: number;
  filterPanelId: string;
  onFilterToggle: () => void;
  onAddProductClick?: (() => void) | undefined; // nincs megadva: nincs létrehozási jog, a gomb nem látszik
  onImportClick?: (() => void) | undefined; // nincs megadva: nincs importálási jog
};

export default function ProductHeader({
    subtitle,
    viewMode,
    onViewModeChange,
    filterOpen,
    activeFilterCount,
    filterPanelId,
    onFilterToggle,
    onAddProductClick,
    onImportClick,
}: ProductHeaderProps) {

    return (
        <header className="product-header">
            <div>
                <h1>Termékek</h1>
                <p className="product-header-subtitle">{subtitle}</p>
            </div>
            <FilterToggle
                open={filterOpen}
                activeCount={activeFilterCount}
                controls={filterPanelId}
                onToggle={onFilterToggle}
            />
            <ViewModeToggle options={viewModeOptions} value={viewMode} onChange={onViewModeChange} />
            <ExportButton kind="products" />
            {onImportClick && (
                <button type="button" className="header-btn" onClick={onImportClick} title="Termékek importálása Excelből">
                    <i className="bi bi-upload" aria-hidden="true"></i>
                    Import
                </button>
            )}
            {onAddProductClick && (
                <button type="button" className="header-btn header-btn-primary" onClick={onAddProductClick}>
                    <i className="bi bi-plus-lg" aria-hidden="true"></i>
                    Új termék hozzáadása
                </button>
            )}
        </header>
    )
}