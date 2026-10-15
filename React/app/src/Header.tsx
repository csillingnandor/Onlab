import ProfileButton from './header_components/ProfileButton.js';
import ThemeToggle from './header_components/ThemeToggle.js';
import LanguageSelect from './header_components/LanguageSelect.js';
import { useAuth } from './auth/useAuth.js';
import { useI18n } from './i18n/useI18n.js';
import './Header.css';

type HeaderProps = {
  appName?: string;
  logoUrl?: string;
};

export default function Header({
  appName = "IOMS - Order & Inventory",
  logoUrl = "src/assets/3d-delivery-box-parcel.png"
}: HeaderProps) {
  const { user, logout } = useAuth();
  const { t } = useI18n();

  return (
    <header className="app-header">
      <div className="container-fluid app-header-container">

        {/* BAL OLDAL: Logó */}
        <div className="app-header-brand">
          <img
            src={logoUrl}
            alt={t('header.logoAlt')}
            width="36"
            height="36"
          />
        </div>

        {/* KÖZÉP: Alkalmazás neve */}
        <div>
          <h1 className="app-header-title">
            {appName}
          </h1>
        </div>

        {/* JOBB OLDAL: téma- és nyelvváltó, jelenlegi felhasználó */}
        <div className="app-header-actions">
          <ThemeToggle />
          <LanguageSelect />
          {user && (
            <ProfileButton
              userName={user.displayName}
              role={user.roles.join(', ') || t('header.noRole')}
              onLogout={logout}
            />
          )}
        </div>

      </div>
    </header>
  );
}
