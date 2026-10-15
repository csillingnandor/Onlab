import { Dropdown } from 'react-bootstrap';
import { useI18n } from '../i18n/useI18n.js';
import { languageNames, type Language } from '../i18n/translations.js';
import '../common/button/HeaderButton.css';

const languages = Object.keys(languageNames) as Language[];

// Nyelvválasztó a fejlécben: a gombon az aktuális nyelv kódja, a menüben a nyelvek a saját nevükön
export default function LanguageSelect() {
  const { language, setLanguage, t } = useI18n();

  return (
    <Dropdown align="end">
      <Dropdown.Toggle
        as="button"
        type="button"
        className="header-btn"
        id="language-menu"
        aria-label={`${t('header.language')}: ${languageNames[language]}`}
        title={t('header.language')}
      >
        <i className="bi bi-translate" aria-hidden="true"></i>
        <span>{language.toUpperCase()}</span>
      </Dropdown.Toggle>

      <Dropdown.Menu>
        {languages.map((lang) => (
          <Dropdown.Item
            key={lang}
            as="button"
            lang={lang}
            active={lang === language}
            onClick={() => setLanguage(lang)}
          >
            {languageNames[lang]}
          </Dropdown.Item>
        ))}
      </Dropdown.Menu>
    </Dropdown>
  );
}
