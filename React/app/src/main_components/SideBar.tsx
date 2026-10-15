import { useMemo, useState } from 'react';
import './SideBar.css';
import { NavLink, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/useAuth.js';
import { navSections } from '../util/pages.js';
import { useI18n } from '../i18n/useI18n.js';

export default function SideBar() {
  const { pathname } = useLocation();
  const { hasPage } = useAuth();
  const { t } = useI18n();

  // Csak a jogosultsággal elérhető oldalak, az üresen maradó szekciók nélkül
  const sections = useMemo(
    () => navSections
      .map((section) => ({ ...section, pages: section.pages.filter((page) => hasPage(page.key)) }))
      .filter((section) => section.pages.length > 0),
    [hasPage],
  );

  // Induláskor az aktuális oldalt tartalmazó szekció legyen nyitva
  const [openSections, setOpenSections] = useState<Set<string>>(() => {
    const active = sections.find((s) => s.pages.some((page) => pathname.startsWith(page.path)));
    return new Set(active ? [active.id] : []);
  });

  const toggleSection = (id: string) => {
    setOpenSections((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  return (
    <aside className="app-sidebar">
      <nav>
        {sections.map((section) => {
          const isOpen = openSections.has(section.id);
          const hasActiveItem = section.pages.some((page) => pathname.startsWith(page.path));

          return (
            <div key={section.id} className={`sidebar-section ${isOpen ? 'open' : ''}`}>
              <button
                type="button"
                className={`sidebar-section-toggle ${hasActiveItem ? 'has-active' : ''}`}
                onClick={() => toggleSection(section.id)}
                aria-expanded={isOpen}
                aria-controls={`sidebar-section-${section.id}`}
              >
                <span className="sidebar-nav-item-content">
                  <i className={`bi ${section.icon} sidebar-nav-icon`}></i>
                  <span>{t(section.title)}</span>
                </span>
                <i className="bi bi-chevron-down sidebar-chevron"></i>
              </button>

              <div id={`sidebar-section-${section.id}`} className="sidebar-collapse">
                <ul className="sidebar-nav-list">
                  {section.pages.map((page) => (
                    <li key={page.key}>
                      <NavLink
                        to={page.path}
                        tabIndex={isOpen ? 0 : -1}
                        className={({ isActive }) => `sidebar-nav-button ${isActive ? 'active' : ''}`}
                      >
                        <span className="sidebar-nav-item-content">
                          <i className={`bi ${page.icon} sidebar-nav-icon`}></i>
                          <span>{t(page.label)}</span>
                        </span>
                      </NavLink>
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          );
        })}
      </nav>
    </aside>
  );
}
