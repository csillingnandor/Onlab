import { clearSession, loadSession } from '../auth/authStorage.js';

// A backend 400-as ValidationProblemDetails válaszából kiszedi az üzeneteket
// (pl. "Már létezik vevő ezzel az e-mail címmel: ..."), egyébként a megadott általános üzenetet adja.
export async function readValidationErrors(response: Response, fallback: string): Promise<string> {
  try {
    const problem: { errors?: Record<string, string[]> } = await response.json();
    const messages = Object.values(problem.errors ?? {}).flat();
    if (messages.length > 0) return messages.join(' ');
  } catch {
    // nem JSON válasz: marad az általános üzenet
  }
  return fallback;
}

// fetch a bejelentkezett felhasználó tokenjével. 401: lejárt / érvénytelen token, ilyenkor kijelentkeztet
// (az AuthProvider a bejelentkező oldalra vált); 403: a felhasználó szerepkörei nem engedik a műveletet.
export async function apiFetch(url: string, init: RequestInit = {}): Promise<Response> {
  const session = loadSession();
  const headers = new Headers(init.headers);
  if (session) headers.set('Authorization', `Bearer ${session.token}`);

  const response = await fetch(url, { ...init, headers });
  if (response.status === 401) {
    clearSession();
    throw new Error('A bejelentkezés lejárt, jelentkezz be újra.');
  }
  if (response.status === 403) throw new Error('Nincs jogosultságod ehhez a művelethez.');
  return response;
}

// JSON törzsű kérés (POST / PUT); hiba esetén a backend validációs üzeneteivel dob.
async function sendJson<T>(method: 'POST' | 'PUT', url: string, body: unknown, errorMessage: string): Promise<T> {
  const response = await apiFetch(url, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (response.status === 400 || response.status === 409) {
    throw new Error(await readValidationErrors(response, errorMessage));
  }
  if (response.status === 404) throw new Error('A módosítandó elem már nem létezik.');
  if (!response.ok) throw new Error(errorMessage);
  return response.json();
}

export const postJson = <T>(url: string, body: unknown, errorMessage: string): Promise<T> =>
  sendJson<T>('POST', url, body, errorMessage);

export const putJson = <T>(url: string, body: unknown, errorMessage: string): Promise<T> =>
  sendJson<T>('PUT', url, body, errorMessage);

// Fájl letöltése a bejelentkezett felhasználó tokenjével (pl. Excel export); a fájlnevet a szerver adja
export async function downloadFile(url: string, fallbackName: string, errorMessage: string): Promise<void> {
  const response = await apiFetch(url);
  if (!response.ok) throw new Error(errorMessage);

  const disposition = response.headers.get('Content-Disposition') ?? '';
  const match = /filename\*=UTF-8''([^;]+)|filename="?([^";]+)"?/i.exec(disposition);
  const name = match ? decodeURIComponent(match[1] ?? match[2] ?? fallbackName) : fallbackName;

  const blobUrl = URL.createObjectURL(await response.blob());
  const link = document.createElement('a');
  link.href = blobUrl;
  link.download = name;
  link.click();
  URL.revokeObjectURL(blobUrl);
}

// DELETE; 409: a backend elutasította (pl. hivatkoznak az elemre), az üzenetét adjuk tovább
export async function deleteRequest(url: string, errorMessage: string): Promise<void> {
  const response = await apiFetch(url, { method: 'DELETE' });
  if (response.status === 404) throw new Error('Az elem már nem létezik.');
  if (response.status === 409) throw new Error(await readValidationErrors(response, errorMessage));
  if (!response.ok) throw new Error(errorMessage);
}
