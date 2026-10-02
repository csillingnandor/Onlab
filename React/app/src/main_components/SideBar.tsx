import { useState } from 'react';
import './SideBar.css';
import { NavLink, useLocation } from 'react-router-dom';

type NavItem = {
  to: string;
  label: string;
  icon: string;
};

type NavSection = {
  id: string;
  title: string;
  icon: string;
  items: NavItem[];
};

const sections: NavSection[] = [
  {
    id: 'dashboard',
    title: 'Műszerfal',
    icon: 'bi-speedometer2',
    items: [{ to: '/dashboard', label: 'Fő Műszerfal & Riportok', icon: 'bi-graph-up' }],
  },
  {
    id: 'warehouse',
    title: 'Raktár',
    icon: 'bi-boxes',
    items: [
      { to: '/products', label: 'Termékek', icon: 'bi-box-seam' },
      { to: '/warehouse', label: 'Raktárak', icon: 'bi-building' },
    ],
  },
  {
    id: 'sales',
    title: 'Eladások',
    icon: 'bi-cart3',
    items: [{ to: '/customer-orders', label: 'Vevői rendelések', icon: 'bi-receipt' }],
  },
  {
    id: 'purchasing',
    title: 'Beszerzés',
    icon: 'bi-truck',
    items: [{ to: '/supplier-orders', label: 'Beszerzési rendelések', icon: 'bi-file-earmark-text' }],
  },
  {
    id: 'master-data',
    title: 'Törzsadatok',
    icon: 'bi-database',
    items: [{ to: '/customers', label: 'Vevők', icon: 'bi-people' }],
  },
];

export default function SideBar() {
  const { pathname } = useLocation();

  // Induláskor az aktuális oldalt tartalmazó szekció legyen nyitva
  const [openSections, setOpenSections] = useState<Set<string>>(() => {
    const active = sections.find((s) => s.items.some((item) => pathname.startsWith(item.to)));
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
          const hasActiveItem = section.items.some((item) => pathname.startsWith(item.to));

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
                  <span>{section.title}</span>
                </span>
                <i className="bi bi-chevron-down sidebar-chevron"></i>
              </button>

              <div id={`sidebar-section-${section.id}`} className="sidebar-collapse">
                <ul className="sidebar-nav-list">
                  {section.items.map((item) => (
                    <li key={item.to}>
                      <NavLink
                        to={item.to}
                        tabIndex={isOpen ? 0 : -1}
                        className={({ isActive }) => `sidebar-nav-button ${isActive ? 'active' : ''}`}
                      >
                        <span className="sidebar-nav-item-content">
                          <i className={`bi ${item.icon} sidebar-nav-icon`}></i>
                          <span>{item.label}</span>
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
