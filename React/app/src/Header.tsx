import ProfileButton from './header_components/ProfileButton.js';
// CSS is loaded by the bundler; TypeScript has no declaration for this side-effect import.
// @ts-expect-error -- the bundler resolves CSS imports at runtime.
import './Header.css';

type HeaderProps = {
  appName?: string;
  logoUrl?: string;
};

export default function Header({
  appName = "IOMS - Order & Inventory",
  logoUrl = "/react.svg"
}: HeaderProps) {

  const handleProfileClick = () => {
    alert('Gomb megnyomva! (Profil beállítások)');
  };

  return (
    <header className="app-header">
      <div className="container-fluid app-header-container">
        
        {/* BAL OLDAL: Logó */}
        <div className="d-flex align-items-center gap-2">
          <img
            src={logoUrl}
            alt="App Logo"
            width="36"
            height="36"
          />
        </div>

        {/* KÖZÉP: Alkalmazás neve */}
        <div className="text-center">
          <h1 className="app-header-title">
            {appName}
          </h1>
        </div>

        {/* JOBB OLDAL: Jelenlegi felhasználó */}
        <div>
          <ProfileButton
            userName="Kovács János"
            role="Raktárvezető"
            onClick={handleProfileClick}
          />
        </div>

      </div>
    </header>
  );
}