import ActionButtons from '../../common/button/ActionButtons.js';
import type { Role } from '../../util/Types.js';
import { appPages, pageActions, type PageKey } from '../../util/pages.js';
import { useI18n } from '../../i18n/useI18n.js';
import '../../common/table/DataTable.css';

type RoleTableProps = {
  roles: Role[];
  onEdit: (role: Role) => void;
  onDelete: (role: Role) => void;
};

// Oldalkulcs -> a felirat fordítási kulcsa
const pageLabelKeys = new Map(appPages.map((page) => [page.key, page.label]));

export default function RoleTable({ roles, onEdit, onDelete }: RoleTableProps) {
  const { t } = useI18n();
  const pageLabel = (page: PageKey) => {
    const key = pageLabelKeys.get(page);
    return key ? t(key) : page;
  };

  return (
    <div className="data-table-wrapper">
      <table className="data-table">
        <thead>
          <tr>
            <th>Név</th>
            <th title="Az oldal mellett: engedélyezett / összes művelet">Elérhető oldalak (műveletek)</th>
            <th className="numeric">Felhasználók</th>
            <th className="numeric">Műveletek</th>
          </tr>
        </thead>
        <tbody>
          {roles.map((role) => (
            <tr key={role.id}>
              <td className="data-table-strong">{role.name}</td>
              <td>
                {role.pages.length > 0
                  ? role.pages.map((page) => {
                      // Oldalanként az engedélyezett műveletek száma; a súgóban a nevük
                      const available = pageActions[page] ?? [];
                      const granted = available.filter((a) => role.actions.includes(a.key));
                      return (
                        <span
                          key={page}
                          className="badge bg-secondary me-1"
                          title={available.length > 0
                            ? (granted.length > 0 ? granted.map((a) => t(a.label)).join(', ') : 'Csak megtekintés')
                            : undefined}
                        >
                          {pageLabel(page)}
                          {available.length > 0 && (
                            <span className="ms-1 opacity-75">{granted.length}/{available.length}</span>
                          )}
                        </span>
                      );
                    })
                  : <span className="text-muted small">Egy oldal sem</span>}
              </td>
              <td className="numeric">{role.userCount} fő</td>
              <td className="numeric">
                {role.isSystem ? (
                  <span className="text-muted small" title="A rendszer-szerepkör mindig minden oldalt és műveletet elér, nem módosítható.">
                    <i className="bi bi-lock me-1" aria-hidden="true"></i>
                    Rendszer
                  </span>
                ) : (
                  <ActionButtons
                    itemLabel={role.name}
                    onEdit={() => onEdit(role)}
                    onDelete={() => onDelete(role)}
                  />
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
