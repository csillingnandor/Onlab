import type { ReactNode } from 'react';
import { Button, Row } from 'react-bootstrap';
import './Filter.css';

type FilterPanelProps = {
  id: string; // a FilterToggle ezzel hivatkozik a panelre
  label: string; // pl. 'Rendelések szűrése'
  activeCount: number;
  onClear: () => void;
  children: ReactNode; // a mezők, egy-egy <Col>-ban
  footer?: ReactNode; // hibaüzenet / tipp a mezők alatt
};

// Lenyíló szűrőpanel az oldal fejléce alatt; a törlés ikongomb mindig a mezők mellett, az utolsó sor végén áll.
export default function FilterPanel({ id, label, activeCount, onClear, children, footer }: FilterPanelProps) {
  return (
    <section id={id} className="filter-panel" aria-label={label}>
      <div className="filter-panel-body">
        <Row className="g-3 align-items-end filter-panel-fields">{children}</Row>

        <Button
          variant="outline-light"
          size="sm"
          className="filter-clear"
          onClick={onClear}
          disabled={activeCount === 0}
          aria-label="Szűrők törlése"
          title="Szűrők törlése"
        >
          <i className="bi bi-arrow-counterclockwise" aria-hidden="true"></i>
        </Button>
      </div>

      {footer}
    </section>
  );
}
