import type { AuthSession, CurrentUser } from '../util/Types.js';
import { apiFetch, readValidationErrors } from '../util/apiErrors.js';

const AUTH_URL = import.meta.env.VITE_API_BASE_URL + '/auth';

const authService = {
  // Sima fetch: itt a 401 hibás jelszót jelent, nem lejárt bejelentkezést (az üzenetet a backend adja)
  login: async (userName: string, password: string): Promise<AuthSession> => {
    const response = await fetch(`${AUTH_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName, password }),
    });
    if (response.status === 400 || response.status === 401) {
      throw new Error(await readValidationErrors(response, 'Hibás felhasználónév vagy jelszó.'));
    }
    if (!response.ok) throw new Error('Nem sikerült a bejelentkezés.');
    return response.json();
  },

  // A tárolt token ellenőrzése (érvénytelen tokennél az apiFetch kijelentkeztet)
  me: async (): Promise<CurrentUser> => {
    const response = await apiFetch(`${AUTH_URL}/me`);
    if (!response.ok) throw new Error('Nem sikerült a felhasználó lekérése.');
    return response.json();
  },
};

export default authService;
