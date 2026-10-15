import { Link } from 'react-router-dom';
import { useAuth } from '../../auth/useAuth.js';
import type { ActionKey } from '../../util/pages.js';

type QuickAction = {
  label: string;
  icon: string; // Bootstrap Icons osztály
  to: string; // a cél oldal; a ?new=1 / ?import=1 jelzőre az oldal rögtön megnyitja az űrlapot (util/useQueryFlag.ts)
  action: ActionKey; // csak ezzel a jogosultsággal jelenik meg
};

const quickActions: QuickAction[] = [
  { label: 'Új vevői rendelés', icon: 'bi-cart-plus', to: '/customer-orders?new=1', action: 'customer-orders.create' },
  { label: 'Új beszerzés', icon: 'bi-truck', to: '/supplier-orders?new=1', action: 'supplier-orders.create' },
  { label: 'Új termék', icon: 'bi-box-seam', to: '/products?new=1', action: 'products.create' },
  { label: 'Termékimport', icon: 'bi-file-earmark-arrow-up', to: '/products?import=1', action: 'products.import' },
  { label: 'Új vevő', icon: 'bi-person-plus', to: '/customers?new=1', action: 'customers.create' },
  { label: 'Új beszállító', icon: 'bi-building-add', to: '/suppliers?new=1', action: 'suppliers.create' },
];

// Gyorsgombok a műszerfal tetején a leggyakoribb műveletekhez; csak az engedélyezettek látszanak
export default function QuickActions() {
  const { hasAction } = useAuth();
  const visible = quickActions.filter((qa) => hasAction(qa.action));

  if (visible.length === 0) return null;

  return (
    <nav className="dashboard-quick-actions" aria-label="Gyorsműveletek">
      {visible.map((qa) => (
        <Link key={qa.to} to={qa.to} className="dashboard-quick-action">
          <i className={`bi ${qa.icon}`} aria-hidden="true"></i>
          <span>{qa.label}</span>
        </Link>
      ))}
    </nav>
  );
}
