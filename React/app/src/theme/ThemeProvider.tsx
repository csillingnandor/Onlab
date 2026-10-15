import { useEffect, useLayoutEffect, useMemo, useState, type ReactNode } from 'react';
import { applyTheme, loadStoredTheme, onSystemThemeChange, storeTheme, systemTheme, type Theme } from './themeDom.js';
import { ThemeContext, type ThemeContextValue } from './useTheme.js';

// Világos / sötét téma. Amíg a felhasználó nem választ, a rendszerbeállítást követi (élőben is);
// a választás a localStorage-ban marad meg.
export default function ThemeProvider({ children }: { children: ReactNode }) {
  const [chosen, setChosen] = useState<Theme | null>(loadStoredTheme);
  const [system, setSystem] = useState<Theme>(systemTheme);

  useEffect(() => onSystemThemeChange(setSystem), []);

  const theme = chosen ?? system;

  useLayoutEffect(() => applyTheme(theme), [theme]);

  const value = useMemo<ThemeContextValue>(() => ({
    theme,
    toggleTheme: () => {
      const next: Theme = theme === 'dark' ? 'light' : 'dark';
      storeTheme(next);
      setChosen(next);
    },
  }), [theme]);

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}
