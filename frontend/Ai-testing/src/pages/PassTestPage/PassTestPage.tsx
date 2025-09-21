import { useParams } from "react-router-dom";
import styles from "./PassTestPage.module.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useEffect, useState } from "react";
import type { TestPreviewDto } from "../../types/test";
import { getTestPreview } from "../../api/testService";

const PassTestPage = () => {
  const { id } = useParams<{ id: string }>();
  const [test, setTest] = useState<TestPreviewDto | null>(null);
  const [testFounded, setTestFounded] = useState<boolean | null>(null);

  useEffect(() => {
    const fetchTest = async () => {
      if (!id) return;
      try {
        const data = await getTestPreview(id);
        setTest(data);

        if (data !== null) setTestFounded(true);
      } catch (error) {
        console.error(error);
        setTestFounded(false);
      }
    };
    fetchTest();
  }, [id]);

  if (testFounded === null)
    return <div className={styles.loader}>Loading...</div>;

  return (
    <div className={styles.passTestPage}>
      <Header />
      <div className={styles.wrapper}>
        <div className={styles.container}>
          {testFounded && test ? (
            <>
              <div
                className={styles.header}
                style={{
                  backgroundImage: test.coverImageUrl
                    ? `url(${test.coverImageUrl})`
                    : "linear-gradient(135deg, #1e3c72, #2a5298)",
                }}
              >
                <div className={styles.overlay}>
                  <h1>{test.title}</h1>
                  {test.description && <p>{test.description}</p>}
                </div>

                <div className={styles.overlayFooter}>
                  <button className={styles.editButton}>Start test</button>
                </div>
              </div>
              <div className={styles.stats}>
                <p>
                  Time limit:{" "}
                  {test.timeLimitMinutes
                    ? test.timeLimitMinutes
                    : "No time limit"}
                </p>
              </div>
            </>
          ) : (
            <div className={styles.notFoundCard}>
              <h2>Test not available</h2>
              <p>
                This test does not exist or is private. Please check the link or
                contact the author.
              </p>
            </div>
          )}
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default PassTestPage;
