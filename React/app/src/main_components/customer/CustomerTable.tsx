import ActionButtons from '../../common/button/ActionButtons.js';
import type { Customer } from '../../util/Types.js';
import { dateFormatter, priceFormatter } from '../../util/format.js';
import '../../common/table/DataTable.css';

type CustomerTableProps = {
  customers: Customer[];
  // Nincs megadva: nincs hozzá jogosultság, a gomb nem jelenik meg (ha egyik sincs, az oszlop sem)
  onEdit?: ((customer: Customer) => void) | undefined;
  onDelete?: ((customer: Customer) => void) | undefined;
};

const missing = <span className="text-muted">–</span>;

export default function CustomerTable({ customers, onEdit, onDelete }: CustomerTableProps) {
  const showActions = onEdit !== undefined || onDelete !== undefined;

  return (
    <div className="data-table-wrapper">
      <table className="data-table">
        <thead>
          <tr>
            <th>Név</th>
            <th>E-mail</th>
            <th>Telefon</th>
            <th className="numeric">Rendelések</th>
            <th className="numeric" title="Csak a kiszállított rendelések">Vásárlás összesen</th>
            <th>Utolsó rendelés</th>
            {showActions && <th className="numeric">Műveletek</th>}
          </tr>
        </thead>
        <tbody>
          {customers.map((customer) => (
            <tr key={customer.id}>
              <td className="data-table-strong">{customer.name}</td>
              <td>
                <a href={`mailto:${customer.email}`} className="customer-email">{customer.email}</a>
              </td>
              <td className="data-table-mono">{customer.phone ?? missing}</td>
              <td className="numeric">{customer.orderCount} db</td>
              <td className="numeric">{priceFormatter.format(customer.totalSpent)}</td>
              <td>
                {customer.lastOrderDate ? dateFormatter.format(new Date(customer.lastOrderDate)) : missing}
              </td>
              {showActions && (
                <td className="numeric">
                  <ActionButtons
                    itemLabel={customer.name}
                    onEdit={onEdit && (() => onEdit(customer))}
                    onDelete={onDelete && (() => onDelete(customer))}
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
