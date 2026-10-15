import ActionButtons from '../../common/button/ActionButtons.js';
import type { User } from '../../util/Types.js';
import '../../common/table/DataTable.css';

type UserTableProps = {
  users: User[];
  currentUserId: string;
  onEdit: (user: User) => void;
  onDelete: (user: User) => void;
};

export default function UserTable({ users, currentUserId, onEdit, onDelete }: UserTableProps) {
  return (
    <div className="data-table-wrapper">
      <table className="data-table">
        <thead>
          <tr>
            <th>Felhasználónév</th>
            <th>Teljes név</th>
            <th>Szerepkörök</th>
            <th className="numeric">Műveletek</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td className="data-table-mono">
                {user.userName}
                {user.id === currentUserId && <span className="badge bg-info ms-2">Te</span>}
              </td>
              <td className="data-table-strong">{user.displayName}</td>
              <td>
                {user.roles.length > 0
                  ? user.roles.map((role) => <span key={role} className="badge bg-secondary me-1">{role}</span>)
                  : <span className="text-warning small">Nincs szerepköre</span>}
              </td>
              <td className="numeric">
                <ActionButtons
                  itemLabel={user.displayName}
                  onEdit={() => onEdit(user)}
                  onDelete={() => onDelete(user)}
                />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
