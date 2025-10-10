import { useLocation, useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./ViewTestAttemptsResults.module.css";
import type { TestAttemptResultDto } from "../../types/testAttempt";

const ViewTestAttemptResults: React.FC = () => {
  const { state } = useLocation();
  const navigate = useNavigate();
  const result = state as TestAttemptResultDto | null;

  if (!result) {
    navigate("/");
    return null;
  }

  const durationMinutes = Math.round(
    (new Date(result.finishedAt).getTime() -
      new Date(result.startedAt).getTime()) /
      60000
  );

  return (
    <>
      <Header />
      <div className={styles.viewTestPage}>
        <div className={styles.wrapper}>
          <div className={styles.container}>
            <div
              className={styles.header}
              style={{
                backgroundImage:
                  "url('https://images.unsplash.com/photo-1557683304-673a23048d34?auto=format&fit=crop&w=1200&q=60')",
              }}
            >
              <div className={styles.overlay}>
                <h1>{result.testTitle}</h1>
                <p>Completed by {result.displayUsername}</p>
              </div>
            </div>

            <div className={styles.content}>
              <div className={styles.stats}>
                <div>
                  <h3>Score</h3>
                  <p className={styles.scoreValue}>
                    {result.score.toFixed(2)}%
                  </p>
                </div>
                <div>
                  <h4>Started</h4>
                  <p>{new Date(result.startedAt).toLocaleString()}</p>
                </div>
                <div>
                  <h4>Finished</h4>
                  <p>{new Date(result.finishedAt).toLocaleString()}</p>
                </div>
                <div>
                  <h4>Duration</h4>
                  <p>{durationMinutes} min</p>
                </div>
              </div>

              <div style={{ textAlign: "center", marginTop: "1.5rem" }}>
                <button
                  className={styles.deleteButton}
                  onClick={() => navigate("/")}
                >
                  Back to Tests
                </button>
              </div>
            </div>
          </div>
        </div>
        <Footer />
      </div>
    </>
  );
};

export default ViewTestAttemptResults;
