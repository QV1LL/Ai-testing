import React from "react";
import styles from "./PrivacyPolicyPage.module.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";

const PrivacyPolicyPage: React.FC = () => {
  return (
    <div>
      <Header />
      <div className={styles.pageContainer}>
        <div className={styles.contentContainer}>
          <p>
            We collect only the data necessary to provide our services. Your
            information is never sold to third parties.
          </p>
          <h2>Data Collection</h2>
          <p>
            We may collect your name, email, and test data to improve your
            experience.
          </p>
          <h2>Data Usage</h2>
          <p>
            Your data is used solely for running tests and providing analytics.
            We do not share personal information without consent.
          </p>
          <h2>Your Rights</h2>
          <p>
            You can request to view or delete your data at any time by
            contacting support@aitesting.com.
          </p>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default PrivacyPolicyPage;
