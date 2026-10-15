import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { appPages, type PageKey } from '../util/pages.js';
import { useAuth } from './useAuth.js';
import { useI18n } from '../i18n/useI18n.js';

// Az oldal csak akkor jelenik meg, ha a felhasználó valamelyik szerepköre hozzáfér
// (a backend ugyanezt ellenőrzi a végpontokon, ez csak a felület).
export default function RequirePage({ page, children }: { page: PageKey; children: ReactNode }) {
  const { hasPage } = useAuth();
  const { t } = useI18n();

  if (hasPage(page)) return children;

  return (
    <div className="flex-grow-1 p-4">
      <div className="alert alert-warning" role="alert">
        <i className="bi bi-shield-lock me-2" aria-hidden="true"></i>
        {t('auth.noAccess')}
      </div>
    </div>
  );
}

// A gyökér útvonal az első olyan oldalra visz, amelyhez van jogosultság
export function HomeRedirect() {
  const { hasPage } = useAuth();
  const { t } = useI18n();
  const first = appPages.find((p) => hasPage(p.key));

  if (!first) {
    return (
      <div className="flex-grow-1 p-4">
        <div className="alert alert-info" role="status">
          {t('auth.noPages')}
        </div>
      </div>
    );
  }

  return <Navigate to={first.path} replace />;
}
