import { useState, type FormEvent } from 'react';
import { Alert, Button, Form, Spinner } from 'react-bootstrap';
import { useAuth } from './useAuth.js';
import { useI18n } from '../i18n/useI18n.js';
import ThemeToggle from '../header_components/ThemeToggle.js';
import LanguageSelect from '../header_components/LanguageSelect.js';
import './LoginPage.css';

// Bejelentkezés nélkül csak ez az oldal látszik (App.tsx)
export default function LoginPage() {
  const { login } = useAuth();
  const { t } = useI18n();
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await login(userName.trim(), password);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : t('login.failed'));
      setPassword('');
      setBusy(false);
    }
  };

  return (
    <div className="login-page">
      {/* Téma- és nyelvváltó itt is, mert a fejléc csak belépés után látszik */}
      <div className="login-page-actions">
        <ThemeToggle />
        <LanguageSelect />
      </div>

      <Form className="login-card" onSubmit={handleSubmit}>
        <div className="login-brand">
          <img src="src/assets/3d-delivery-box-parcel.png" alt="" width="48" height="48" />
          <h1>IOMS</h1>
          <p>Order & Inventory</p>
        </div>

        {error && <Alert variant="danger">{error}</Alert>}

        <Form.Group controlId="login-username" className="mb-3">
          <Form.Label>{t('login.userName')}</Form.Label>
          <Form.Control
            type="text"
            autoComplete="username"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            required
            autoFocus
          />
        </Form.Group>

        <Form.Group controlId="login-password" className="mb-4">
          <Form.Label>{t('login.password')}</Form.Label>
          <Form.Control
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </Form.Group>

        <Button type="submit" variant="primary" className="w-100" disabled={busy}>
          {busy ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" aria-hidden="true" />
              {t('login.submitting')}
            </>
          ) : (
            <>
              <i className="bi bi-box-arrow-in-right me-2" aria-hidden="true"></i>
              {t('login.submit')}
            </>
          )}
        </Button>
      </Form>
    </div>
  );
}
