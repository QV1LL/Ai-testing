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
        console.log(data);
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
              <p className={styles.JoinTestId}>
                Test id: <u>{test.joinId}</u>
              </p>
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
                <div className={styles.kpiGrid}>
                  <div className={styles.kpiCard}>
                    <p className={styles.kpiLabel}>Total attempts</p>
                    <p className={styles.kpiValue}>{test.attemptsCount}</p>
                  </div>

                  <div className={styles.kpiCard}>
                    <p className={styles.kpiLabel}>Average score</p>
                    <p className={styles.kpiValue}>
                      {test.averageScore.toFixed(2)}%
                    </p>
                  </div>

                  <div className={styles.kpiCard}>
                    <p className={styles.kpiLabel}>Highest score</p>
                    <p className={styles.kpiValue}>
                      {test.testAttempts.length > 0
                        ? `${Math.max(
                            ...test.testAttempts.map((a) => a.score)
                          )}%`
                        : "—"}
                    </p>
                  </div>

                  <div className={styles.kpiCard}>
                    <p className={styles.kpiLabel}>Lowest score</p>
                    <p className={styles.kpiValue}>
                      {test.testAttempts.length > 0
                        ? `${Math.min(
                            ...test.testAttempts.map((a) => a.score)
                          )}%`
                        : "—"}
                    </p>
                  </div>
                </div>

                <div className={styles.tableWrapper}>
                  {/* Desktop table */}
                  <table className={styles.statsTable}>
                    <thead>
                      <tr>
                        <th>#</th>
                        <th>Username</th>
                        <th>Score</th>
                        <th>Started</th>
                        <th>Finished</th>
                        <th>Duration</th>
                      </tr>
                    </thead>
                    <tbody>
                      {test.testAttempts
                        .sort((a, b) => b.score - a.score)
                        .map((attempt, index) => (
                          <tr key={attempt.id}>
                            <td>{index + 1}</td>
                            <td>{attempt.userDisplayName}</td>
                            <td>{attempt.score}%</td>
                            <td>
                              {new Date(attempt.startedAt).toLocaleString()}
                            </td>
                            <td>
                              {attempt.finishedAt
                                ? new Date(attempt.finishedAt).toLocaleString()
                                : "—"}
                            </td>
                            <td>
                              {attempt.finishedAt && attempt.startedAt
                                ? Math.round(
                                    (new Date(attempt.finishedAt).getTime() -
                                      new Date(attempt.startedAt).getTime()) /
                                      1000 /
                                      60
                                  ) + " min"
                                : "—"}
                            </td>
                          </tr>
                        ))}
                    </tbody>
                  </table>

                  {/* Mobile cards */}
                  <div className={styles.statsCards}>
                    {test.testAttempts.map((attempt, index) => (
                      <div className={styles.statsCard} key={attempt.id}>
                        <p className={styles.label}>#</p>
                        <p className={styles.value}>{index + 1}</p>

                        <p className={styles.label}>Username</p>
                        <p className={styles.value}>
                          {attempt.userDisplayName}
                        </p>

                        <p className={styles.label}>Score</p>
                        <p className={styles.value}>{attempt.score}%</p>

                        <p className={styles.label}>Started</p>
                        <p className={styles.value}>
                          {new Date(attempt.startedAt).toLocaleString()}
                        </p>

                        <p className={styles.label}>Finished</p>
                        <p className={styles.value}>
                          {attempt.finishedAt
                            ? new Date(attempt.finishedAt).toLocaleString()
                            : "—"}
                        </p>

                        <p className={styles.label}>Duration</p>
                        <p className={styles.value}>
                          {attempt.finishedAt && attempt.startedAt
                            ? Math.round(
                                (new Date(attempt.finishedAt).getTime() -
                                  new Date(attempt.startedAt).getTime()) /
                                  1000 /
                                  60
                              ) + " min"
                            : "—"}
                        </p>
                      </div>
                    ))}
                  </div>
                </div>
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
