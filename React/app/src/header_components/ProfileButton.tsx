import { Dropdown } from 'react-bootstrap';
import { useI18n } from '../i18n/useI18n.js';

type ProfileButtonProps = {
  userName: string;
  role: string;
  avatarUrl?: string;
  onLogout: () => void;
};

// A bejelentkezett felhasználó a fejléc jobb oldalán; kattintásra lenyíló menü a kijelentkezéssel
export default function ProfileButton({
  userName,
  role,
  avatarUrl,
  onLogout,
}: ProfileButtonProps) {
  const { t } = useI18n();

  return (
    <Dropdown align="end">
      <Dropdown.Toggle as="button" type="button" className="header-btn profile-btn" id="profile-menu">
        {avatarUrl ? (
          <img
            src={avatarUrl}
            alt={userName}
            className="profile-avatar-img"
          />
        ) : (
          <div className="profile-avatar-fallback">
            {userName.charAt(0).toUpperCase()}
          </div>
        )}

        <div className="profile-info">
          <div className="profile-name">{userName}</div>
          <small className="profile-role">{role}</small>
        </div>
      </Dropdown.Toggle>

      <Dropdown.Menu>
        <Dropdown.Item as="button" onClick={onLogout}>
          <i className="bi bi-box-arrow-right me-2" aria-hidden="true"></i>
          {t('header.logout')}
        </Dropdown.Item>
      </Dropdown.Menu>
    </Dropdown>
  );
}
