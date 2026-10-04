type KpiCardProps = {
  label: string;
  value: string;
  sub: string;
  tone?: 'default' | 'warning';
};

export default function KpiCard({ label, value, sub, tone = 'default' }: KpiCardProps) {
  return (
    <div className="kpi-card">
      <span className="kpi-card-label">{label}</span>
      <span className="kpi-card-value">{value}</span>
      <span className={`kpi-card-sub ${tone === 'warning' ? 'is-warning' : ''}`}>{sub}</span>
    </div>
  );
}
