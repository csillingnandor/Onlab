import { createContext, useContext } from 'react';
import type { CurrentUser } from '../util/Types.js';
import type { ActionKey, PageKey } from '../util/pages.js';

export type AuthContextValue = {
  user: CurrentUser | null;
  login: (userName: string, password: string) => Promise<void>;
  logout: () => void;
  hasPage: (page: PageKey) => boolean;
  // Oldalon belüli művelet (pl. rendelés lemondása); a backend ugyanezt ellenőrzi
  hasAction: (action: ActionKey) => boolean;
};

export const AuthContext = createContext<AuthContextValue | null>(null);

// A bejelentkezett felhasználó és a jogosultságai; csak az AuthProvider-en belül használható
export function useAuth(): AuthContextValue {
  const value = useContext(AuthContext);
  if (!value) throw new Error('useAuth csak AuthProvider-en belül használható.');
  return value;
}
