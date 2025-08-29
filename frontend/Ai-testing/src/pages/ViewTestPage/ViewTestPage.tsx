import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { deleteTest, getById } from "../../api/testService";
import styles from "./ViewTestPage.module.css";
import type { FullTestDto } from "../../types/test";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import QuestionCard from "../../components/ViewTestPage/QuestionCard/QuestionCard";

const ViewTestPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [test, setTest] = useState<FullTestDto | null>(null);
  const [tab, setTab] = useState<"questions" | "stats">("questions");

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
    <div className={styles.viewTestPage}>
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
              <div className={styles.overlayFooter}>
                <button
                  className={styles.editButton}
                  onClick={() => navigate(`/tests/edit/${test.id}`)}
                >
                  Edit Test
                </button>
                <button
                  className={styles.deleteButton}
                  onClick={async () => {
                    await deleteTest(test.id);
                    navigate(`/tests`);
                  }}
                >
                  Delete Test
                </button>
              </div>
            </div>
          </div>

          {/* Tabs */}
          <div className={styles.tabs}>
            <button
              onClick={() => setTab("questions")}
              className={tab === "questions" ? styles.active : ""}
            >
              Questions
            </button>
            <button
              onClick={() => setTab("stats")}
              className={tab === "stats" ? styles.active : ""}
            >
              Statistics
            </button>
          </div>

          {/* Content */}
          <div className={styles.content}>
            {tab === "questions" ? (
              sortedQuestions.length > 0 ? (
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
              )
            ) : (
              <div className={styles.stats}>
                <p>Total attempts: {test.attemptsCount}</p>
                <p>Average score: {test.averageScore.toFixed(2)}%</p>
              </div>
            )}
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default ViewTestPage;
