import './ViewModeToggle.css';

export type ViewModeOption<T extends string> = {
  value: T;
  label: string; // pl. 'Lista nézet' (tooltip és képernyőolvasó)
  icon: string; // Bootstrap Icons osztály, pl. 'bi-list'
};

type ViewModeToggleProps<T extends string> = {
  options: ViewModeOption<T>[];
  value: T;
  onChange: (value: T) => void;
};

// Szegmentált ikongomb-csoport nézetváltáshoz (pl. lista / rács, táblázat / térkép).
export default function ViewModeToggle<T extends string>({ options, value, onChange }: ViewModeToggleProps<T>) {
  return (
    <div className="view-mode-toggle">
      {options.map((option) => (
        <button
          key={option.value}
          type="button"
          className={value === option.value ? 'active' : ''}
          onClick={() => onChange(option.value)}
          aria-pressed={value === option.value}
          aria-label={option.label}
          title={option.label}
        >
          <i className={`bi ${option.icon}`} aria-hidden="true"></i>
        </button>
      ))}
    </div>
  );
}
