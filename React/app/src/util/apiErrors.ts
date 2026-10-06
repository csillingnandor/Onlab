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

// POST kérés JSON törzzsel; hiba esetén a backend validációs üzeneteivel dob.
export async function postJson<T>(url: string, body: unknown, errorMessage: string): Promise<T> {
  const response = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (response.status === 400) throw new Error(await readValidationErrors(response, errorMessage));
  if (!response.ok) throw new Error(errorMessage);
  return response.json();
}
