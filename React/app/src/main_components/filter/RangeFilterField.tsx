import { Form } from 'react-bootstrap';
import { isRangeInvalid } from '../../util/filterUtils.js';

type RangeFilterFieldProps = {
  id: string;
  label: string; // pl. 'Ár (Ft)'
  min: string;
  max: string;
  onChange: (min: string, max: string) => void;
  step?: string; // '1' egész számokhoz, 'any' árakhoz
};

// Számintervallum (tól–ig) két mezővel; bármelyik határ üresen hagyható.
export default function RangeFilterField({ id, label, min, max, onChange, step = '1' }: RangeFilterFieldProps) {
  const invalid = isRangeInvalid(min, max);

  return (
    <Form.Group>
      <Form.Label htmlFor={`${id}-min`}>{label}</Form.Label>
      <div className="filter-range">
        <Form.Control
          id={`${id}-min`}
          type="number"
          size="sm"
          min={0}
          step={step}
          placeholder="tól"
          aria-label={`${label} – legalább`}
          value={min}
          onChange={(e) => onChange(e.target.value, max)}
        />
        <span className="filter-range-separator" aria-hidden="true">–</span>
        <Form.Control
          id={`${id}-max`}
          type="number"
          size="sm"
          min={0}
          step={step}
          placeholder="ig"
          aria-label={`${label} – legfeljebb`}
          value={max}
          onChange={(e) => onChange(min, e.target.value)}
          isInvalid={invalid}
        />
      </div>
    </Form.Group>
  );
}
