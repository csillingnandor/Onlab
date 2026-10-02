import type { ViewMode } from '../../util/Types.js';

type ProductHeaderProps = {
  viewMode: ViewMode;
  onViewModeChange: (mode: ViewMode) => void;
  onAddProductClick?: () => void;
};

export default function ProductHeader({ viewMode, onViewModeChange, onAddProductClick }: ProductHeaderProps) {

    return (
        <header className="product-header">
            <h1>Termékek</h1>
            <div className="view-mode-toggle">
                <button
                    type="button"
                    className={viewMode === 'list' ? 'active' : ''}
                    onClick={() => onViewModeChange('list')}
                    aria-pressed={viewMode === 'list'}
                    aria-label="Lista nézet"
                    title="Lista nézet"
                >
                    <i className="bi bi-list"></i>
                </button>
                <button
                    type="button"
                    className={viewMode === 'grid' ? 'active' : ''}
                    onClick={() => onViewModeChange('grid')}
                    aria-pressed={viewMode === 'grid'}
                    aria-label="Rács nézet"
                    title="Rács nézet"
                >
                    <i className="bi bi-grid"></i>
                </button>
            </div>
            <button className="add-product-btn" onClick={onAddProductClick}>
                Új termék hozzáadása
            </button>
        </header>
    )
}