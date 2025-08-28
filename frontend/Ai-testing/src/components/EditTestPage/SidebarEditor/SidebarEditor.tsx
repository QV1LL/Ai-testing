import React, { useState, useEffect } from "react";
import {
  type EditableQuestionDto,
  type AnswerOptionDto,
  QuestionType,
  QuestionState,
} from "../../../types/test";
import styles from "./SidebarEditor.module.css";
import DropZone from "../DropZone/DropZone";

interface Props {
  questions: EditableQuestionDto[];
  setQuestions: (qs: EditableQuestionDto[]) => void;
  selectedQuestion: EditableQuestionDto | null;
  selectedOption: AnswerOptionDto | null;
  setSelectedQuestion: (q: EditableQuestionDto | null) => void;
  setSelectedOption: (o: AnswerOptionDto | null) => void;
}

const SidebarEditor: React.FC<Props> = ({
  questions,
  setQuestions,
  selectedQuestion,
  selectedOption,
  setSelectedQuestion,
  setSelectedOption,
}) => {
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
      if (window.innerWidth >= 768) setIsOpen(false);
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const markChanged = (q: EditableQuestionDto) =>
    q.state === QuestionState.Added ? q.state : QuestionState.Changed;

  const updateQuestion = (
    updater: (q: EditableQuestionDto) => EditableQuestionDto
  ) => {
    if (!selectedQuestion) return;
    const nextQ = updater(selectedQuestion);
    const next = questions.map((q) => (q.id === nextQ.id ? nextQ : q));
    setQuestions(next);
    setSelectedQuestion(nextQ);
  };

  const updateQuestionText = (text: string) => {
    updateQuestion((q) => ({ ...q, text, state: markChanged(q) }));
  };

  const updateOptionText = (text: string) => {
    if (!selectedQuestion || !selectedOption) return;
    const updatedOption = { ...selectedOption, text };
    const nextQ: EditableQuestionDto = {
      ...selectedQuestion,
      options: selectedQuestion.options.map((o) =>
        o.id === updatedOption.id ? updatedOption : o
      ),
      state: markChanged(selectedQuestion),
    };
    const next = questions.map((q) => (q.id === nextQ.id ? nextQ : q));
    setQuestions(next);
    setSelectedQuestion(nextQ);
    setSelectedOption(updatedOption);
  };

  const deleteSelectedQuestion = () => {
    if (!selectedQuestion) return;
    if (selectedQuestion.state === QuestionState.Added) {
      const left = questions.filter((q) => q.id !== selectedQuestion.id);
      setQuestions(left.map((q, i) => ({ ...q, order: i })));
    } else {
      const next = questions
        .map((q) =>
          q.id === selectedQuestion.id
            ? { ...q, state: QuestionState.Deleted }
            : q
        )
        .map((q, i) =>
          q.state === QuestionState.Deleted ? q : { ...q, order: i }
        );
      setQuestions(next);
    }
    setSelectedQuestion(null);
    setSelectedOption(null);
  };

  const deleteSelectedOption = () => {
    if (!selectedQuestion || !selectedOption) return;
    const nextQ: EditableQuestionDto = {
      ...selectedQuestion,
      options: selectedQuestion.options
        .filter((o) => o.id !== selectedOption.id)
        .map((o, i) => ({ ...o, order: i })),
      correctAnswers: (selectedQuestion.correctAnswers || []).filter(
        (o) => o.id !== selectedOption.id
      ),
      state: markChanged(selectedQuestion),
    };
    const next = questions.map((q) => (q.id === nextQ.id ? nextQ : q));
    setQuestions(next);
    setSelectedQuestion(nextQ);
    setSelectedOption(null);
  };

  const editorContent = (
    <div className={styles.container}>
      {selectedQuestion ? (
        <div className={styles.editorForm}>
          <div className={styles.questionPart}>
            <div className={styles.buttons}>
              <button
                type="button"
                onClick={deleteSelectedQuestion}
                className={styles.deleteBtn}
              >
                Delete Question
              </button>
              {selectedOption && (
                <button
                  type="button"
                  onClick={deleteSelectedOption}
                  className={styles.deleteBtn}
                >
                  Delete Option
                </button>
              )}
            </div>

            <label>
              Question type:
              <select
                value={selectedQuestion.type}
                onChange={(e) => {
                  const nextType = Number(e.target.value) as QuestionType;
                  const normalized =
                    nextType === QuestionType.OpenEnded
                      ? {
                          ...selectedQuestion,
                          type: nextType,
                          options: [],
                          correctOptions: [],
                          state: markChanged(selectedQuestion),
                        }
                      : {
                          ...selectedQuestion,
                          type: nextType,
                          correctTextAnswer: null,
                          state: markChanged(selectedQuestion),
                        };
                  const next = questions.map((q) =>
                    q.id === normalized.id ? normalized : q
                  );
                  setQuestions(next);
                  setSelectedQuestion(normalized);
                  if (nextType === QuestionType.OpenEnded)
                    setSelectedOption(null);
                }}
              >
                <option value={QuestionType.SingleChoice}>Single Choice</option>
                <option value={QuestionType.MultipleChoice}>
                  Multiple Choice
                </option>
                <option value={QuestionType.OpenEnded}>Open Ended</option>
              </select>
            </label>

            <label>
              Question text:
              <input
                type="text"
                value={selectedQuestion.text}
                onChange={(e) => updateQuestionText(e.target.value)}
              />
            </label>

            <DropZone
              preview={selectedQuestion.imageFile ?? null}
              onFileSelect={(file) => {
                updateQuestion((q) => ({
                  ...q,
                  imageFile: file,
                  state: markChanged(q),
                }));
              }}
            />
          </div>

          {selectedQuestion.type === QuestionType.OpenEnded ? (
            <div>
              <label>
                Correct answer:
                <input
                  type="text"
                  value={selectedQuestion.correctTextAnswer ?? ""}
                  onChange={(e) =>
                    updateQuestion((q) => ({
                      ...q,
                      correctTextAnswer: e.target.value,
                      state: markChanged(q),
                    }))
                  }
                />
              </label>
            </div>
          ) : selectedOption ? (
            <>
              <label>
                Option text:
                <input
                  type="text"
                  value={selectedOption.text}
                  onChange={(e) => updateOptionText(e.target.value)}
                />
              </label>
              <DropZone
                preview={selectedOption?.imageFile ?? null}
                onFileSelect={(file) => {
                  if (!selectedQuestion || !selectedOption) return;
                  const updatedOption = { ...selectedOption, imageFile: file };
                  const nextQ: EditableQuestionDto = {
                    ...selectedQuestion,
                    options: selectedQuestion.options.map((o) =>
                      o.id === updatedOption.id ? updatedOption : o
                    ),
                    state: markChanged(selectedQuestion),
                  };
                  const next = questions.map((q) =>
                    q.id === nextQ.id ? nextQ : q
                  );
                  setQuestions(next);
                  setSelectedQuestion(nextQ);
                  setSelectedOption(updatedOption);
                }}
              />
            </>
          ) : (
            <p>Select an option to edit or add new ones</p>
          )}
        </div>
      ) : (
        <p>Select a question to edit</p>
      )}
    </div>
  );

  if (isMobile) {
    return (
      <>
        <button
          className={`${styles.toggleButton} ${isOpen ? styles.open : ""}`}
          onClick={() => setIsOpen(!isOpen)}
        >
          ➤
        </button>
        <div
          className={`${styles.mobileSidebar} ${
            isOpen ? styles.mobileOpen : ""
          }`}
        >
          {editorContent}
        </div>
      </>
    );
  }

  return <div className={styles.sidebar}>{editorContent}</div>;
};

export default SidebarEditor;
