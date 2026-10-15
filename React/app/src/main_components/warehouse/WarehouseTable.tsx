import ActionButtons from '../../common/button/ActionButtons.js';
import type { Warehouse } from '../../util/Types.js';
import '../../common/table/DataTable.css';

type WarehouseTableProps = {
  warehouses: Warehouse[];
  // Nincs megadva: nincs hozzá jogosultság, a gomb nem jelenik meg (ha egyik sincs, az oszlop sem)
  onEdit?: ((warehouse: Warehouse) => void) | undefined;
  onDelete?: ((warehouse: Warehouse) => void) | undefined;
};

function formatCoordinates(warehouse: Warehouse) {
  if (warehouse.latitude === null || warehouse.longitude === null) {
    // Ezek a raktárak a térképen nem jelennek meg, ezért itt jelezzük.
    return <span className="text-muted">nincs megadva</span>;
  }
  return `${warehouse.latitude.toFixed(4)}, ${warehouse.longitude.toFixed(4)}`;
}

export default function WarehouseTable({ warehouses, onEdit, onDelete }: WarehouseTableProps) {
  const showActions = onEdit !== undefined || onDelete !== undefined;

  return (
    <div className="data-table-wrapper">
      <table className="data-table">
        <thead>
          <tr>
            <th>Név</th>
            <th>Cím</th>
            <th className="numeric">Kapacitás</th>
            <th>Koordináták</th>
            {showActions && <th className="numeric">Műveletek</th>}
          </tr>
        </thead>
        <tbody>
          {warehouses.map((warehouse) => (
            <tr key={warehouse.id}>
              <td className="data-table-strong">{warehouse.name}</td>
              <td>{warehouse.address || <span className="text-muted">–</span>}</td>
              <td className="numeric">{warehouse.capacity.toLocaleString('hu-HU')} hely</td>
              <td className="data-table-mono">{formatCoordinates(warehouse)}</td>
              {showActions && (
                <td className="numeric">
                  <ActionButtons
                    itemLabel={warehouse.name}
                    onEdit={onEdit && (() => onEdit(warehouse))}
                    onDelete={onDelete && (() => onDelete(warehouse))}
                  />
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
