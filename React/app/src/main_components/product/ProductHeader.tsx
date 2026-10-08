import type { ViewMode } from '../../util/Types.js';
import ViewModeToggle, { type ViewModeOption } from '../../common/button/ViewModeToggle.js';
import FilterToggle from '../../common/filter/FilterToggle.js';

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
  onAddProductClick?: () => void;
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
            <button className="add-product-btn" onClick={onAddProductClick}>
                Új termék hozzáadása
            </button>
        </header>
    )
}