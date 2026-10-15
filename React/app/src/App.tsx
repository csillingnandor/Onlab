import { BrowserRouter } from 'react-router-dom';
import MainContent from './MainContent.js';
import Header from './Header.js';
import Footer from './Footer.js';
import AuthProvider from './auth/AuthProvider.js';
import ThemeProvider from './theme/ThemeProvider.js';
import I18nProvider from './i18n/I18nProvider.js';
import LoginPage from './auth/LoginPage.js';
import { useAuth } from './auth/useAuth.js';

// Bejelentkezés nélkül csak a bejelentkező oldal látszik (az útvonal megmarad, belépés után oda jutunk)
function AppShell() {
  const { user } = useAuth();

  if (!user) return <LoginPage />;

  return (
    <>
      <Header />
      <MainContent />
      <Footer />
    </>
  );
}

export default function App() {
  return (
    <ThemeProvider>
      <I18nProvider>
        <BrowserRouter>
          <AuthProvider>
            <AppShell />
          </AuthProvider>
        </BrowserRouter>
      </I18nProvider>
    </ThemeProvider>
  );
}
