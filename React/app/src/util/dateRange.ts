// Dátumok 'YYYY-MM-DD' alakban (időpont nélkül), ahogy a backend DateOnly-ként várja és küldi.
// Szándékosan nem használunk toISOString-et: az UTC-ben számol, és éjfél körül egy napot csúszhat.

export type DateRangePreset = 'day' | 'week' | 'month' | 'custom';

export type DateRange = {
  preset: DateRangePreset;
  from: string;
  to: string;
};

export const presetLabels: Record<DateRangePreset, string> = {
  day: 'Utolsó nap',
  week: 'Utolsó hét',
  month: 'Utolsó hónap',
  custom: 'Egyedi időszak',
};

// Hány napot fed le a preset, a mai nappal együtt
const presetDays: Record<Exclude<DateRangePreset, 'custom'>, number> = {
  day: 1,
  week: 7,
  month: 30,
};

export function toIsoDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function presetRange(preset: Exclude<DateRangePreset, 'custom'>, today: Date = new Date()): DateRange {
  const from = new Date(today);
  from.setDate(today.getDate() - (presetDays[preset] - 1));
  return { preset, from: toIsoDate(from), to: toIsoDate(today) };
}

// '2026-09-01' -> '2026.09.01.'
export function formatIsoDate(isoDate: string): string {
  return `${isoDate.replaceAll('-', '.')}.`;
}

// '2026-09-01' -> '09.01.' (tengelyfeliratnak)
export function formatShortIsoDate(isoDate: string): string {
  return `${isoDate.slice(5).replace('-', '.')}.`;
}
