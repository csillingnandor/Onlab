import { createContext, useContext } from 'react';
import type { Theme } from './themeDom.js';

export type ThemeContextValue = {
  theme: Theme;
  toggleTheme: () => void;
};

export const ThemeContext = createContext<ThemeContextValue | null>(null);

// Az aktuális téma és a váltás; csak a ThemeProvider-en belül használható
export function useTheme(): ThemeContextValue {
  const value = useContext(ThemeContext);
  if (!value) throw new Error('useTheme csak ThemeProvider-en belül használható.');
  return value;
}
