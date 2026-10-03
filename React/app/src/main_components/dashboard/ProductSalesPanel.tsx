import { useEffect, useState } from 'react';
import { Alert, Card, Spinner, ToggleButton, ToggleButtonGroup } from 'react-bootstrap';
import productService from '../../services/productService.js';
import type { Product, ProductSaleStatistics } from '../../util/Types.js';
import { presetRange, type DateRange } from '../../util/dateRange.js';
import DateRangeMenu from './DateRangeMenu.js';
import ProductSelect from './ProductSelect.js';
import SalesBarChart, { type SalesMetric } from './SalesBarChart.js';

export default function ProductSalesPanel() {
  const [products, setProducts] = useState<Product[]>([]);
  const [productsLoading, setProductsLoading] = useState<boolean>(true);
  const [productId, setProductId] = useState<number | null>(null);
  const [range, setRange] = useState<DateRange>(() => presetRange('month'));
  const [metric, setMetric] = useState<SalesMetric>('quantity');
  const [statistics, setStatistics] = useState<ProductSaleStatistics | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // Terméklista a választóhoz: egyszer töltjük be, és alapból az első (név szerint) termék lesz kiválasztva.
  useEffect(() => {
    let ignore = false;

    productService.getAll()
      .then((data) => {
        if (ignore) return;
        const sorted = [...data].sort((a, b) => a.name.localeCompare(b.name, 'hu'));
        setProducts(sorted);
        setProductId((current) => current ?? sorted[0]?.id ?? null);
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Nem sikerült a termékek betöltése.');
      })
      .finally(() => {
        if (!ignore) setProductsLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, []);

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
      </div>
    );
  };

  return (
    <Card>
      <Card.Header className="sales-panel-header">
        <div className="sales-panel-title">
          <i className="bi bi-bar-chart me-2" aria-hidden="true"></i>
          Termékeladások
        </div>

        <div className="sales-panel-controls">
          <ProductSelect
            products={products}
            value={productId}
            onChange={setProductId}
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

          <DateRangeMenu value={range} onChange={setRange} />
        </div>
      </Card.Header>

      <Card.Body>{renderBody()}</Card.Body>
    </Card>
  );
}
