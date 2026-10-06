import { Col, Form } from 'react-bootstrap';
import { countActive } from '../../util/filterUtils.js';
import type { WarehouseFilter } from '../../util/warehouseFilter.js';
import FilterPanel from '../filter/FilterPanel.js';
import RangeFilterField from '../filter/RangeFilterField.js';

type WarehouseFilterPanelProps = {
  id: string;
  value: WarehouseFilter;
  onChange: (filter: WarehouseFilter) => void;
  onClear: () => void;
};

export default function WarehouseFilterPanel({ id, value, onChange, onClear }: WarehouseFilterPanelProps) {
  const update = (patch: Partial<WarehouseFilter>) => onChange({ ...value, ...patch });

  return (
    <FilterPanel id={id} label="Raktárak szűrése" activeCount={countActive(value)} onClear={onClear}>
      <Col xs={12} md={6}>
        <Form.Group controlId={`${id}-search`}>
          <Form.Label>Keresés</Form.Label>
          <Form.Control
            type="search"
            size="sm"
            placeholder="Név vagy cím…"
            value={value.search}
            onChange={(e) => update({ search: e.target.value })}
          />
        </Form.Group>
      </Col>

      <Col xs={6} md={3}>
        <RangeFilterField
          id={`${id}-capacity`}
          label="Kapacitás (hely)"
          min={value.capacityMin}
          max={value.capacityMax}
          onChange={(capacityMin, capacityMax) => update({ capacityMin, capacityMax })}
        />
      </Col>

      <Col xs={6} md={3}>
        <Form.Group controlId={`${id}-location`}>
          <Form.Label>Koordináták</Form.Label>
          <Form.Select
            size="sm"
            value={value.location}
            onChange={(e) => update({ location: e.target.value as WarehouseFilter['location'] })}
          >
            <option value="">Összes</option>
            <option value="with">Megadva</option>
            <option value="without">Nincs megadva</option>
          </Form.Select>
        </Form.Group>
      </Col>
    </FilterPanel>
  );
}
