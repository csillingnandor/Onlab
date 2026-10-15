import { useI18n } from '../i18n/useI18n.js';
import { useTheme } from '../theme/useTheme.js';
import '../common/button/HeaderButton.css';

// Ikongomb a fejlécben: a másik téma ikonját mutatja (sötétben nap, világosban hold)
export default function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();
  const { t } = useI18n();
  const label = theme === 'dark' ? t('header.themeToLight') : t('header.themeToDark');

  return (
    <button type="button" className="header-btn" onClick={toggleTheme} aria-label={label} title={label}>
      <i className={`bi ${theme === 'dark' ? 'bi-sun' : 'bi-moon'}`} aria-hidden="true"></i>
    </button>
  );
}
