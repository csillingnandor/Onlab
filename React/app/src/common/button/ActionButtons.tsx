import './ActionButtons.css';

// További ikonos művelet a módosítás / törlés előtt (pl. készlet)
export type ExtraAction = {
  icon: string; // Bootstrap Icons osztály, pl. 'bi-boxes'
  label: string; // tooltip, pl. 'Készlet'
  ariaLabel: string; // pl. 'Logitech MX Master 3S egér készlete'
  onClick: () => void;
};

type ActionButtonsProps = {
  // Az elem neve a képernyőolvasós feliratokhoz, pl. 'Logitech MX Master 3S egér' -> '... módosítása'
  itemLabel: string;
  onEdit: () => void;
  onDelete: () => void;
  extraActions?: ExtraAction[];
  className?: string;
};

// Csak ikonos módosítás / törlés gombok (és opcionális további műveletek) kártyára vagy táblázatsor végére.
export default function ActionButtons({ itemLabel, onEdit, onDelete, extraActions = [], className }: ActionButtonsProps) {
  return (
    <div className={`action-buttons ${className ?? ''}`}>
      {extraActions.map((action) => (
        <button
          key={action.icon}
          type="button"
          className="action-btn"
          onClick={action.onClick}
          aria-label={action.ariaLabel}
          title={action.label}
        >
          <i className={`bi ${action.icon}`} aria-hidden="true"></i>
        </button>
      ))}
      <button
        type="button"
        className="action-btn"
        onClick={onEdit}
        aria-label={`${itemLabel} módosítása`}
        title="Módosítás"
      >
        <i className="bi bi-pencil" aria-hidden="true"></i>
      </button>
      <button
        type="button"
        className="action-btn action-btn-danger"
        onClick={onDelete}
        aria-label={`${itemLabel} törlése`}
        title="Törlés"
      >
        <i className="bi bi-trash" aria-hidden="true"></i>
      </button>
    </div>
  );
}
