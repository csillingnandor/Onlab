import './ActionButtons.css';
import { useI18n } from '../../i18n/useI18n.js';

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
  onEdit?: (() => void) | undefined; // ha nincs megadva (pl. nincs hozzá jogosultság), a gomb nem jelenik meg
  onDelete?: (() => void) | undefined;
  extraActions?: ExtraAction[];
  className?: string;
};

// Csak ikonos módosítás / törlés gombok (és opcionális további műveletek) kártyára vagy táblázatsor végére.
export default function ActionButtons({ itemLabel, onEdit, onDelete, extraActions = [], className }: ActionButtonsProps) {
  const { t } = useI18n();

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
      {onEdit && (
      <button
        type="button"
        className="action-btn"
        onClick={onEdit}
        aria-label={t('common.editItem', { item: itemLabel })}
        title={t('common.edit')}
      >
        <i className="bi bi-pencil" aria-hidden="true"></i>
      </button>
      )}
      {onDelete && (
      <button
        type="button"
        className="action-btn action-btn-danger"
        onClick={onDelete}
        aria-label={t('common.deleteItem', { item: itemLabel })}
        title={t('common.delete')}
      >
        <i className="bi bi-trash" aria-hidden="true"></i>
      </button>
      )}
    </div>
  );
}
