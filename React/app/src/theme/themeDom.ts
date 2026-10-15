import darklyUrl from 'bootswatch/dist/darkly/bootstrap.min.css?url';
import flatlyUrl from 'bootswatch/dist/flatly/bootstrap.min.css?url';

// A téma alkalmazása a dokumentumra: <html data-theme / data-bs-theme> és a Bootstrap stíluslap
// (sötét: bootswatch Darkly, világos: a testvértémája, a Flatly). A Darkly a data-bs-theme="light"
// alatt is sötét színeket ad, ezért a stíluslapot is cserélni kell.
export type Theme = 'light' | 'dark';

const STORAGE_KEY = 'ioms.theme';

// A felhasználó választása; null, ha még nem választott (ilyenkor a rendszerbeállítás érvényes)
export function loadStoredTheme(): Theme | null {
  try {
    const value = localStorage.getItem(STORAGE_KEY);
    return value === 'light' || value === 'dark' ? value : null;
  } catch {
    return null;
  }
}

export function storeTheme(theme: Theme): void {
  try {
    localStorage.setItem(STORAGE_KEY, theme);
  } catch {
    // Tiltott tároló (pl. privát mód): a választás csak a munkamenet végéig él
  }
}

const darkQuery = window.matchMedia('(prefers-color-scheme: dark)');

export function systemTheme(): Theme {
  return darkQuery.matches ? 'dark' : 'light';
}

// A visszaadott függvénnyel lehet leiratkozni
export function onSystemThemeChange(listener: (theme: Theme) => void): () => void {
  const handler = () => listener(systemTheme());
  darkQuery.addEventListener('change', handler);
  return () => darkQuery.removeEventListener('change', handler);
}

// Mindkét stíluslap betöltve, csak az aktív érvényes (media="all"), így a váltás villanás nélküli.
// A <head> elejére kerülnek, hogy a saját CSS-fájlok felülírhassák őket.
function createStylesheet(href: string): HTMLLinkElement {
  const link = document.createElement('link');
  link.rel = 'stylesheet';
  link.href = href;
  link.media = 'not all';
  return link;
}

const stylesheets: Record<Theme, HTMLLinkElement> = {
  dark: createStylesheet(darklyUrl),
  light: createStylesheet(flatlyUrl),
};
document.head.prepend(stylesheets.dark, stylesheets.light);

export function applyTheme(theme: Theme): void {
  const root = document.documentElement;
  root.setAttribute('data-theme', theme);
  root.setAttribute('data-bs-theme', theme);
  stylesheets.dark.media = theme === 'dark' ? 'all' : 'not all';
  stylesheets.light.media = theme === 'light' ? 'all' : 'not all';
}

// Már a React indulása előtt (main.jsx importálja elsőként), hogy ne villanjon a rossz téma
applyTheme(loadStoredTheme() ?? systemTheme());
