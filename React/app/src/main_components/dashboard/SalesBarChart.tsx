import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import type { DailyProductSale } from '../../util/Types.js';
import { compactPriceFormatter, priceFormatter } from '../../util/format.js';
import { formatIsoDate, formatShortIsoDate } from '../../util/dateRange.js';

export type SalesMetric = 'quantity' | 'revenue';

type SalesBarChartProps = {
  daily: DailyProductSale[];
  metric: SalesMetric;
};

const metricLabels: Record<SalesMetric, string> = {
  quantity: 'Eladott darab',
  revenue: 'Bevétel',
};

function formatValue(metric: SalesMetric, value: number): string {
  return metric === 'revenue' ? priceFormatter.format(value) : `${value} db`;
}

// Csak megjelenít: az adatot a szülő tölti be, így más paneleken is újrahasználható.
export default function SalesBarChart({ daily, metric }: SalesBarChartProps) {
  return (
    // A ResponsiveContainer a szülő méretét veszi át, ezért a wrappernek fix magasság kell.
    <div className="sales-chart">
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={daily} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
          <CartesianGrid stroke="var(--bs-border-color)" strokeDasharray="3 3" vertical={false} />
          <XAxis
            dataKey="date"
            tickFormatter={formatShortIsoDate}
            tick={{ fill: 'var(--bs-secondary-color)', fontSize: 12 }}
            stroke="var(--bs-border-color)"
            interval="preserveStartEnd"
            minTickGap={12}
          />
          <YAxis
            width={72}
            allowDecimals={metric === 'revenue'}
            tickFormatter={(value: number) =>
              metric === 'revenue' ? compactPriceFormatter.format(value) : String(value)
            }
            tick={{ fill: 'var(--bs-secondary-color)', fontSize: 12 }}
            stroke="var(--bs-border-color)"
          />
          <Tooltip
            cursor={{ fill: 'rgba(255, 255, 255, 0.05)' }}
            contentStyle={{
              backgroundColor: 'var(--bs-body-bg)',
              border: '1px solid var(--bs-border-color)',
              borderRadius: '0.375rem',
            }}
            labelStyle={{ color: 'var(--bs-body-color)' }}
            itemStyle={{ color: 'var(--bs-body-color)' }}
            labelFormatter={(label) => formatIsoDate(String(label))}
            formatter={(value) => [formatValue(metric, Number(value)), metricLabels[metric]]}
          />
          <Bar dataKey={metric} name={metricLabels[metric]} fill="var(--bs-primary)" radius={[4, 4, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
