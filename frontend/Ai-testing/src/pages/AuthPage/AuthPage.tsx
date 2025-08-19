import React, { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./AuthPage.module.css";
import * as authService from "../../api/authService";
import type { LoginDto, RegisterDto } from "../../types/user";

const AuthPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const [isSignUp, setIsSignUp] = useState(false);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    const mode = searchParams.get("mode");
    setIsSignUp(mode === "signup");
  }, [searchParams]);

  const resetForm = () => {
    setName("");
    setEmail("");
    setPassword("");
    setError("");
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      if (isSignUp) {
        const data: RegisterDto = { name, email, password };
        await authService.register(data);
      } else {
        const data: LoginDto = { email, password };
        await authService.login(data);
      }

      navigate("/profile");
      resetForm();
    } catch (err: any) {
      setError(err.response?.data?.message || "Something went wrong");
    }
  };

  return (
    <div className={styles.authPage}>
      <Header />
      <div className={styles.pageWrapper}>
        <div className={styles.formContainer}>
          <form onSubmit={handleSubmit} className={styles.authForm}>
            {isSignUp && (
              <div className={styles.formGroup}>
                <label htmlFor="name">Name</label>
                <input
                  id="name"
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Enter your name"
                  required
                  className={error ? styles.errorInput : ""}
                  onFocus={() => setError("")}
                />
              </div>
            )}

            <div className={styles.formGroup}>
              <label htmlFor="email">Email</label>
              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter your email"
                required
                className={error ? styles.errorInput : ""}
                onFocus={() => setError("")}
              />
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="password">Password</label>
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Enter your password"
                required
                className={error ? styles.errorInput : ""}
                onFocus={() => setError("")}
              />
            </div>

            <button type="submit" className={styles.primaryBtn}>
              {isSignUp ? "Sign Up" : "Login"}
            </button>

            {error && <p className={styles.errorText}>{error}</p>}

            <p className={styles.toggleText}>
              {isSignUp ? "Already have an account?" : "Don't have an account?"}{" "}
              <span
                className={styles.toggleLink}
                onClick={() => {
                  setIsSignUp(!isSignUp);
                  resetForm();
                }}
              >
                {isSignUp ? "Login" : "Sign Up"}
              </span>
            </p>
          </form>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default AuthPage;
