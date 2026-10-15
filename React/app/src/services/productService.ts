import type { Product, ProductImportPreview, ProductImportResult, ProductInput, ProductSaleStatistics, StockAdjustment, StockMovement } from '../util/Types.js';
import { apiFetch, downloadFile, postJson, readValidationErrors } from '../util/apiErrors.js';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL + '/products';
const IMPORT_URL = import.meta.env.VITE_API_BASE_URL + '/import/products';

const productService = {
  getAll: async (): Promise<Product[]> => {
    const response = await apiFetch(API_BASE_URL);
    if (!response.ok) throw new Error('Nem sikerült a termékek betöltése.');
    return response.json();
  },

  create: async (product: ProductInput): Promise<Product> => {
    const response = await apiFetch(API_BASE_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(product),
    });
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'Hibás termékadatok.'));
    if (!response.ok) throw new Error('Nem sikerült a termék mentése.');
    return response.json();
  },

  // A válasz a mentett termék a frissen számolt állapottal (status) együtt
  update: async (id: number, product: ProductInput): Promise<Product> => {
    const response = await apiFetch(`${API_BASE_URL}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(product),
    });
    if (response.status === 404) throw new Error('A termék már nem létezik.');
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'Hibás termékadatok.'));
    if (!response.ok) throw new Error('Nem sikerült a termék módosítása.');
    return response.json();
  },

  // 409: rendelés hivatkozik a termékre, ezért nem törölhető (az üzenetet a backend adja)
  delete: async (id: number): Promise<void> => {
    const response = await apiFetch(`${API_BASE_URL}/${id}`, { method: 'DELETE' });
    if (response.status === 404) throw new Error('A termék már nem létezik.');
    if (response.status === 409) throw new Error(await readValidationErrors(response, 'A termék nem törölhető.'));
    if (!response.ok) throw new Error('Nem sikerült a termék törlése.');
  },

  // Készlettörténet minden raktárból, legújabb elöl
  getMovements: async (productId: number): Promise<StockMovement[]> => {
    const response = await apiFetch(`${API_BASE_URL}/${productId}/movements`);
    if (response.status === 404) throw new Error('Nincs ilyen termék.');
    if (!response.ok) throw new Error('Nem sikerült a készlettörténet betöltése.');
    return response.json();
  },

  // Kézi korrekció (leltár): a válasz a frissített termék
  adjustStock: (productId: number, adjustment: StockAdjustment): Promise<Product> =>
    postJson(`${API_BASE_URL}/${productId}/adjustments`, adjustment, 'Nem sikerült a készlet korrigálása.'),

  downloadImportTemplate: (): Promise<void> =>
    downloadFile(`${IMPORT_URL}/template`, 'termek-import-sablon.xlsx', 'Nem sikerült a sablon letöltése.'),

  // Előnézet: a fájl sorai a hibáikkal, mentés nélkül
  previewImport: async (file: File): Promise<ProductImportPreview> => {
    const response = await apiFetch(`${IMPORT_URL}/preview`, { method: 'POST', body: formWith(file) });
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'A fájl nem dolgozható fel.'));
    if (!response.ok) throw new Error('Nem sikerült a fájl feldolgozása.');
    return response.json();
  },

  // Mentés: a szerver újra feldolgozza a fájlt, és csak hibátlan fájlt fogad el
  importProducts: async (file: File): Promise<ProductImportResult> => {
    const response = await apiFetch(IMPORT_URL, { method: 'POST', body: formWith(file) });
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'A fájl hibás, nem importálható.'));
    if (!response.ok) throw new Error('Nem sikerült az importálás.');
    return response.json();
  },

  // from / to: 'YYYY-MM-DD'; ha elhagyjuk, a backend az utolsó 30 napot adja
  getSaleStatistics: async (productId: number, from?: string, to?: string): Promise<ProductSaleStatistics> => {
    const params = new URLSearchParams();
    if (from) params.set('from', from);
    if (to) params.set('to', to);
    const query = params.size > 0 ? `?${params}` : '';

    const response = await apiFetch(`${API_BASE_URL}/${productId}/sales${query}`);
    if (response.status === 404) throw new Error('Nincs ilyen termék.');
    if (response.status === 400) throw new Error(await readValidationErrors(response, 'Hibás lekérdezési paraméterek.'));
    if (!response.ok) throw new Error('Nem sikerült a termék eladási statisztikáinak lekérése.');
    return response.json();
  },
};

export default productService;

function formWith(file: File): FormData {
  const form = new FormData();
  form.append('file', file);
  return form;
}
