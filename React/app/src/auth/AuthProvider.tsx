import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react';
import authService from '../services/authService.js';
import type { AuthSession } from '../util/Types.js';
import type { ActionKey, PageKey } from '../util/pages.js';
import { clearSession, loadSession, onSessionCleared, saveSession } from './authStorage.js';
import { AuthContext, type AuthContextValue } from './useAuth.js';

// A bejelentkezés állapota az egész alkalmazásnak. Kijelentkeztet, ha a token lejár,
// vagy ha bármelyik API hívás 401-et kap (apiFetch -> clearSession).
export default function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(loadSession);

  useEffect(() => onSessionCleared(() => setSession(null)), []);

  // Induláskor a tárolt token ellenőrzése (pl. a szerver kulcsa azóta megváltozott)
  useEffect(() => {
    if (loadSession()) authService.me().catch(() => { /* 401 esetén az apiFetch már kijelentkeztetett */ });
  }, []);

  // Automatikus kijelentkezés a token lejáratakor
  useEffect(() => {
    if (!session) return;
    const timeout = setTimeout(clearSession, new Date(session.expiresAt).getTime() - Date.now());
    return () => clearTimeout(timeout);
  }, [session]);

  const login = useCallback(async (userName: string, password: string) => {
    const newSession = await authService.login(userName, password);
    saveSession(newSession);
    setSession(newSession);
  }, []);

  const value = useMemo<AuthContextValue>(() => {
    const pages = new Set<PageKey>(session?.user.pages ?? []);
    const actions = new Set<ActionKey>(session?.user.actions ?? []);
    return {
      user: session?.user ?? null,
      login,
      logout: clearSession,
      hasPage: (page) => pages.has(page),
      hasAction: (action) => actions.has(action),
    };
  }, [session, login]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
