import React from "react";
import styles from "./Footer.module.css";
import { FaGithub } from "react-icons/fa";

const Footer: React.FC = () => {
  return (
    <footer className={styles.footer}>
      <div className={styles.footerColumns}>
        <div className={styles.footerColumn}>
          <h4>Your Account</h4>
          <ul>
            <li>
              <a href="/auth?mode=login">Log in</a>
            </li>
            <li>
              <a href="/auth?mode=signup">Sign up</a>
            </li>
          </ul>
        </div>

        <div className={styles.footerColumn}>
          <h4>Sections</h4>
          <ul>
            <li>
              <a href="/tests">Tests</a>
            </li>
            <li>
              <a href="/create">Create a Test</a>
            </li>
            <li>
              <a href="/attempt">Attempt by ID</a>
            </li>
          </ul>
        </div>

        <div className={styles.footerColumn}>
          <h4>Information</h4>
          <ul>
            <li>
              <a href="/about">About</a>
            </li>
            <li>
              <a href="/docs">Documentation</a>
            </li>
            <li>
              <a href="/privacy">Privacy Policy</a>
            </li>
          </ul>
        </div>

        <div className={styles.footerColumn}>
          <h4>Contact</h4>
          <ul>
            <li>
              <a
                href="https://github.com/QV1LL/Ai-testing"
                target="_blank"
                rel="noopener noreferrer"
                className={styles.socialLink}
              >
                <FaGithub size={20} /> View on GitHub
              </a>
            </li>
          </ul>
        </div>
      </div>

      <div className={styles.footerBottom}>
        <p translate="no">© {new Date().getFullYear()}, AI Testing</p>
      </div>
    </footer>
  );
};

export default Footer;
