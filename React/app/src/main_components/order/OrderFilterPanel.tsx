import { Button, Col, Form, Row } from 'react-bootstrap';
import type { OrderStatus } from '../../util/Types.js';
import { countActiveFilters, isDateRangeInvalid, type OrderFilter } from '../../util/orderFilter.js';
import { statusLabels } from './OrderStatusBadge.js';

type OrderFilterPanelProps = {
  id: string;
  value: OrderFilter;
  onChange: (filter: OrderFilter) => void;
  onClear: () => void;
  nameLabel: string; // pl. 'Vevő' vagy 'Beszállító'
};

const statuses = Object.keys(statusLabels) as OrderStatus[];

export default function OrderFilterPanel({ id, value, onChange, onClear, nameLabel }: OrderFilterPanelProps) {
  const update = (patch: Partial<OrderFilter>) => onChange({ ...value, ...patch });
  const rangeInvalid = isDateRangeInvalid(value);

  return (
    <section id={id} className="order-filter-panel" aria-label="Rendelések szűrése">
      <Row className="g-3 align-items-end">
        <Col xs={12} md={4}>
          <Form.Group controlId={`${id}-name`}>
            <Form.Label>{nameLabel}</Form.Label>
            <Form.Control
              type="search"
              size="sm"
              placeholder={`${nameLabel} neve…`}
              value={value.name}
              onChange={(e) => update({ name: e.target.value })}
            />
          </Form.Group>
        </Col>

        <Col xs={12} sm={6} md={2}>
          <Form.Group controlId={`${id}-status`}>
            <Form.Label>Státusz</Form.Label>
            <Form.Select
              size="sm"
              value={value.status}
              onChange={(e) => update({ status: e.target.value as OrderStatus | '' })}
            >
              <option value="">Összes</option>
              {statuses.map((status) => (
                <option key={status} value={status}>{statusLabels[status]}</option>
              ))}
            </Form.Select>
          </Form.Group>
        </Col>

        <Col xs={6} sm={3} md={2}>
          <Form.Group controlId={`${id}-from`}>
            <Form.Label>Dátumtól</Form.Label>
            <Form.Control
              type="date"
              size="sm"
              value={value.from}
              max={value.to || undefined}
              onChange={(e) => update({ from: e.target.value })}
            />
          </Form.Group>
        </Col>

        <Col xs={6} sm={3} md={2}>
          <Form.Group controlId={`${id}-to`}>
            <Form.Label>Dátumig</Form.Label>
            <Form.Control
              type="date"
              size="sm"
              value={value.to}
              min={value.from || undefined}
              onChange={(e) => update({ to: e.target.value })}
              isInvalid={rangeInvalid}
            />
          </Form.Group>
        </Col>

        <Col xs={12} md={2} className="text-md-end">
          <Button
            variant="outline-light"
            size="sm"
            onClick={onClear}
            disabled={countActiveFilters(value) === 0}
          >
            <i className="bi bi-x-lg me-1" aria-hidden="true"></i>
            Szűrők törlése
          </Button>
        </Col>
      </Row>

      {rangeInvalid && (
        <p className="order-filter-error" role="alert">
          A „Dátumig” nem lehet korábbi a „Dátumtól” értéknél.
        </p>
      )}
      <p className="order-filter-hint">Egyetlen napra szűréshez add meg ugyanazt a dátumot mindkét mezőben.</p>
    </section>
  );
}
