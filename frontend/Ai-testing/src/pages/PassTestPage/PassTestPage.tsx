import { useParams } from "react-router-dom";
import styles from "./PassTestPage.module.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useEffect, useState } from "react";
import type { FullTestDto } from "../../types/test";
import QuestionCard from "../../components/ViewTestPage/QuestionCard/QuestionCard";
import { getById } from "../../api/testService";

const PassTestPage = () => {
  const { id } = useParams<{ id: string }>();
  const [test, setTest] = useState<FullTestDto | null>(null);

  useEffect(() => {
    const fetchTest = async () => {
      if (!id) return;
      try {
        const data = await getById(id);
        setTest(data);
      } catch (error) {
        console.error(error);
      }
    };
    fetchTest();
  }, [id]);

  if (!test) return <div className={styles.loader}>Loading...</div>;

  const sortedQuestions = [...test.questions].sort(
    (a, b) => (a.order ?? 0) - (b.order ?? 0)
  );

  return (
    <div className={styles.passTestPage}>
      <Header />
      <div className={styles.wrapper}>
        <div className={styles.container}>
          {/* Test Header */}
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
          </div>

          {/* Questions */}
          <div className={styles.content}>
            {sortedQuestions.length > 0 ? (
              <div className={styles.questionsList}>
                {sortedQuestions.map((question, index) => (
                  <QuestionCard
                    key={question.id}
                    question={question}
                    index={index}
                  />
                ))}
              </div>
            ) : (
              <div className={styles.noQuestions}>
                <p>No questions in this test yet.</p>
              </div>
            )}
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default PassTestPage;
