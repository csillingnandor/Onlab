import { useState } from 'react';
import { Button, Dropdown, Form } from 'react-bootstrap';
import {
  formatIsoDate,
  presetLabels,
  presetRange,
  type DateRange,
  type DateRangePreset,
} from '../../util/dateRange.js';

type DateRangeMenuProps = {
  value: DateRange;
  onChange: (range: DateRange) => void;
};

const presets: Exclude<DateRangePreset, 'custom'>[] = ['day', 'week', 'month'];

// Menü stílusú időszakválasztó. Általános: a dashboard bármelyik paneljén használható.
export default function DateRangeMenu({ value, onChange }: DateRangeMenuProps) {
  const [open, setOpen] = useState(false);
  const [customOpen, setCustomOpen] = useState(false);
  const [customFrom, setCustomFrom] = useState(value.from);
  const [customTo, setCustomTo] = useState(value.to);

  const customInvalid = customFrom === '' || customTo === '' || customFrom > customTo;

  const handleToggle = (nextOpen: boolean) => {
    setOpen(nextOpen);
    if (nextOpen) {
      // Megnyitáskor az egyedi mezők az aktuális időszakról induljanak
      setCustomFrom(value.from);
      setCustomTo(value.to);
      setCustomOpen(value.preset === 'custom');
    }
  };

  const selectPreset = (preset: Exclude<DateRangePreset, 'custom'>) => {
    onChange(presetRange(preset));
    setOpen(false);
  };

  const applyCustom = (event: React.FormEvent) => {
    event.preventDefault();
    if (customInvalid) return;
    onChange({ preset: 'custom', from: customFrom, to: customTo });
    setOpen(false);
  };

  const label = value.preset === 'custom'
    ? `${formatIsoDate(value.from)} – ${formatIsoDate(value.to)}`
    : presetLabels[value.preset];

  return (
    // autoClose="outside": a menün belüli kattintás (pl. a dátummezők) nem zárja be; mi zárjuk választáskor
    <Dropdown show={open} onToggle={handleToggle} autoClose="outside" align="end">
      <Dropdown.Toggle variant="outline-light" size="sm">
        <i className="bi bi-calendar3 me-2" aria-hidden="true"></i>
        {label}
      </Dropdown.Toggle>

      <Dropdown.Menu className="date-range-menu">
        {presets.map((preset) => (
          <Dropdown.Item
            key={preset}
            as="button"
            active={value.preset === preset}
            onClick={() => selectPreset(preset)}
          >
            {presetLabels[preset]}
          </Dropdown.Item>
        ))}

        <Dropdown.Divider />

        <Dropdown.Item
          as="button"
          active={value.preset === 'custom'}
          onClick={() => setCustomOpen((prev) => !prev)}
          aria-expanded={customOpen}
        >
          {presetLabels.custom}…
        </Dropdown.Item>

        {customOpen && (
          <Form className="date-range-menu-custom" onSubmit={applyCustom}>
            <Form.Group className="mb-2" controlId="date-range-from">
              <Form.Label className="small mb-1">Kezdete</Form.Label>
              <Form.Control
                type="date"
                size="sm"
                value={customFrom}
                max={customTo || undefined}
                onChange={(e) => setCustomFrom(e.target.value)}
              />
            </Form.Group>
            <Form.Group className="mb-2" controlId="date-range-to">
              <Form.Label className="small mb-1">Vége</Form.Label>
              <Form.Control
                type="date"
                size="sm"
                value={customTo}
                min={customFrom || undefined}
                onChange={(e) => setCustomTo(e.target.value)}
                isInvalid={customFrom !== '' && customTo !== '' && customFrom > customTo}
              />
              <Form.Control.Feedback type="invalid">
                A vége nem lehet korábbi a kezdeténél.
              </Form.Control.Feedback>
            </Form.Group>
            <Button type="submit" size="sm" className="w-100" disabled={customInvalid}>
              Alkalmaz
            </Button>
          </Form>
        )}
      </Dropdown.Menu>
    </Dropdown>
  );
}
