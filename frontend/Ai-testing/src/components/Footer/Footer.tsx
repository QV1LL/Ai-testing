import React, { useState } from "react";
import "./Footer.css";
import { FaGithub } from "react-icons/fa";
import AuthModal from "../AuthModal/AuthModal";

const Footer: React.FC = () => {
  const [showModal, setShowModal] = useState(false);
  const [defaultMode, setDefaultMode] = useState("login");

  return (
    <footer className="footer">
      {showModal && (
        <AuthModal
          defaultMode={defaultMode}
          isOpen={showModal}
          onClose={() => setShowModal(false)}
        />
      )}
      <div className="footer-columns">
        <div className="footer-column">
          <h4>Your Account</h4>
          <ul>
            <li>
              <a
                onClick={() => {
                  setDefaultMode("login");
                  setShowModal(true);
                }}
              >
                Log in
              </a>
            </li>
            <li>
              <a
                onClick={() => {
                  setDefaultMode("signup");
                  setShowModal(true);
                }}
              >
                Sign up
              </a>
            </li>
          </ul>
        </div>

        <div className="footer-column">
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

        <div className="footer-column">
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

        <div className="footer-column">
          <h4>Contact</h4>
          <ul>
            <li>
              <a
                href="https://github.com/QV1LL/Ai-testing"
                target="_blank"
                rel="noopener noreferrer"
                className="social-link"
              >
                <FaGithub size={20} /> View on GitHub
              </a>
            </li>
          </ul>
        </div>
      </div>

      <div className="footer-bottom">
        <p translate="no">© {new Date().getFullYear()}, AI Testing</p>
      </div>
    </footer>
  );
};

export default Footer;
