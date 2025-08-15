import React from "react";
import styles from "./AboutPage.module.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";

const AboutPage: React.FC = () => {
  return (
    <div>
      <Header />
      <div className={styles.pageContainer}>
        <div className={styles.contentContainer}>
          <h2>About AI Testing</h2>
          <p>
            AI Testing is an innovative platform that helps educators and
            professionals create AI-powered tests quickly and accurately.
          </p>
          <p>
            Our mission is to make assessments faster, fairer, and more
            insightful, using AI to provide instant analytics and insights for
            better evaluation.
          </p>
          <h2>Developers</h2>
          <p>
            Developed by QV1LL, GitHub:{" "}
            <a href="https://github.com/QV1LL">https://github.com/QV1LL</a>
          </p>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default AboutPage;
