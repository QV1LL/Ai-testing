import React, { useState } from "react";
import styles from "./Header.module.css";

interface HeaderProps {
  avatarUrl?: string;
}

const Header: React.FC<HeaderProps> = ({ avatarUrl }) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className={styles.wrapper}>
      <header className={styles.header}>
        <div className={styles.logo} translate="no">
          <img className={styles.iconImage} src="/icon.png" />
          <span>AI Testing</span>
        </div>

        <nav className={`${styles.navLinks} ${isOpen ? styles.open : ""}`}>
          <a href="/">Home</a>
          <a href="/about">About</a>
          <a href="/tests">My Tests</a>
          <a href="/profile">
            {avatarUrl && (
              <img src={avatarUrl} alt="Avatar" className="avatar" />
            )}
            Personal Cabinet
          </a>
        </nav>

        <button
          className={`${styles.menuToggle} ${isOpen ? styles.open : ""}`}
          onClick={() => setIsOpen(!isOpen)}
          aria-label="Toggle menu"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>
      </header>
    </div>
  );
};

export default Header;
