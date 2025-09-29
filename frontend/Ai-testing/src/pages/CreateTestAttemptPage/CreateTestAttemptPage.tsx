import { useEffect, useState } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import { getById } from "../../api/testService";
import styles from "./CreateTestAttemptPage.module.css";
import {
  QuestionType,
  type FullTestDto,
  type QuestionDto,
  type AnswerOptionDto,
} from "../../types/test";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { createAttempt } from "../../api/testAttemptService";
import type {
  AddTestAttemptDto,
  AttemptAnswerDto,
} from "../../types/testAttempt";
import { jwtDecode } from "jwt-decode";
import { getAccessToken } from "../../api/api";

interface AnswerState {
  selectedOptionIds?: string[];
  writtenAnswer?: string;
}

const CreateTestAttemptPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const location = useLocation();

  const [test, setTest] = useState<FullTestDto | null>(null);
  const [answers, setAnswers] = useState<{ [questionId: string]: AnswerState }>(
    {}
  );
  const [loading, setLoading] = useState(true);
  const [loggedUserId, setLoggedUserId] = useState<string | null>(null);
  const [guestName, setGuestName] = useState<string | null>(null);

  useEffect(() => {
    const token = getAccessToken();

    if (token) {
      try {
        const decoded: any = jwtDecode(token);

        setLoggedUserId(decoded.sub || null);
      } catch (error) {
        console.error("Error decoding token:", error);
      }
    } else {
      const queryParams = new URLSearchParams(location.search);
      const guestNameFromUrl = queryParams.get("guestName") || null;
      if (guestNameFromUrl) {
        setGuestName(guestNameFromUrl);
      }
    }

    const fetchTest = async () => {
      if (!id) return;
      try {
        const data = await getById(id);
        setTest(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };
    fetchTest();
  }, [id]);

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
        [qId]: {
          ...prevAnswer,
          selectedOptionIds,
        },
      };
    });
  };

  const handleText = (qId: string, text: string) => {
    setAnswers((prev) => {
      const prevAnswer = prev[qId] || {};
      return {
        ...prev,
        [qId]: {
          ...prevAnswer,
          writtenAnswer: text || undefined,
        },
      };
    });
  };

  const handleSubmit = async () => {
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

      const dto: AddTestAttemptDto = {
        testId: id,
        userId: loggedUserId || undefined,
        guestName: guestName === null ? undefined : guestName,
        startedAt: new Date(),
        answers: answerDtos,
      };

      await createAttempt(dto);
      console.log("Test attempt submitted successfully");
      navigate("/");
    } catch (error) {
      console.error("Failed to submit attempt:", error);
    }
  };

  if (loading) return <div className={styles.loader}>Loading...</div>;
  if (!test) return <div className={styles.loader}>Test not found</div>;

  const sortedQuestions = [...test.questions].sort(
    (a, b) => (a.order ?? 0) - (b.order ?? 0)
  );

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
            {!loggedUserId && !guestName ? (
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
                        alt={`Question ${idx + 1} image`}
                        className={styles.questionImage}
                      />
                    )}

                    {(q.type === QuestionType.SingleChoice ||
                      q.type === QuestionType.MultipleChoice) && (
                      <ul>
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
                                  alt={`Option ${opt.text} image`}
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
                    onClick={handleSubmit}
                  >
                    Submit Answers
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default CreateTestAttemptPage;
