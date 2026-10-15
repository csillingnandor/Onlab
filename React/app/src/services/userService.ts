import type { NewUser, Role, RoleInput, User, UserUpdate } from '../util/Types.js';
import { apiFetch, deleteRequest, postJson, putJson } from '../util/apiErrors.js';

const USERS_URL = import.meta.env.VITE_API_BASE_URL + '/users';
const ROLES_URL = import.meta.env.VITE_API_BASE_URL + '/roles';
const NOTIFICATIONS_URL = import.meta.env.VITE_API_BASE_URL + '/notifications';

// Felhasználók és szerepkörök kezelése (csak a "users" oldal jogosultságával)
const userService = {
  getAll: async (): Promise<User[]> => {
    const response = await apiFetch(USERS_URL);
    if (!response.ok) throw new Error('Nem sikerült a felhasználók betöltése.');
    return response.json();
  },

  create: (user: NewUser): Promise<User> =>
    postJson(USERS_URL, user, 'Nem sikerült a felhasználó mentése.'),

  update: (id: string, user: UserUpdate): Promise<User> =>
    putJson(`${USERS_URL}/${id}`, user, 'Nem sikerült a felhasználó módosítása.'),

  // 409: saját magát vagy az utolsó adminisztrátort nem lehet törölni
  delete: (id: string): Promise<void> =>
    deleteRequest(`${USERS_URL}/${id}`, 'Nem sikerült a felhasználó törlése.'),

  getRoles: async (): Promise<Role[]> => {
    const response = await apiFetch(ROLES_URL);
    if (!response.ok) throw new Error('Nem sikerült a szerepkörök betöltése.');
    return response.json();
  },

  createRole: (role: RoleInput): Promise<Role> =>
    postJson(ROLES_URL, role, 'Nem sikerült a szerepkör mentése.'),

  updateRole: (id: string, role: RoleInput): Promise<Role> =>
    putJson(`${ROLES_URL}/${id}`, role, 'Nem sikerült a szerepkör módosítása.'),

  // 409: rendszer-szerepkör, vagy még tartoznak hozzá felhasználók
  deleteRole: (id: string): Promise<void> =>
    deleteRequest(`${ROLES_URL}/${id}`, 'Nem sikerült a szerepkör törlése.'),

  // A napi összefoglaló azonnali kiküldése az adminisztrátoroknak; a válasz a címzettek száma
  sendDailySummary: (): Promise<{ recipients: number }> =>
    postJson(`${NOTIFICATIONS_URL}/daily-summary`, null, 'Nem sikerült az összefoglaló kiküldése.'),
};

export default userService;
