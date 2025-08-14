import React, { useState } from "react";
import "./Header.css";

interface HeaderProps {
  avatarUrl?: string;
}

const Header: React.FC<HeaderProps> = ({ avatarUrl }) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <header className="header">
      <div className="logo" translate="no">
        <img className="icon-image" src="/icon.png" />
        <span>AI Testing</span>
      </div>

      <nav className={`nav-links ${isOpen ? "open" : ""}`}>
        <a href="/">Home</a>
        <a href="/about">About</a>
        <a href="/tests">My Tests</a>
        <a href="/profile">
          {avatarUrl && <img src={avatarUrl} alt="Avatar" className="avatar" />}
          Personal Cabinet
        </a>
      </nav>

      <button
        className={`menu-toggle ${isOpen ? "open" : ""}`}
        onClick={() => setIsOpen(!isOpen)}
        aria-label="Toggle menu"
      >
        <span></span>
        <span></span>
        <span></span>
      </button>
    </header>
  );
};

export default Header;
