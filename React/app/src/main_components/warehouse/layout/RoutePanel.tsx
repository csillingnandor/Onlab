import { Form, Spinner } from 'react-bootstrap';
import type { CustomerOrder, PickLine, PickRoute } from '../../../util/Types.js';

type RoutePanelProps = {
  orders: CustomerOrder[] | null; // a raktár függő rendelései; null: még töltődik
  ordersError: string | null;
  selectedOrderId: number | null;
  onSelectOrder: (orderId: number | null) => void;
  route: PickRoute | null;
  routeLoading: boolean;
  routeError: string | null;
  playing: boolean;
  onTogglePlaying: () => void;
  onSelectLocation: (locationId: number) => void;
};

function PickLines({ lines, onSelectLocation }: { lines: PickLine[]; onSelectLocation?: (locationId: number) => void }) {
  return (
    <ul className="layout3d-stock-list">
      {lines.map((line) => (
        <li key={`${line.locationId ?? 'dock'}-${line.productId}`}>
          <span>
            {line.productName}
            {line.locationId !== null && line.locationCode && (
              onSelectLocation
                ? (
                  <button type="button" className="layout3d-link" onClick={() => onSelectLocation(line.locationId!)}>
                    {line.locationCode}
                  </button>
                )
                : <small className="data-table-mono">{line.locationCode}</small>
            )}
          </span>
          <strong>{line.quantity} db</strong>
        </li>
      ))}
    </ul>
  );
}

// Felvételi útvonal: egy függő rendelés kiválasztása, a körút hossza, a megállók sorrendben (a helykódra
// kattintva a hely kijelölődik a 3D nézetben), a depóról felvehető tételek és az esetleges hiány.
export default function RoutePanel({
  orders,
  ordersError,
  selectedOrderId,
  onSelectOrder,
  route,
  routeLoading,
  routeError,
  playing,
  onTogglePlaying,
  onSelectLocation,
}: RoutePanelProps) {
  const renderRoute = () => {
    if (routeLoading) return <Spinner animation="border" size="sm" role="status" />;
    if (routeError) return <div className="alert alert-warning py-2 mb-0" role="alert">{routeError}</div>;
    if (!route) return null;

    return (
      <>
        <div className="layout3d-route-summary">
          <span>
            <strong>{route.distance} m</strong> · {route.stops.length} megálló
          </span>
          {route.stops.length > 0 && (
            <button type="button" className="header-btn" onClick={onTogglePlaying}>
              <i className={`bi ${playing ? 'bi-pause-fill' : 'bi-play-fill'}`} aria-hidden="true"></i>
              {playing ? 'Szünet' : 'Lejátszás'}
            </button>
          )}
        </div>

        {route.shortages.length > 0 && (
          <div className="alert alert-danger py-2" role="alert">
            Nincs elég készlet, a rendelés így nem szállítható ki:
            <PickLines lines={route.shortages} />
          </div>
        )}

        <ol className="layout3d-stops">
          {route.stops.map((stop) => (
            <li key={stop.sequence}>
              <span className="layout3d-stop-number" aria-hidden="true">{stop.sequence}</span>
              <PickLines lines={stop.picks} onSelectLocation={onSelectLocation} />
            </li>
          ))}
        </ol>

        {route.fromDock.length > 0 && (
          <>
            <p className="layout3d-muted small mb-1 mt-2">A depóról (nem kell érte bemenni):</p>
            <PickLines lines={route.fromDock} />
          </>
        )}
      </>
    );
  };

  return (
    <section>
      <h2>Felvételi útvonal</h2>
      {ordersError && <p className="layout3d-muted">{ordersError}</p>}
      {!ordersError && orders === null && <Spinner animation="border" size="sm" role="status" />}
      {orders !== null && orders.length === 0 && (
        <p className="layout3d-muted">Ebből a raktárból nincs kiszállításra váró rendelés.</p>
      )}
      {orders !== null && orders.length > 0 && (
        <>
          <Form.Select
            size="sm"
            className="mb-2"
            aria-label="Rendelés kiválasztása"
            value={selectedOrderId ?? ''}
            onChange={(e) => onSelectOrder(e.target.value === '' ? null : Number(e.target.value))}
          >
            <option value="">Válassz függő rendelést...</option>
            {orders.map((o) => (
              <option key={o.id} value={o.id}>#{o.id} – {o.customerName}</option>
            ))}
          </Form.Select>
          {renderRoute()}
        </>
      )}
    </section>
  );
}
