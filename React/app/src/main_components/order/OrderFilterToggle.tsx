type OrderFilterToggleProps = {
  open: boolean;
  activeCount: number;
  controls: string; // a szűrőpanel id-ja
  onToggle: () => void;
};

// Ikongomb a fejlécben az „Összes kinyitása” mellett; jelzi, hány szűrő aktív (a panel becsukva is).
export default function OrderFilterToggle({ open, activeCount, controls, onToggle }: OrderFilterToggleProps) {
  const label = activeCount > 0 ? `Szűrők (${activeCount} aktív)` : 'Szűrők';

  return (
    <button
      type="button"
      className={`order-expand-all order-filter-toggle ${open ? 'active' : ''}`}
      onClick={onToggle}
      aria-expanded={open}
      aria-controls={controls}
      aria-label={label}
      title={label}
    >
      <i className={`bi ${activeCount > 0 ? 'bi-funnel-fill' : 'bi-funnel'}`} aria-hidden="true"></i>
      {activeCount > 0 && <span className="order-filter-count">{activeCount}</span>}
    </button>
  );
}
