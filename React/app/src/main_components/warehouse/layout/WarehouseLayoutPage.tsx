import { useEffect, useMemo, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Form, Spinner } from 'react-bootstrap';
import warehouseService from '../../../services/warehouseService.js';
import type { WarehouseLayout } from '../../../util/Types.js';
import ViewModeToggle, { type ViewModeOption } from '../../../common/button/ViewModeToggle.js';
import { useTheme } from '../../../theme/useTheme.js';
import WarehouseScene, { type LayoutViewMode } from './WarehouseScene.js';
import LocationPanel from './LocationPanel.js';
import { scenePalette } from './scenePalette.js';
import '../WarehousePage.css';
import './WarehouseLayoutPage.css';

const viewModeOptions: ViewModeOption<LayoutViewMode>[] = [
  { value: '3d', label: '3D nézet', icon: 'bi-box' },
  { value: 'top', label: 'Felülnézet', icon: 'bi-grid-3x3' },
];

// Egy raktár belső elrendezése 3D-ben (csak megtekintés): polcok, tárolóhelyek telítettsége,
// a kiválasztott hely tartalma és a depón lévő, még el nem helyezett készlet.
export default function WarehouseLayoutPage() {
  const { id } = useParams();
  const warehouseId = Number(id);
  const { theme } = useTheme();
  const [layout, setLayout] = useState<WarehouseLayout | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [mode, setMode] = useState<LayoutViewMode>('3d');
  const [selectedLocationId, setSelectedLocationId] = useState<number | null>(null);
  const [highlightProductId, setHighlightProductId] = useState<number | null>(null);

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    warehouseService.getLayout(warehouseId)
      .then((data) => {
        if (!ignore) setLayout(data);
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.');
      });

    return () => {
      ignore = true;
    };
  }, [warehouseId]);

  const locations = useMemo(() => layout?.racks.flatMap((rack) => rack.locations) ?? [], [layout]);
  const selectedLocation = locations.find((l) => l.id === selectedLocationId) ?? null;

  // A kiemelhető termékek: amelyek legalább egy helyen megtalálhatók
  const products = useMemo(() => {
    const byId = new Map<number, string>();
    for (const location of locations)
      for (const stock of location.stocks) byId.set(stock.productId, stock.productName);
    return [...byId].map(([productId, name]) => ({ productId, name })).sort((a, b) => a.name.localeCompare(b.name, 'hu'));
  }, [locations]);

  const occupiedCount = locations.filter((l) => l.quantity > 0).length;

  const renderContent = () => {
    if (error) return <div className="alert alert-danger" role="alert">{error}</div>;

    if (!layout) {
      return (
        <div className="text-center py-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Elrendezés betöltése...</span>
          </Spinner>
        </div>
      );
    }

    if (!layout.hasLayout) {
      return <div className="alert alert-info" role="status">Ennek a raktárnak még nincs megadva belső elrendezése.</div>;
    }

    return (
      <div className="layout3d-body">
        <div className="layout3d-canvas">
          <WarehouseScene
            layout={layout}
            mode={mode}
            palette={scenePalette(theme)}
            selectedLocationId={selectedLocationId}
            highlightProductId={highlightProductId}
            onSelect={setSelectedLocationId}
          />
        </div>
        <LocationPanel
          location={selectedLocation}
          unplacedStock={layout.unplacedStock}
          palette={scenePalette(theme)}
          highlightActive={highlightProductId !== null}
        />
      </div>
    );
  };

  return (
    <div className="warehouse-page">
      <header className="warehouse-page-header">
        <div>
          <Link to="/warehouse" className="layout3d-back">
            <i className="bi bi-arrow-left" aria-hidden="true"></i> Raktárak
          </Link>
          <h1>{layout?.warehouseName ?? 'Raktár elrendezése'}</h1>
          {layout?.hasLayout && (
            <p className="warehouse-page-subtitle">
              {layout.width} × {layout.depth} m · {layout.racks.length} polc · {occupiedCount} / {locations.length} foglalt hely
            </p>
          )}
        </div>

        {layout?.hasLayout && (
          <div className="warehouse-header-actions">
            <Form.Select
              size="sm"
              className="layout3d-product-select"
              aria-label="Termék helyeinek kiemelése"
              value={highlightProductId ?? ''}
              onChange={(e) => setHighlightProductId(e.target.value === '' ? null : Number(e.target.value))}
            >
              <option value="">Minden termék</option>
              {products.map((p) => (
                <option key={p.productId} value={p.productId}>{p.name}</option>
              ))}
            </Form.Select>
            <ViewModeToggle options={viewModeOptions} value={mode} onChange={setMode} />
          </div>
        )}
      </header>

      {renderContent()}
    </div>
  );
}
