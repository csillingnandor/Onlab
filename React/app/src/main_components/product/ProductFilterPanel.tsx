import { Col, Form } from 'react-bootstrap';
import type { Product } from '../../util/Types.js';
import { countActive } from '../../util/filterUtils.js';
import type { ProductFilter } from '../../util/productFilter.js';
import FilterPanel from '../filter/FilterPanel.js';
import RangeFilterField from '../filter/RangeFilterField.js';
import { statusLabels } from './ProductStatusBadge.js';

type ProductFilterPanelProps = {
  id: string;
  value: ProductFilter;
  categories: string[]; // a betöltött termékekben előforduló kategóriák
  onChange: (filter: ProductFilter) => void;
  onClear: () => void;
};

const statuses = Object.keys(statusLabels) as Product['status'][];

export default function ProductFilterPanel({ id, value, categories, onChange, onClear }: ProductFilterPanelProps) {
  const update = (patch: Partial<ProductFilter>) => onChange({ ...value, ...patch });

  return (
    <FilterPanel id={id} label="Termékek szűrése" activeCount={countActive(value)} onClear={onClear}>
      <Col xs={12} md={6} lg={4}>
        <Form.Group controlId={`${id}-search`}>
          <Form.Label>Keresés</Form.Label>
          <Form.Control
            type="search"
            size="sm"
            placeholder="Név vagy SKU…"
            value={value.search}
            onChange={(e) => update({ search: e.target.value })}
          />
        </Form.Group>
      </Col>

      <Col xs={6} md={3} lg={2}>
        <Form.Group controlId={`${id}-category`}>
          <Form.Label>Kategória</Form.Label>
          <Form.Select size="sm" value={value.category} onChange={(e) => update({ category: e.target.value })}>
            <option value="">Összes</option>
            {categories.map((category) => (
              <option key={category} value={category}>{category}</option>
            ))}
          </Form.Select>
        </Form.Group>
      </Col>

      <Col xs={6} md={3} lg={2}>
        <Form.Group controlId={`${id}-status`}>
          <Form.Label>Állapot</Form.Label>
          <Form.Select
            size="sm"
            value={value.status}
            onChange={(e) => update({ status: e.target.value as ProductFilter['status'] })}
          >
            <option value="">Összes</option>
            {statuses.map((status) => (
              <option key={status} value={status}>{statusLabels[status]}</option>
            ))}
          </Form.Select>
        </Form.Group>
      </Col>

      <Col xs={6} md={4} lg={2}>
        <RangeFilterField
          id={`${id}-stock`}
          label="Készlet (db)"
          min={value.stockMin}
          max={value.stockMax}
          onChange={(stockMin, stockMax) => update({ stockMin, stockMax })}
        />
      </Col>

      <Col xs={6} md={4} lg={2}>
        <RangeFilterField
          id={`${id}-price`}
          label="Ár (Ft)"
          step="any"
          min={value.priceMin}
          max={value.priceMax}
          onChange={(priceMin, priceMax) => update({ priceMin, priceMax })}
        />
      </Col>
    </FilterPanel>
  );
}
