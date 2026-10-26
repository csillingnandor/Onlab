import type { ReactNode } from 'react';
import type { LocationStock, StorageLocation } from '../../../util/Types.js';
import type { ScenePalette } from './scenePalette.js';

type LocationPanelProps = {
  location: StorageLocation | null; // a 3D nézetben kiválasztott hely
  unplacedStock: LocationStock[];
  palette: ScenePalette;
  highlightLabel: string | null; // kiemelés van érvényben (pl. 'A felvétel helyei'): más a jelmagyarázat
  showRouteLegend: boolean;
  children?: ReactNode; // a panel elejére kerülő további szakasz (felvételi útvonal)
};

function StockList({ stocks }: { stocks: LocationStock[] }) {
  return (
    <ul className="layout3d-stock-list">
      {stocks.map((s) => (
        <li key={s.productId}>
          <span>
            {s.productName}
            <small className="data-table-mono">{s.productSku}</small>
          </span>
          <strong>{s.quantity} db</strong>
        </li>
      ))}
    </ul>
  );
}

// A 3D nézet melletti panel: a kiválasztott tárolóhely tartalma, jelmagyarázat és a depón lévő készlet
export default function LocationPanel({
  location,
  unplacedStock,
  palette,
  highlightLabel,
  showRouteLegend,
  children,
}: LocationPanelProps) {
  const legend = highlightLabel
    ? [
      { color: palette.highlight, label: highlightLabel },
      { color: palette.emptyBin, label: 'Más / üres hely' },
    ]
    : [
      { color: palette.fillLow, label: 'Alig foglalt' },
      { color: palette.fillHigh, label: 'Tele' },
      { color: palette.emptyBin, label: 'Üres (áttetsző)' },
    ];
  if (showRouteLegend) {
    legend.push({ color: palette.route, label: 'Útvonal és megállók' }, { color: palette.picker, label: 'Árufelvevő' });
  }

  return (
    <aside className="layout3d-panel">
      {children}

      <section>
        <h2>Tárolóhely</h2>
        {location ? (
          <>
            <p className="layout3d-location-code">{location.code}</p>
            <p className="layout3d-muted">
              {location.bay}. rekesz, {location.level}. szint · {location.quantity} / {location.maxQuantity} db
            </p>
            {location.stocks.length > 0
              ? <StockList stocks={location.stocks} />
              : <p className="layout3d-muted">A hely üres.</p>}
          </>
        ) : (
          <p className="layout3d-muted">Kattints egy tárolóhelyre a tartalmáért.</p>
        )}
      </section>

      <section>
        <h2>Jelmagyarázat</h2>
        <ul className="layout3d-legend">
          {legend.map((item) => (
            <li key={item.label}>
              <span className="layout3d-swatch" style={{ background: item.color }} aria-hidden="true"></span>
              {item.label}
            </li>
          ))}
          <li>
            <span className="layout3d-swatch" style={{ background: palette.dock }} aria-hidden="true"></span>
            Depó
          </li>
        </ul>
        <p className="layout3d-muted small mb-0">A doboz magassága a hely telítettségével arányos.</p>
      </section>

      <section>
        <h2>Depón, elhelyezésre vár</h2>
        {unplacedStock.length > 0
          ? <StockList stocks={unplacedStock} />
          : <p className="layout3d-muted">Minden készlet helyhez van rendelve.</p>}
      </section>
    </aside>
  );
}
