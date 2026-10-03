import { Col, Row } from 'react-bootstrap';
import ProductSalesPanel from './ProductSalesPanel.js';
import './DashboardPage.css';

export default function DashboardPage() {
  return (
    <div className="dashboard-page">
      <header className="dashboard-page-header">
        <h1>Műszerfal</h1>
        <p className="dashboard-page-subtitle">
          Itt kezelheted a termékeket, megrendeléseket és statisztikákat.
        </p>
      </header>

      <Row className="g-4">
        <Col xs={12}>
          <ProductSalesPanel />
        </Col>
      </Row>
    </div>
  );
}
