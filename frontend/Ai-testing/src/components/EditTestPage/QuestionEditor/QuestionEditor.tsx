import type { QuestionDto } from "../../../types/test";

interface QuestionEditorProps {
  question: QuestionDto;
  onChangeQuestionText: (id: string, newText: string) => void;
  onChangeOptionText: (
    questionId: string,
    optionId: string,
    newText: string
  ) => void;
}

export const QuestionEditor: React.FC<QuestionEditorProps> = ({
  question,
  onChangeQuestionText,
  onChangeOptionText,
}) => (
  <div>
    <input
      type="text"
      value={question.text}
      onChange={(e) => onChangeQuestionText(question.id, e.target.value)}
    />
    <ul>
      {question.options.map((o) => (
        <li key={o.id}>
          <input
            type="text"
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
