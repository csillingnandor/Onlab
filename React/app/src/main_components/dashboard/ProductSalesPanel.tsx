import { useEffect, useState } from 'react';
import { Alert, Card, Spinner, ToggleButton, ToggleButtonGroup } from 'react-bootstrap';
import productService from '../../services/productService.js';
import type { Product, ProductSaleStatistics } from '../../util/Types.js';
import type { DateRange } from '../../util/dateRange.js';
import { priceFormatter } from '../../util/format.js';
import ProductSelect from './ProductSelect.js';
import SalesBarChart, { type SalesMetric } from './SalesBarChart.js';

type ProductSalesPanelProps = {
  products: Product[]; // név szerint rendezve; a műszerfal tölti be
  productsLoading: boolean;
  range: DateRange; // a műszerfal közös időszaka
};

export default function ProductSalesPanel({ products, productsLoading, range }: ProductSalesPanelProps) {
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [metric, setMetric] = useState<SalesMetric>('quantity');
  const [statistics, setStatistics] = useState<ProductSaleStatistics | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // Amíg a felhasználó nem választott, az első termék látszik
  const productId = selectedId ?? products[0]?.id ?? null;

  // Statisztika: a termék vagy az időszak változásakor újratöltjük.
  useEffect(() => {
    if (productId === null) return;

    let ignore = false; // gyors váltáskor a régi választ eldobjuk

    setLoading(true);
    setError(null);

    productService.getSaleStatistics(productId, range.from, range.to)
      .then((data) => {
        if (!ignore) setStatistics(data);
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.');
      })
      .finally(() => {
        if (!ignore) setLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, [productId, range.from, range.to]);

  const renderBody = () => {
    if (error) return <Alert variant="danger" className="mb-0">{error}</Alert>;

    if (!productsLoading && products.length === 0) {
      return <Alert variant="info" className="mb-0">Még nincs termék a rendszerben.</Alert>;
    }

    if (!statistics) {
      return (
        <div className="text-center py-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Betöltés...</span>
          </Spinner>
        </div>
      );
    }

    return (
      // Újratöltéskor a régi diagram halványan látszik, így nem ugrál az elrendezés
      <div className={loading ? 'sales-panel-body is-loading' : 'sales-panel-body'}>
        {statistics.totalQuantity === 0 && (
          <Alert variant="info" className="py-2">
            Ebben az időszakban nem volt kiszállított eladás.
          </Alert>
        )}
        <SalesBarChart daily={statistics.daily} metric={metric} />
        <dl className="sales-summary">
          <div>
            <dt>Eladott</dt>
            <dd>{statistics.totalQuantity} db</dd>
          </div>
          <div>
            <dt>Bevétel</dt>
            <dd>{priceFormatter.format(statistics.totalRevenue)}</dd>
          </div>
          <div>
            <dt>Átlagár</dt>
            <dd>{statistics.averageUnitPrice === null ? '–' : priceFormatter.format(statistics.averageUnitPrice)}</dd>
          </div>
        </dl>
      </div>
    );
  };

  return (
    <Card className="dashboard-card h-100">
      <Card.Header className="dashboard-card-header">
        <span className="dashboard-card-title">Termékeladások</span>

        <div className="sales-panel-controls">
          <ProductSelect
            products={products}
            value={productId}
            onChange={setSelectedId}
            disabled={productsLoading}
          />

          <ToggleButtonGroup
            type="radio"
            name="sales-metric"
            size="sm"
            value={metric}
            onChange={(value: SalesMetric) => setMetric(value)}
          >
            <ToggleButton id="sales-metric-quantity" value="quantity" variant="outline-light">
              Darab
            </ToggleButton>
            <ToggleButton id="sales-metric-revenue" value="revenue" variant="outline-light">
              Bevétel
            </ToggleButton>
          </ToggleButtonGroup>
        </div>
      </Card.Header>

      <Card.Body>{renderBody()}</Card.Body>
    </Card>
  );
}
