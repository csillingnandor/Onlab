import { useEffect, useMemo, useState } from 'react';
import { Link, useParams, useSearchParams } from 'react-router-dom';
import { Form, Spinner } from 'react-bootstrap';
import warehouseService from '../../../services/warehouseService.js';
import orderService from '../../../services/orderService.js';
import type { CustomerOrder, PickRoute, WarehouseLayout } from '../../../util/Types.js';
import ViewModeToggle, { type ViewModeOption } from '../../../common/button/ViewModeToggle.js';
import { useTheme } from '../../../theme/useTheme.js';
import WarehouseScene, { type LayoutViewMode } from './WarehouseScene.js';
import LocationPanel from './LocationPanel.js';
import RoutePanel from './RoutePanel.js';
import { scenePalette } from './scenePalette.js';
import '../WarehousePage.css';
import './WarehouseLayoutPage.css';

const viewModeOptions: ViewModeOption<LayoutViewMode>[] = [
  { value: '3d', label: '3D nézet', icon: 'bi-box' },
  { value: 'top', label: 'Felülnézet', icon: 'bi-grid-3x3' },
];

// Egy raktár belső elrendezése 3D-ben: polcok, tárolóhelyek telítettsége, a kiválasztott hely tartalma,
// a depón lévő, még el nem helyezett készlet, és egy függő rendelés felvételi útvonala (?order=<id>).
export default function WarehouseLayoutPage() {
  const { id } = useParams();
  const warehouseId = Number(id);
  const [searchParams, setSearchParams] = useSearchParams();
  const orderParam = Number(searchParams.get('order'));
  const selectedOrderId = Number.isInteger(orderParam) && orderParam > 0 ? orderParam : null;
  const { theme } = useTheme();
  const [layout, setLayout] = useState<WarehouseLayout | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [mode, setMode] = useState<LayoutViewMode>('3d');
  const [selectedLocationId, setSelectedLocationId] = useState<number | null>(null);
  const [highlightProductId, setHighlightProductId] = useState<number | null>(null);
  const [orders, setOrders] = useState<CustomerOrder[] | null>(null);
  const [ordersError, setOrdersError] = useState<string | null>(null);
  const [route, setRoute] = useState<PickRoute | null>(null);
  const [routeError, setRouteError] = useState<string | null>(null);
  const [playing, setPlaying] = useState<boolean>(true);

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

  // A raktárból kiszállításra váró (függő) rendelések az útvonaltervezéshez
  useEffect(() => {
    let ignore = false;

    orderService.getAll()
      .then((data) => {
        if (!ignore) setOrders(data.filter((o) => o.warehouseId === warehouseId && o.status === 'Pending'));
      })
      .catch(() => {
        if (!ignore) setOrdersError('A rendelések nem érhetők el (nincs hozzá jogosultság, vagy hiba történt).');
      });

    return () => {
      ignore = true;
    };
  }, [warehouseId]);

  // A kiválasztott rendelés körútja; rendelésváltáskor az előző eltűnik, amíg az új betöltődik
  useEffect(() => {
    if (selectedOrderId === null) return;
    let ignore = false;

    orderService.getPickRoute(selectedOrderId)
      .then((data) => {
        if (!ignore) setRoute(data);
      })
      .catch((err: unknown) => {
        if (!ignore) setRouteError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.');
      });

    return () => {
      ignore = true;
    };
  }, [selectedOrderId]);

  const selectOrder = (orderId: number | null) => {
    setRoute(null);
    setRouteError(null);
    setPlaying(true);
    setHighlightProductId(null);
    setSearchParams(orderId === null ? {} : { order: String(orderId) }, { replace: true });
  };

  const locations = useMemo(() => layout?.racks.flatMap((rack) => rack.locations) ?? [], [layout]);

  // Kiemelés: a felvételi útvonal helyei, különben a kiválasztott termék helyei
  const highlightLocationIds = useMemo(() => {
    if (route) {
      return new Set(route.stops.flatMap((s) => s.picks.map((p) => p.locationId)).filter((x): x is number => x !== null));
    }
    if (highlightProductId === null) return null;
    return new Set(locations.filter((l) => l.stocks.some((s) => s.productId === highlightProductId)).map((l) => l.id));
  }, [route, highlightProductId, locations]);
  const highlightLabel = route ? 'A felvétel helyei' : highlightProductId !== null ? 'A kiválasztott termék helyei' : null;
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
            highlightLocationIds={highlightLocationIds}
            route={route}
            routePlaying={playing}
            onSelect={setSelectedLocationId}
          />
        </div>
        <LocationPanel
          location={selectedLocation}
          unplacedStock={layout.unplacedStock}
          palette={scenePalette(theme)}
          highlightLabel={highlightLabel}
          showRouteLegend={route !== null}
        >
          <RoutePanel
            orders={orders}
            ordersError={ordersError}
            selectedOrderId={selectedOrderId}
            onSelectOrder={selectOrder}
            route={route}
            routeLoading={selectedOrderId !== null && route === null && routeError === null}
            routeError={routeError}
            playing={playing}
            onTogglePlaying={() => setPlaying((prev) => !prev)}
            onSelectLocation={setSelectedLocationId}
          />
        </LocationPanel>
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
