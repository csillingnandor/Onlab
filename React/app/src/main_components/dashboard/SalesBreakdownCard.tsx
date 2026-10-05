import { useState } from 'react';
import { Card, ToggleButton, ToggleButtonGroup } from 'react-bootstrap';
import type { SalesBreakdownRow, SalesBreakdowns } from '../../util/dashboardStats.js';
import { priceFormatter } from '../../util/format.js';

type Dimension = keyof SalesBreakdowns;
type Metric = 'revenue' | 'quantity';

const dimensions: { key: Dimension; label: string; empty: string }[] = [
  { key: 'byProduct', label: 'Termékenként', empty: 'termék' },
  { key: 'byCustomer', label: 'Vevőnként', empty: 'vevő' },
  { key: 'byCategory', label: 'Kategóriánként', empty: 'kategória' },
];

const MAX_ROWS = 8;

type SalesBreakdownCardProps = {
  breakdowns: SalesBreakdowns;
};

// Rangsor vízszintes sávokkal: a sáv hossza a legnagyobb értékhez, a % az összeshez viszonyít.
export default function SalesBreakdownCard({ breakdowns }: SalesBreakdownCardProps) {
  const [dimension, setDimension] = useState<Dimension>('byProduct');
  const [metric, setMetric] = useState<Metric>('revenue');

  const value = (row: SalesBreakdownRow) => (metric === 'revenue' ? row.revenue : row.quantity);
  const format = (n: number) => (metric === 'revenue' ? priceFormatter.format(n) : `${n} db`);

  const rows = [...breakdowns[dimension]].sort((a, b) => value(b) - value(a));
  const total = rows.reduce((sum, row) => sum + value(row), 0);
  const max = rows[0] ? value(rows[0]) : 0;
  const visible = rows.slice(0, MAX_ROWS);
  const hiddenCount = rows.length - visible.length;
  const current = dimensions.find((d) => d.key === dimension)!;

  return (
    <Card className="dashboard-card">
      <Card.Header className="dashboard-card-header">
        <span className="dashboard-card-title">Eladások bontása</span>

        <div className="sales-panel-controls">
          <ToggleButtonGroup
            type="radio"
            name="breakdown-dimension"
            size="sm"
            value={dimension}
            onChange={(next: Dimension) => setDimension(next)}
          >
            {dimensions.map((d) => (
              <ToggleButton key={d.key} id={`breakdown-${d.key}`} value={d.key} variant="outline-light">
                {d.label}
              </ToggleButton>
            ))}
          </ToggleButtonGroup>

          <ToggleButtonGroup
            type="radio"
            name="breakdown-metric"
            size="sm"
            value={metric}
            onChange={(next: Metric) => setMetric(next)}
          >
            <ToggleButton id="breakdown-revenue" value="revenue" variant="outline-light">Bevétel</ToggleButton>
            <ToggleButton id="breakdown-quantity" value="quantity" variant="outline-light">Darab</ToggleButton>
          </ToggleButtonGroup>
        </div>
      </Card.Header>

      <Card.Body>
        {rows.length === 0 ? (
          <p className="dashboard-empty">Ebben az időszakban nem volt kiszállított eladás.</p>
        ) : (
          <>
            <ol className="breakdown-list">
              {visible.map((row, index) => {
                const share = total > 0 ? Math.round((value(row) / total) * 100) : 0;
                return (
                  <li key={row.key} className="breakdown-row">
                    <span className="breakdown-rank">{index + 1}</span>
                    <div className="breakdown-main">
                      <div className="breakdown-head">
                        <span className="breakdown-label" title={row.label}>{row.label}</span>
                        <span className="breakdown-value">{format(value(row))}</span>
                      </div>
                      <div className="breakdown-bar" aria-hidden="true">
                        <div
                          className="breakdown-fill"
                          style={{ width: `${max > 0 ? (value(row) / max) * 100 : 0}%` }}
                        ></div>
                      </div>
                      <span className="dashboard-muted">
                        {share}% · {metric === 'revenue' ? `${row.quantity} db` : priceFormatter.format(row.revenue)}
                        {' · '}{row.orderCount} rendelés
                      </span>
                    </div>
                  </li>
                );
              })}
            </ol>
            <p className="breakdown-footer">
              Összesen: <strong>{format(total)}</strong> · {rows.length} {current.empty}
              {hiddenCount > 0 && ` (további ${hiddenCount} nem látszik)`}
            </p>
          </>
        )}
      </Card.Body>
    </Card>
  );
}
