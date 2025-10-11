import styles from "./ProfileCardSection.module.css";

interface ProfileCardProps {
  name: string;
  email: string;
  avatarUrl: string;
  isLoaded: boolean;
  onEditButtonClick: () => void;
  onLogOutButtonClick: () => void;
  onDeleteButtonClick: () => void;
}

const ProfileCardSection: React.FC<ProfileCardProps> = ({
  name,
  email,
  avatarUrl,
  isLoaded,
  onEditButtonClick,
  onLogOutButtonClick,
  onDeleteButtonClick,
}) => {
  return (
    <div className={styles.wrapper}>
      <div className={styles.container}>
        <div className={styles.profileCard}>
          <div className={styles.avatarWrapper}>
            {avatarUrl !== "" ? (
              <img
                src={avatarUrl}
                alt="User Avatar"
                className={styles.avatar}
              />
            ) : (
              <div className={styles.avatarPlaceholder}>{name.charAt(0)}</div>
            )}
          </div>

          <div className={styles.userInfoWrapper}>
            <h1 className={styles.heading}>
              Welcome,{" "}
              <span translate="no" className={styles.noTranslate}>
                {name}
              </span>
            </h1>

            <p className={styles.email}>{email}</p>

            <div className={styles.actions}>
              <button
                className="primary-btn"
                onClick={() => onEditButtonClick()}
                disabled={!isLoaded}
              >
                Edit Profile
              </button>
              <button
                onClick={() => onLogOutButtonClick()}
                className={`${styles.secondaryBtn} secondary-btn`}
                disabled={!isLoaded}
              >
                Logout
              </button>
              <button
                onClick={() => onDeleteButtonClick()}
                className={`${styles.deleteBtn}`}
                disabled={!isLoaded}
              >
                Delete Profile
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfileCardSection;
