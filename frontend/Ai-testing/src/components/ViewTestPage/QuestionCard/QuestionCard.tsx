import React from "react";
import { QuestionType, type QuestionDto } from "../../../types/test";
import styles from "./QuestionCard.module.css";

interface QuestionCardProps {
  question: QuestionDto;
  index: number;
}

const QuestionCard: React.FC<QuestionCardProps> = ({ question, index }) => {
  const correctAnswers = question.correctAnswers ?? [];
  const options = question.options ?? [];

  const sortedOptions = [...options].sort(
    (a, b) => (a.order ?? 0) - (b.order ?? 0)
  );

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

      {sortedOptions.length > 0 && (
        <ul className={styles.optionsList}>
          {sortedOptions.map((o) => {
            const isCorrect = correctAnswers.some((ca) => ca.id === o.id);

            return (
              <li key={o.id} className={isCorrect ? styles.correct : ""}>
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

      {question.type === QuestionType.OpenEnded &&
        question.correctTextAnswer && (
          <p className={styles.correctTextAnswer}>
            Correct answer:{" "}
            <span className={styles.correct}>{question.correctTextAnswer}</span>
          </p>
        )}
    </div>
  );
};

export default QuestionCard;
