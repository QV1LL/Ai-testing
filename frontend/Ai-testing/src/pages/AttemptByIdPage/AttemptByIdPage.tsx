import { useState } from "react";
import Footer from "../../components/Footer/Footer";
import Header from "../../components/Header/Header";
import styles from "./AttemptByIdPage.module.css";

const AttemptByIdPage: React.FC = () => {
  const [testId, setTestId] = useState<string>();

  const onJoin = () => {
    if (testId && testId.trim()) {
      window.location.href = `/pass-test/${testId}`;
    }
  };

  return (
    <>
      <Header />
      <div className={styles.pageContainer}>
        <div className={styles.contentContainer}>
          <h2>Join a Test</h2>
          <div className={styles.joinForm}>
            <input
              type="text"
              placeholder="Enter Test ID"
              value={testId}
              onChange={(e) => setTestId(e.target.value)}
            />
            <button className="primary-btn" onClick={onJoin}>
              Start
            </button>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
};

export default AttemptByIdPage;
