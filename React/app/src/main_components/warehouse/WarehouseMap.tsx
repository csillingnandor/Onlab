import { useEffect, useMemo } from 'react';
import L, { type LatLngTuple } from 'leaflet';
import { MapContainer, Marker, Popup, TileLayer, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import type { Warehouse } from '../../util/Types.js';

// Saját ikon az alapértelmezett helyett: Vite alatt az L.Icon.Default a CSS-ből kitalált útvonalat
// elé fűzi az importált URL-nek, és törött képet ad. A méretek a Leaflet alapértelmezett markeréé.
const warehouseIcon = L.icon({
  iconUrl: markerIcon,
  iconRetinaUrl: markerIcon2x,
  shadowUrl: markerShadow,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});

// Kiinduló nézet, ha egyetlen raktárnak sincs koordinátája: Magyarország közepe
const HUNGARY_CENTER: LatLngTuple = [47.16, 19.5];
const HUNGARY_ZOOM = 7;

type LocatedWarehouse = Warehouse & { latitude: number; longitude: number };

function hasLocation(warehouse: Warehouse): warehouse is LocatedWarehouse {
  return warehouse.latitude !== null && warehouse.longitude !== null;
}

// A térkép a markerekhez igazodik; egyetlen marker esetén sem zoomol túl közel.
function FitToMarkers({ positions }: { positions: LatLngTuple[] }) {
  const map = useMap();

  useEffect(() => {
    if (positions.length === 0) return;
    map.fitBounds(L.latLngBounds(positions), { padding: [48, 48], maxZoom: 12 });
  }, [map, positions]);

  return null;
}

type WarehouseMapProps = {
  warehouses: Warehouse[];
};

export default function WarehouseMap({ warehouses }: WarehouseMapProps) {
  const located = useMemo(() => warehouses.filter(hasLocation), [warehouses]);
  const missingCount = warehouses.length - located.length;
  // Leaflet [lat, lng] sorrendet vár. Memoizálva, hogy újrarendereléskor ne igazítsa vissza a térképet.
  const positions = useMemo(
    () => located.map((w): LatLngTuple => [w.latitude, w.longitude]),
    [located],
  );

  return (
    <div className="warehouse-map-wrapper">
      <MapContainer center={HUNGARY_CENTER} zoom={HUNGARY_ZOOM} className="warehouse-map" scrollWheelZoom>
        {/* OpenStreetMap csempék (API-kulcs nélkül); a sötét megjelenést CSS-szűrő adja. Az OSM feltüntetése kötelező. */}
        <TileLayer
          url="https://tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> közreműködők'
          maxZoom={19}
        />

        {located.map((warehouse) => (
          <Marker key={warehouse.id} position={[warehouse.latitude, warehouse.longitude]} icon={warehouseIcon}>
            <Popup>
              <strong>{warehouse.name}</strong>
              <br />
              {warehouse.address}
              <br />
              Kapacitás: {warehouse.capacity.toLocaleString('hu-HU')} hely
            </Popup>
          </Marker>
        ))}

        <FitToMarkers positions={positions} />
      </MapContainer>

      {missingCount > 0 && (
        <p className="warehouse-map-note">
          <i className="bi bi-info-circle me-1" aria-hidden="true"></i>
          {missingCount} raktárnak nincs megadva helye, ezért nem látszik a térképen (lásd a táblázatos nézetet).
        </p>
      )}
    </div>
  );
}
