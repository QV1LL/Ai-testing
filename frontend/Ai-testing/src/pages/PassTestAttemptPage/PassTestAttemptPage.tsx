import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getById } from "../../api/testService";
import styles from "./PassTestAttemptPage.module.css";
import {
  QuestionType,
  type FullTestDto,
  type QuestionDto,
  type AnswerOptionDto,
} from "../../types/test";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { finishAttempt, getAttemptById } from "../../api/testAttemptService";
import type {
  AttemptAnswerDto,
  FinishTestAttemptDto,
  TestAttemptMetadataDto,
} from "../../types/testAttempt";
import { jwtDecode } from "jwt-decode";
import { getAccessToken } from "../../api/api";

interface AnswerState {
  selectedOptionIds?: string[];
  writtenAnswer?: string;
}

const PassTestAttemptPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [testAttempt, setTestAttempt] = useState<TestAttemptMetadataDto | null>(
    null
  );
  const [test, setTest] = useState<FullTestDto | null>(null);
  const [answers, setAnswers] = useState<{ [questionId: string]: AnswerState }>(
    {}
  );
  const [loading, setLoading] = useState(true);
  const [loggedUserId, setLoggedUserId] = useState<string | null>(null);
  const [remainingTime, setRemainingTime] = useState<number | null>(null);

  useEffect(() => {
    const fetchTestAttempt = async () => {
      if (!id) return;
      try {
        const data = await getAttemptById(id);
        setTestAttempt(data);

        if (data === null) {
          console.log(data);
          return;
        }

        await fetchTest(data.testId);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    const fetchTest = async (testId: string) => {
      try {
        const data = await getById(testId);
        setTest(data);

        console.log(data);
        if (data.timeLimitMinutes) {
          setRemainingTime(data.timeLimitMinutes * 60);
        }
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    const fetchUserId = async () => {
      const token = getAccessToken();
      if (token) {
        try {
          const decoded: any = jwtDecode(token);
          setLoggedUserId(decoded.sub || null);
        } catch (error) {
          console.error("Error decoding token:", error);
        }
      }
    };

    fetchTestAttempt();
    fetchUserId();
  }, [id]);

  useEffect(() => {
    if (remainingTime === null || remainingTime <= 0) return;

    const timer = setInterval(() => {
      setRemainingTime((prev) => {
        if (prev === null) return null;
        if (prev <= 1) {
          clearInterval(timer);
          handleSubmit(true);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [remainingTime]);

  const handleSelect = (qId: string, optionId: string, multiple: boolean) => {
    setAnswers((prev) => {
      const prevAnswer = prev[qId] || {};
      let selectedOptionIds = prevAnswer.selectedOptionIds || [];

      if (multiple) {
        selectedOptionIds = selectedOptionIds.includes(optionId)
          ? selectedOptionIds.filter((id) => id !== optionId)
          : [...selectedOptionIds, optionId];
      } else {
        selectedOptionIds = [optionId];
      }

      return {
        ...prev,
        [qId]: { ...prevAnswer, selectedOptionIds },
      };
    });
  };

  const handleText = (qId: string, text: string) => {
    setAnswers((prev) => ({
      ...prev,
      [qId]: { ...prev[qId], writtenAnswer: text || undefined },
    }));
  };

  const handleSubmit = async (auto = false) => {
    if (!id || !test) return;
    try {
      const answerDtos: AttemptAnswerDto[] = Object.entries(answers).map(
        ([questionId, data]) => {
          const question = test.questions.find((q) => q.id === questionId);
          const selectedOptions: AnswerOptionDto[] = data.selectedOptionIds
            ? data.selectedOptionIds.map((optionId) => {
                const option = question?.options.find(
                  (opt) => opt.id === optionId
                );
                return option || { id: optionId, text: "", order: 0 };
              })
            : [];
          return {
            questionId,
            selectedOptions,
            writtenAnswer: data.writtenAnswer,
          };
        }
      );

      const dto: FinishTestAttemptDto = {
        attemptId: id,
        answers: answerDtos,
      };

      if (auto) {
        alert("⏰ Time’s up! Your answers are being submitted automatically.");
      }

      const result = await finishAttempt(dto);
      navigate("/test-attempt/result", { state: result });
    } catch (error) {
      console.error("Failed to submit attempt:", error);
    }
  };

  if (loading) return <div className={styles.loader}>Loading...</div>;
  if (!test) return <div className={styles.loader}>Test not found</div>;

  const sortedQuestions = [...test.questions].sort(
    (a, b) => (a.order ?? 0) - (b.order ?? 0)
  );

  const formatTime = (seconds: number) => {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, "0")}`;
  };

  return (
    <div className={styles.createTestAttemptPage}>
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
            </div>
          </div>

          <div className={styles.content}>
            {!loggedUserId && !testAttempt?.guestName ? (
              <div className={styles.noQuestions}>
                <h3>Access Denied</h3>
                <p>
                  You must be logged in or provide a guest name to take this
                  test.
                </p>
              </div>
            ) : (
              <>
                {sortedQuestions.map((q: QuestionDto, idx) => (
                  <div key={q.id} className={styles.question}>
                    <h3>
                      {idx + 1}. {q.text}
                    </h3>
                    {q.imageUrl && (
                      <img
                        src={q.imageUrl}
                        alt={`Question ${idx + 1}`}
                        className={styles.questionImage}
                      />
                    )}

                    {(q.type === QuestionType.SingleChoice ||
                      q.type === QuestionType.MultipleChoice) && (
                      <ul className={styles.optionsList}>
                        {q.options.map((opt) => (
                          <li key={opt.id} className={styles.optionItem}>
                            <label className={styles.optionLabel}>
                              <input
                                type={
                                  q.type === QuestionType.MultipleChoice
                                    ? "checkbox"
                                    : "radio"
                                }
                                name={q.id}
                                value={opt.id}
                                checked={
                                  q.type === QuestionType.MultipleChoice
                                    ? answers[
                                        q.id
                                      ]?.selectedOptionIds?.includes(opt.id) ||
                                      false
                                    : answers[q.id]?.selectedOptionIds?.[0] ===
                                      opt.id
                                }
                                onChange={() =>
                                  handleSelect(
                                    q.id,
                                    opt.id,
                                    q.type === QuestionType.MultipleChoice
                                  )
                                }
                              />
                              <span>{opt.text}</span>
                              {opt.imageUrl && (
                                <img
                                  src={opt.imageUrl}
                                  alt={`Option ${opt.text}`}
                                  className={styles.optionImage}
                                />
                              )}
                            </label>
                          </li>
                        ))}
                      </ul>
                    )}

                    {q.type === QuestionType.OpenEnded && (
                      <textarea
                        value={answers[q.id]?.writtenAnswer || ""}
                        onChange={(e) => handleText(q.id, e.target.value)}
                        placeholder="Write your answer..."
                      />
                    )}
                  </div>
                ))}

                <div style={{ textAlign: "center", marginTop: "2rem" }}>
                  <button
                    className={styles.submitButton}
                    onClick={() => handleSubmit()}
                  >
                    Submit Answers
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      </div>

      {remainingTime !== null && (
        <div className={styles.timerBar}>
          <span>⏰ Time left: {formatTime(remainingTime)}</span>
        </div>
      )}

      <Footer />
    </div>
  );
};

export default PassTestAttemptPage;
