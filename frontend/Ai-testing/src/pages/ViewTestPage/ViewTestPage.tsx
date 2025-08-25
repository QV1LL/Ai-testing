import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { deleteTest, getById } from "../../api/testService";
import styles from "./ViewTestPage.module.css";
import type { FullTestDto } from "../../types/test";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import {
  FixedSizeList as List,
  type ListChildComponentProps,
} from "react-window";

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
        console.log(error);
      }
    };
    fetchTest();
  }, [id]);

  if (!test) return <div className={styles.loader}>Loading...</div>;

  const QuestionRow = ({ index, style }: ListChildComponentProps) => {
    const q = test.questions[index];
    return (
      <div style={style} className={styles.question}>
        <h3>
          {index + 1}. {q.text}
        </h3>
        <ul>
          {q.options.map((o) => (
            <li
              key={o.id}
              className={
                q.correctAnswers.some((ca) => ca.id === o.id)
                  ? styles.correct
                  : ""
              }
            >
              {o.text}
            </li>
          ))}
        </ul>
      </div>
    );
  };

  return (
    <div className={styles.viewTestPage}>
      <Header />
      <div className={styles.wrapper}>
        <div className={styles.container}>
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

          <div className={styles.content}>
            {tab === "questions" ? (
              test.questions.length > 0 ? (
                <List
                  height={500}
                  itemCount={test.questions.length}
                  itemSize={120}
                  width="100%"
                >
                  {QuestionRow}
                </List>
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
