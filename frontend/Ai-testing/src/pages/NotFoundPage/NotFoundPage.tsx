import React from "react";
import { useNavigate } from "react-router-dom";
import styles from "./NotFoundPage.module.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";

const NotFoundPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div className={styles.wrapper}>
      <Header />
      <div className={styles.notFoundPage}>
        <div className={styles.content}>
          <h1 className={styles.NotFoundHeader}>404</h1>
          <h2 className={styles.NotFoundSecondHeader}>Page Not Found</h2>
          <p className={styles.NotFoundDesc}>
            Oops! The page you are looking for does not exist.
          </p>
          <button onClick={() => navigate("/")} className={styles.homeBtn}>
            Go to Home
          </button>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default NotFoundPage;
