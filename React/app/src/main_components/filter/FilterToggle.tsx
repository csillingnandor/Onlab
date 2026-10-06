import './Filter.css';

type FilterToggleProps = {
  open: boolean;
  activeCount: number;
  controls: string; // a szűrőpanel id-ja
  onToggle: () => void;
};

// Ikongomb az oldal fejlécében; jelzi, hány szűrő aktív (a panel becsukva is).
export default function FilterToggle({ open, activeCount, controls, onToggle }: FilterToggleProps) {
  const label = activeCount > 0 ? `Szűrők (${activeCount} aktív)` : 'Szűrők';

  return (
    <button
      type="button"
      className={`filter-toggle ${open ? 'active' : ''}`}
      onClick={onToggle}
      aria-expanded={open}
      aria-controls={controls}
      aria-label={label}
      title={label}
    >
      <i className={`bi ${activeCount > 0 ? 'bi-funnel-fill' : 'bi-funnel'}`} aria-hidden="true"></i>
      {activeCount > 0 && <span className="filter-count">{activeCount}</span>}
    </button>
  );
}
