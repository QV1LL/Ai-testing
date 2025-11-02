import React from "react";
import { QuestionType, type QuestionDto } from "../../../types/test";
import { type AttemptAnswerDto } from "../../../types/testAttempt";
import styles from "./QuestionCard.module.css";

interface QuestionCardProps {
  question: QuestionDto;
  index: number;
  mode?: "view" | "results";
  answers?: AttemptAnswerDto[];
}

const QuestionCard: React.FC<QuestionCardProps> = ({
  question,
  index,
  mode = "view",
  answers = [],
}) => {
  const correctAnswers = question.correctAnswers ?? [];
  const options = question.options ?? [];
  const sortedOptions = [...options].sort(
    (a, b) => (a.order ?? 0) - (b.order ?? 0)
  );

  const userAnswer = answers.find((a) => a.questionId === question.id);
  const userSelectedIds = userAnswer?.selectedOptions?.map((o) => o.id) ?? [];

  return (
    <div className={styles.questionCard}>
      {question.imageUrl && (
        <img
          src={question.imageUrl}
          alt={`Question ${index + 1}`}
          className={styles.questionImage}
        />
      )}

      <h3 className={styles.title}>
        {index + 1}. {question.text || "Untitled Question"}
      </h3>

      {/* Options */}
      {sortedOptions.length > 0 && (
        <ul className={styles.optionsList}>
          {sortedOptions.map((o) => {
            const isCorrect = correctAnswers.some((ca) => ca.id === o.id);
            const isSelected = userSelectedIds.includes(o.id);

            let liClass = "";
            if (mode === "view") {
              liClass = isCorrect ? styles.correct : "";
            } else if (mode === "results") {
              if (isCorrect && isSelected) liClass = styles.correct;
              else if (!isCorrect && isSelected) liClass = styles.incorrect;
              else if (isCorrect && !isSelected)
                liClass = styles.correctHighlight;
            }

            return (
              <li key={o.id} className={liClass}>
                {o.imageUrl && (
                  <img
                    src={o.imageUrl}
                    alt={o.text}
                    className={styles.optionImage}
                  />
                )}
                <span>{o.text || "Untitled Option"}</span>
              </li>
            );
          })}
        </ul>
      )}

      {/* Open-ended question */}
      {question.type === QuestionType.OpenEnded && (
        <div className={styles.openEndedBlock}>
          {mode === "results" ? (
            <>
              <p>
                <strong>Your answer:</strong> {userAnswer?.writtenAnswer || "—"}
              </p>
              <p>
                <strong>Correct answer:</strong>{" "}
                <span className={styles.correct}>
                  {question.correctTextAnswer || "—"}
                </span>
              </p>
            </>
          ) : (
            question.correctTextAnswer && (
              <p className={styles.correctTextAnswer}>
                Correct answer:{" "}
                <span className={styles.correct}>
                  {question.correctTextAnswer}
                </span>
              </p>
            )
          )}
        </div>
      )}
    </div>
  );
};

export default QuestionCard;
