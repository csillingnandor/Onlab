import { Col, Form } from 'react-bootstrap';
import { isLastOrderRangeInvalid, type CustomerFilter } from '../../util/customerFilter.js';
import { countActive } from '../../util/filterUtils.js';
import FilterPanel from '../filter/FilterPanel.js';
import RangeFilterField from '../filter/RangeFilterField.js';

type CustomerFilterPanelProps = {
  id: string;
  value: CustomerFilter;
  onChange: (filter: CustomerFilter) => void;
  onClear: () => void;
};

export default function CustomerFilterPanel({ id, value, onChange, onClear }: CustomerFilterPanelProps) {
  const update = (patch: Partial<CustomerFilter>) => onChange({ ...value, ...patch });
  const dateRangeInvalid = isLastOrderRangeInvalid(value);

  return (
    <FilterPanel
      id={id}
      label="Vevők szűrése"
      activeCount={countActive(value)}
      onClear={onClear}
      footer={
        <>
          {dateRangeInvalid && (
            <p className="filter-panel-error" role="alert">
              Az utolsó rendelés „ig” dátuma nem lehet korábbi a „tól” dátumnál.
            </p>
          )}
          <p className="filter-panel-hint">Dátum megadásakor a még nem rendelt vevők nem jelennek meg.</p>
        </>
      }
    >
      <Col xs={12} md={6} lg={3}>
        <Form.Group controlId={`${id}-search`}>
          <Form.Label>Keresés</Form.Label>
          <Form.Control
            type="search"
            size="sm"
            placeholder="Név, e-mail vagy telefon…"
            value={value.search}
            onChange={(e) => update({ search: e.target.value })}
          />
        </Form.Group>
      </Col>

      <Col xs={6} md={3} lg={2}>
        <RangeFilterField
          id={`${id}-orders`}
          label="Rendelések (db)"
          min={value.ordersMin}
          max={value.ordersMax}
          onChange={(ordersMin, ordersMax) => update({ ordersMin, ordersMax })}
        />
      </Col>

      <Col xs={6} md={3} lg={3}>
        <RangeFilterField
          id={`${id}-spent`}
          label="Vásárlás összesen (Ft)"
          step="any"
          min={value.spentMin}
          max={value.spentMax}
          onChange={(spentMin, spentMax) => update({ spentMin, spentMax })}
        />
      </Col>

      <Col xs={12} md={6} lg={4}>
        <Form.Group>
          <Form.Label htmlFor={`${id}-last-from`}>Utolsó rendelés</Form.Label>
          <div className="filter-range">
            <Form.Control
              id={`${id}-last-from`}
              type="date"
              size="sm"
              aria-label="Utolsó rendelés – ettől"
              value={value.lastOrderFrom}
              max={value.lastOrderTo || undefined}
              onChange={(e) => update({ lastOrderFrom: e.target.value })}
            />
            <span className="filter-range-separator" aria-hidden="true">–</span>
            <Form.Control
              id={`${id}-last-to`}
              type="date"
              size="sm"
              aria-label="Utolsó rendelés – eddig"
              value={value.lastOrderTo}
              min={value.lastOrderFrom || undefined}
              onChange={(e) => update({ lastOrderTo: e.target.value })}
              isInvalid={dateRangeInvalid}
            />
          </div>
        </Form.Group>
      </Col>
    </FilterPanel>
  );
}
