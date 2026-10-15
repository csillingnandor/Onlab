import { useCallback, useLayoutEffect, useMemo, useState, type ReactNode } from 'react';
import { dictionaries, type Language } from './translations.js';
import { I18nContext, type I18nContextValue } from './useI18n.js';

// A felület nyelve (magyar / angol); a választás a localStorage-ban marad meg, alapértelmezés a magyar.
const STORAGE_KEY = 'ioms.language';

function loadLanguage(): Language {
  try {
    return localStorage.getItem(STORAGE_KEY) === 'en' ? 'en' : 'hu';
  } catch {
    return 'hu';
  }
}

export default function I18nProvider({ children }: { children: ReactNode }) {
  const [language, setLanguageState] = useState<Language>(loadLanguage);

  // A <html lang> a képernyőolvasóknak és a böngésző elválasztásának / helyesírás-ellenőrzésének
  useLayoutEffect(() => {
    document.documentElement.lang = language;
  }, [language]);

  const setLanguage = useCallback((next: Language) => {
    try {
      localStorage.setItem(STORAGE_KEY, next);
    } catch {
      // Tiltott tároló (pl. privát mód): a választás csak a munkamenet végéig él
    }
    setLanguageState(next);
  }, []);

  const value = useMemo<I18nContextValue>(() => {
    const dictionary = dictionaries[language];
    return {
      language,
      setLanguage,
      t: (key, params) => {
        const text = dictionary[key];
        if (!params) return text;
        return text.replace(/\{(\w+)\}/g, (match, name: string) => (name in params ? String(params[name]) : match));
      },
    };
  }, [language, setLanguage]);

  return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
}
