import type { QuestionDto } from "../../../types/test";
import { QuestionEditor } from "../QuestionEditor/QuestionEditor";

interface QuestionListProps {
  questions: QuestionDto[];
  onChangeQuestionText: (id: string, newText: string) => void;
  onChangeOptionText: (
    questionId: string,
    optionId: string,
    newText: string
  ) => void;
  onSave: () => void;
}

export const QuestionList: React.FC<QuestionListProps> = ({
  questions,
  onChangeQuestionText,
  onChangeOptionText,
  onSave,
}) => (
  <div>
    {questions.map((q) => (
      <QuestionEditor
        key={q.id}
        question={q}
        onChangeQuestionText={onChangeQuestionText}
        onChangeOptionText={onChangeOptionText}
      />
    ))}
    <button onClick={onSave}>Save Questions</button>
  </div>
);
