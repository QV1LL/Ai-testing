import React, { useState } from "react";
import styles from "./AuthModal.module.css";

interface AuthModalProps {
  isOpen: boolean;
  defaultMode: string;
  onClose: () => void;
}

const AuthModal: React.FC<AuthModalProps> = ({
  isOpen,
  onClose,
  defaultMode = "signup",
}) => {
  const [isSignUp, setIsSignUp] = useState(defaultMode === "signup");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  if (!isOpen) return null;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (isSignUp) {
      console.log("Sign Up:", { name, email, password });
    } else {
      console.log("Login:", { email, password });
    }
    setName("");
    setEmail("");
    setPassword("");
    onClose(); // close modal after submission
  };

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <button className={styles.closeBtn} onClick={onClose}>
          &times;
        </button>
        <h2 className={styles.modalHeader}>{isSignUp ? "Sign Up" : "Login"}</h2>
        <form onSubmit={handleSubmit} className={styles.form}>
          {isSignUp && (
            <div className={styles.formGroup}>
              <label>Name</label>
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>
          )}
          <div className={styles.formGroup}>
            <label>Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className={styles.formGroup}>
            <label>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <button type="submit" className={styles.primaryBtn}>
            {isSignUp ? "Sign Up" : "Login"}
          </button>
          <p className={styles.toggleText}>
            {isSignUp ? "Already have an account?" : "Don't have an account?"}{" "}
            <span
              className={styles.toggleLink}
              onClick={() => setIsSignUp(!isSignUp)}
            >
              {isSignUp ? "Login" : "Sign Up"}
            </span>
          </p>
        </form>
      </div>
    </div>
  );
};

export default AuthModal;
