import React from "react";
import type { QuestionDto } from "../../../types/test";
import styles from "./QuestionRow.module.css";

interface QuestionRowProps {
  question: QuestionDto;
  onChangeQuestionText: (questionId: string, newText: string) => void;
  onChangeOptionText: (
    questionId: string,
    optionId: string,
    newText: string
  ) => void;
}

const QuestionRow: React.FC<QuestionRowProps> = ({
  question,
  onChangeQuestionText,
  onChangeOptionText,
}) => {
  return (
    <div className={styles.question}>
      <input
        className={styles.questionInput}
        value={question.text}
        onChange={(e) => onChangeQuestionText(question.id, e.target.value)}
      />

      <ul>
        {question.options.map((o) => (
          <li key={o.id}>
            <input
              className={styles.optionInput}
              value={o.text}
              onChange={(e) =>
                onChangeOptionText(question.id, o.id, e.target.value)
              }
            />
          </li>
        ))}
      </ul>
    </div>
  );
};

export default QuestionRow;
