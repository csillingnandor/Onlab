import { Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import type { StockAlert } from '../../util/dashboardStats.js';

type StockAlertsCardProps = {
  alerts: StockAlert[];
};

// A készletszint nem függ a kiválasztott időszaktól: mindig az aktuális állapotot mutatja.
export default function StockAlertsCard({ alerts }: StockAlertsCardProps) {
  return (
    <Card className="dashboard-card h-100">
      <Card.Header className="dashboard-card-header">
        <span className="dashboard-card-title">Készletriasztások</span>
        <Link to="/products" className="dashboard-card-link">Termékek</Link>
      </Card.Header>
      <Card.Body>
        {alerts.length === 0 ? (
          <p className="dashboard-empty">Minden termékből van elég készlet.</p>
        ) : (
          <ul className="stock-alert-list">
            {alerts.map(({ product, fillPercent }) => {
              const isOut = product.status === 'Out of Stock';
              return (
                <li key={product.id} className={`stock-alert ${isOut ? 'is-out' : 'is-low'}`}>
                  <div className="stock-alert-row">
                    <span className="stock-alert-name">{product.name}</span>
                    <span className="stock-alert-level">
                      {product.stockQuantity} / {product.minStockLevel} db
                    </span>
                  </div>
                  <div
                    className="stock-alert-bar"
                    role="meter"
                    aria-label={`${product.name} készlete a minimumhoz képest`}
                    aria-valuemin={0}
                    aria-valuemax={100}
                    aria-valuenow={fillPercent}
                  >
                    <div className="stock-alert-fill" style={{ width: `${fillPercent}%` }}></div>
                  </div>
                  <span className="dashboard-muted">{isOut ? 'Elfogyott' : 'Alacsony készlet'}</span>
                </li>
              );
            })}
          </ul>
        )}
      </Card.Body>
    </Card>
  );
}
