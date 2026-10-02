import type { ReactNode } from 'react';

type ProfileButtonProps = {
  userName: string;
  role: string;
  avatarUrl?: string;
  onClick: () => void;
  children?: ReactNode;
};

export default function ProfileButton({
  userName,
  role,
  avatarUrl,
  onClick,
  children
}: ProfileButtonProps) {
  return (
    <button onClick={onClick} className="profile-btn">
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

      {children}
    </button>
  );
}