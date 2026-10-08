import './StatusBadge.css';

export type StatusTone = 'success' | 'warning' | 'danger' | 'info' | 'neutral';

type StatusBadgeProps = {
  label: string;
  tone: StatusTone;
  className?: string | undefined;
};

// Kerekített állapotjelvény; a domain-specifikus jelvények (termék, rendelés) csak a feliratot és a színt adják meg.
export default function StatusBadge({ label, tone, className = '' }: StatusBadgeProps) {
  return <span className={`status-badge status-badge-${tone} ${className}`}>{label}</span>;
}
