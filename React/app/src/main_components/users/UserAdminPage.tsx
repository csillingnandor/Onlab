import { useCallback, useEffect, useState } from 'react';
import { Nav, Spinner } from 'react-bootstrap';
import ConfirmModal from '../../common/modal/ConfirmModal.js';
import userService from '../../services/userService.js';
import { useAuth } from '../../auth/useAuth.js';
import type { Role, User } from '../../util/Types.js';
import RoleFormModal from './RoleFormModal.js';
import RoleTable from './RoleTable.js';
import UserFormModal from './UserFormModal.js';
import UserTable from './UserTable.js';
import '../../common/button/HeaderButton.css';
import './UserAdminPage.css';

type Tab = 'users' | 'roles';

// Felhasználók és szerepkörök (csoportok) kezelése; a szerepkörök határozzák meg, ki melyik oldalt éri el.
export default function UserAdminPage() {
  const { user: currentUser } = useAuth();
  const [tab, setTab] = useState<Tab>('users');
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // undefined: zárva, null: új elem, egyébként a szerkesztett elem
  const [editingUser, setEditingUser] = useState<User | null | undefined>(undefined);
  const [deletingUser, setDeletingUser] = useState<User | null>(null);
  const [editingRole, setEditingRole] = useState<Role | null | undefined>(undefined);
  const [deletingRole, setDeletingRole] = useState<Role | null>(null);
  const [summaryBusy, setSummaryBusy] = useState(false);
  const [summaryMessage, setSummaryMessage] = useState<string | null>(null);

  // Kipróbáláshoz: az ütemezett napi összefoglaló azonnali kiküldése
  const handleSendSummary = async () => {
    setSummaryBusy(true);
    setSummaryMessage(null);
    try {
      const { recipients } = await userService.sendDailySummary();
      setSummaryMessage(recipients > 0
        ? `A napi összefoglaló kiküldve ${recipients} adminisztrátornak.`
        : 'Egy adminisztrátornak sincs megadva e-mail címe, ezért nem ment ki levél.');
    } catch (err: unknown) {
      setSummaryMessage(err instanceof Error ? err.message : 'Nem sikerült az összefoglaló kiküldése.');
    } finally {
      setSummaryBusy(false);
    }
  };

  // A két lista összefügg (szerepkör átnevezése, taglétszám), ezért módosítás után mindkettőt újratöltjük
  const reload = useCallback(async () => {
    const [loadedUsers, loadedRoles] = await Promise.all([userService.getAll(), userService.getRoles()]);
    setUsers(loadedUsers);
    setRoles(loadedRoles);
  }, []);

  useEffect(() => {
    let ignore = false; // StrictMode dupla futtatásánál a régi választ eldobjuk

    Promise.all([userService.getAll(), userService.getRoles()])
      .then(([loadedUsers, loadedRoles]) => {
        if (ignore) return;
        setUsers(loadedUsers);
        setRoles(loadedRoles);
      })
      .catch((err: unknown) => {
        if (!ignore) setError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.');
      })
      .finally(() => {
        if (!ignore) setLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, []);

  const handleSaved = async () => {
    setEditingUser(undefined);
    setEditingRole(undefined);
    await reload().catch((err: unknown) => setError(err instanceof Error ? err.message : 'Ismeretlen hiba történt.'));
  };

  // Ha a backend elutasítja (409), a ConfirmModal mutatja a hibaüzenetet
  const handleConfirmDeleteUser = async () => {
    if (!deletingUser) return;
    await userService.delete(deletingUser.id);
    setDeletingUser(null);
    await reload();
  };

  const handleConfirmDeleteRole = async () => {
    if (!deletingRole) return;
    await userService.deleteRole(deletingRole.id);
    setDeletingRole(null);
    await reload();
  };

  const renderContent = () => {
    if (loading) {
      return (
        <div className="text-center py-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Betöltés...</span>
          </Spinner>
        </div>
      );
    }

    if (error) return <div className="alert alert-danger" role="alert">{error}</div>;

    return tab === 'users' ? (
      <UserTable
        users={users}
        currentUserId={currentUser?.id ?? ''}
        onEdit={setEditingUser}
        onDelete={setDeletingUser}
      />
    ) : (
      <RoleTable roles={roles} onEdit={setEditingRole} onDelete={setDeletingRole} />
    );
  };

  return (
    <div className="user-admin-page">
      <header className="user-admin-header">
        <div>
          <h1>Felhasználók & jogosultságok</h1>
          <p className="user-admin-subtitle">
            {users.length} felhasználó, {roles.length} szerepkör · a módosítások a felhasználó következő bejelentkezésekor lépnek életbe
          </p>
        </div>
        <div className="d-flex align-items-center gap-2">
          <button
            type="button"
            className="header-btn"
            onClick={handleSendSummary}
            disabled={summaryBusy}
            title="A tegnapi nap összefoglalójának azonnali kiküldése az adminisztrátoroknak"
          >
            <i className="bi bi-envelope-paper" aria-hidden="true"></i>
            Napi összefoglaló küldése
          </button>
          <button
            type="button"
            className="header-btn header-btn-primary"
            onClick={() => (tab === 'users' ? setEditingUser(null) : setEditingRole(null))}
          >
            <i className="bi bi-plus-lg" aria-hidden="true"></i>
            {tab === 'users' ? 'Új felhasználó' : 'Új szerepkör'}
          </button>
        </div>
      </header>

      {summaryMessage && (
        <div className="alert alert-info alert-dismissible mb-0" role="status">
          {summaryMessage}
          <button type="button" className="btn-close" aria-label="Bezárás" onClick={() => setSummaryMessage(null)}></button>
        </div>
      )}

      <Nav variant="tabs" activeKey={tab} onSelect={(key) => setTab(key === 'roles' ? 'roles' : 'users')}>
        <Nav.Item>
          <Nav.Link eventKey="users">
            <i className="bi bi-people me-2" aria-hidden="true"></i>
            Felhasználók
          </Nav.Link>
        </Nav.Item>
        <Nav.Item>
          <Nav.Link eventKey="roles">
            <i className="bi bi-shield-check me-2" aria-hidden="true"></i>
            Szerepkörök
          </Nav.Link>
        </Nav.Item>
      </Nav>

      {renderContent()}

      <UserFormModal
        show={editingUser !== undefined}
        user={editingUser ?? null}
        roles={roles}
        onSaved={handleSaved}
        onHide={() => setEditingUser(undefined)}
      />
      <RoleFormModal
        show={editingRole !== undefined}
        role={editingRole ?? null}
        onSaved={handleSaved}
        onHide={() => setEditingRole(undefined)}
      />

      <ConfirmModal
        show={deletingUser !== null}
        title="Felhasználó törlése"
        confirmLabel="Törlés"
        confirmIcon="bi-trash"
        busyLabel="Törlés..."
        onConfirm={handleConfirmDeleteUser}
        onHide={() => setDeletingUser(null)}
      >
        {deletingUser && (
          <>
            <p className="mb-2">Biztosan törlöd ezt a felhasználót?</p>
            <p className="mb-2">
              <strong>{deletingUser.displayName}</strong>
              <span className="text-secondary font-monospace ms-2">{deletingUser.userName}</span>
            </p>
            <p className="mb-0 small text-secondary">A művelet nem vonható vissza.</p>
          </>
        )}
      </ConfirmModal>
      <ConfirmModal
        show={deletingRole !== null}
        title="Szerepkör törlése"
        confirmLabel="Törlés"
        confirmIcon="bi-trash"
        busyLabel="Törlés..."
        onConfirm={handleConfirmDeleteRole}
        onHide={() => setDeletingRole(null)}
      >
        {deletingRole && (
          <>
            <p className="mb-2">Biztosan törlöd ezt a szerepkört?</p>
            <p className="mb-2"><strong>{deletingRole.name}</strong></p>
            <p className="mb-0 small text-secondary">Csak olyan szerepkör törölhető, amelyhez nem tartozik felhasználó.</p>
          </>
        )}
      </ConfirmModal>
    </div>
  );
}
