// Közös segédfüggvények a kliensoldali listaszűrőkhöz (termék, raktár, vevő).
// A szűrők mezői a beviteli mezők nyers szöveges értékei; '' = nincs megadva.

export const normalizeText = (text: string) => text.trim().toLocaleLowerCase('hu-HU');

// A (már normalizált) keresőkifejezés bármelyik mezőben előfordul-e; üres kifejezés mindenre illeszkedik.
export function matchesText(term: string, ...values: (string | null | undefined)[]): boolean {
  return !term || values.some((value) => value != null && normalizeText(value).includes(term));
}

// Zárt intervallum; az üres határ nem szűr
export function inRange(value: number, min: string, max: string): boolean {
  return (min === '' || value >= Number(min)) && (max === '' || value <= Number(max));
}

export function isRangeInvalid(min: string, max: string): boolean {
  return min !== '' && max !== '' && Number(min) > Number(max);
}

export function countActive(filter: Record<string, string>): number {
  return Object.values(filter).filter((value) => value.trim() !== '').length;
}
