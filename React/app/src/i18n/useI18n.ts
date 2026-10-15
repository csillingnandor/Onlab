import { createContext, useContext } from 'react';
import type { Language, TranslationKey } from './translations.js';

export type TranslateParams = Record<string, string | number>;

export type I18nContextValue = {
  language: Language;
  setLanguage: (language: Language) => void;
  t: (key: TranslationKey, params?: TranslateParams) => string;
};

export const I18nContext = createContext<I18nContextValue | null>(null);

// Az aktuális nyelv és a fordító függvény; csak az I18nProvider-en belül használható
export function useI18n(): I18nContextValue {
  const value = useContext(I18nContext);
  if (!value) throw new Error('useI18n csak I18nProvider-en belül használható.');
  return value;
}
