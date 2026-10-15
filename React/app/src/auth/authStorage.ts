import type { AuthSession } from '../util/Types.js';

// A bejelentkezés (token + felhasználó) a localStorage-ban, hogy újratöltés után is megmaradjon.
// Az apiFetch innen olvassa a tokent, az AuthProvider pedig feliratkozik a kijelentkeztetésre (lejárt / érvénytelen token).
const STORAGE_KEY = 'ioms.auth';

type Listener = () => void;
const listeners = new Set<Listener>();

export function loadSession(): AuthSession | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const session: AuthSession = JSON.parse(raw);
    // A lejárt tokennel úgyis 401 jönne, ezért el sem kezdjük használni
    return new Date(session.expiresAt).getTime() > Date.now() ? session : null;
  } catch {
    return null;
  }
}

export function saveSession(session: AuthSession): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
}

export function clearSession(): void {
  localStorage.removeItem(STORAGE_KEY);
  listeners.forEach((listener) => listener());
}

// A visszaadott függvénnyel lehet leiratkozni
export function onSessionCleared(listener: Listener): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}
