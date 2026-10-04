import type { Warehouse } from '../../util/Types.js';
import '../../DataTable.css';

type WarehouseTableProps = {
  warehouses: Warehouse[];
};

function formatCoordinates(warehouse: Warehouse) {
  if (warehouse.latitude === null || warehouse.longitude === null) {
    // Ezek a raktárak a térképen nem jelennek meg, ezért itt jelezzük.
    return <span className="text-muted">nincs megadva</span>;
  }
  return `${warehouse.latitude.toFixed(4)}, ${warehouse.longitude.toFixed(4)}`;
}

export default function WarehouseTable({ warehouses }: WarehouseTableProps) {
  return (
    <div className="data-table-wrapper">
      <table className="data-table">
        <thead>
          <tr>
            <th>Név</th>
            <th>Cím</th>
            <th className="numeric">Kapacitás</th>
            <th>Koordináták</th>
          </tr>
        </thead>
        <tbody>
          {warehouses.map((warehouse) => (
            <tr key={warehouse.id}>
              <td className="data-table-strong">{warehouse.name}</td>
              <td>{warehouse.address || <span className="text-muted">–</span>}</td>
              <td className="numeric">{warehouse.capacity.toLocaleString('hu-HU')} hely</td>
              <td className="data-table-mono">{formatCoordinates(warehouse)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
