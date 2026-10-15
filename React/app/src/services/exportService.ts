import { downloadFile } from '../util/apiErrors.js';

const EXPORT_URL = import.meta.env.VITE_API_BASE_URL + '/export';

// Excel export listánként (a backend ExportController végpontjai)
export type ExportKind = 'products' | 'customers' | 'suppliers' | 'warehouses' | 'customer-orders' | 'supplier-orders';

export const exportList = (kind: ExportKind): Promise<void> =>
  downloadFile(`${EXPORT_URL}/${kind}`, `${kind}.xlsx`, 'Nem sikerült az exportálás.');
