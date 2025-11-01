import React, { useState, useEffect } from "react";
import {
  type EditableQuestionDto,
  type UpdateOptionDto,
  QuestionType,
  QuestionState,
} from "../../../types/test";
import styles from "./SidebarEditor.module.css";
import DropZone from "../DropZone/DropZone";

interface Props {
  questions: EditableQuestionDto[];
  setQuestions: (qs: EditableQuestionDto[]) => void;
  selectedQuestion: EditableQuestionDto | null;
  selectedOption: UpdateOptionDto | null;
  setSelectedQuestion: (q: EditableQuestionDto | null) => void;
  setSelectedOption: (o: UpdateOptionDto | null) => void;
  onUserSendPrompt: (prompt: string) => void;
}

const SidebarEditor: React.FC<Props> = ({
  questions,
  setQuestions,
  selectedQuestion,
  selectedOption,
  setSelectedQuestion,
  setSelectedOption,
  onUserSendPrompt,
}) => {
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [isOpen, setIsOpen] = useState(false);

  const [promptText, setPromptText] = useState<string>("");
  const [isPromptButtonDisabled, setIsPromptButtonDisabled] =
    useState<boolean>(false);

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
    const updatedOption: UpdateOptionDto = { ...selectedOption, text };
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

      const deletedOrder = selectedQuestion.order;
      setQuestions(
        left.map((q) =>
          q.order > deletedOrder ? { ...q, order: q.order - 1 } : q
        )
      );
    } else {
      const deletedOrder = selectedQuestion.order;
      const next = questions.map((q) =>
        q.id === selectedQuestion.id
          ? { ...q, state: QuestionState.Deleted }
          : q.order > deletedOrder
          ? { ...q, order: q.order - 1 }
          : q
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
      correctAnswers: selectedQuestion.correctAnswers.filter(
        (o) => o.id !== selectedOption.id
      ),
      state: markChanged(selectedQuestion),
    };
    const next = questions.map((q) => (q.id === nextQ.id ? nextQ : q));
    setQuestions(next);
    setSelectedQuestion(nextQ);
    setSelectedOption(null);
  };

  const toggleCorrectAnswer = (option: UpdateOptionDto) => {
    if (!selectedQuestion) return;

    let updatedCorrect: UpdateOptionDto[] = [];

    if (selectedQuestion.type === QuestionType.SingleChoice) {
      updatedCorrect = [option];
    } else if (selectedQuestion.type === QuestionType.MultipleChoice) {
      const exists = selectedQuestion.correctAnswers?.some(
        (ca) => ca.id === option.id
      );
      updatedCorrect = exists
        ? selectedQuestion.correctAnswers.filter((ca) => ca.id !== option.id)
        : [...(selectedQuestion.correctAnswers || []), option];
    }

    const nextQ: EditableQuestionDto = {
      ...selectedQuestion,
      correctAnswers: updatedCorrect,
      state: markChanged(selectedQuestion),
    };

    const next = questions.map((q) => (q.id === nextQ.id ? nextQ : q));
    setQuestions(next);
    setSelectedQuestion(nextQ);

    const stillExists = nextQ.options.find((o) => o.id === option.id);
    if (stillExists) {
      setSelectedOption(stillExists);
    }
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
                  const normalized: EditableQuestionDto =
                    nextType === QuestionType.OpenEnded
                      ? {
                          ...selectedQuestion,
                          type: nextType,
                          options: [],
                          correctAnswers: [],
                          correctTextAnswer: null,
                          state: markChanged(selectedQuestion),
                        }
                      : {
                          ...selectedQuestion,
                          type: nextType,
                          correctTextAnswer: null,
                          options: selectedQuestion.options || [],
                          correctAnswers: [],
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
              <textarea
                value={selectedQuestion.text}
                onChange={(e) => updateQuestionText(e.target.value)}
                className={styles.questionTextInput}
              />
            </label>

            <DropZone
              preview={selectedQuestion.imageUrl ?? null}
              onFileSelect={(file) => {
                updateQuestion((q) => ({
                  ...q,
                  imageFile: file,
                  imageUrl: URL.createObjectURL(file),
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
                      correctTextAnswer: e.target.value || null,
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

              {(selectedQuestion.type === QuestionType.SingleChoice ||
                selectedQuestion.type === QuestionType.MultipleChoice) && (
                <label className={styles.checkbox}>
                  Mark as correct:
                  <input
                    type="checkbox"
                    checked={selectedQuestion.correctAnswers?.some(
                      (ca) => ca.id === selectedOption.id
                    )}
                    onChange={() => toggleCorrectAnswer(selectedOption)}
                  />
                </label>
              )}

              <DropZone
                preview={selectedOption?.imageUrl ?? null}
                onFileSelect={(file) => {
                  if (!selectedQuestion || !selectedOption) return;
                  const updatedOption: UpdateOptionDto = {
                    ...selectedOption,
                    imageFile: file,
                    imageUrl: URL.createObjectURL(file),
                  };
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
      <div className={styles.promptForm}>
        <label>
          Prompt:
          <input
            type="text"
            value={promptText}
            onChange={(e) => {
              const newValue = e.target.value;
              setPromptText(newValue);
              setIsPromptButtonDisabled(newValue.trim() === "");
            }}
          />
        </label>
        <button
          disabled={isPromptButtonDisabled}
          onClick={() => {
            if (promptText.trim() !== "") {
              onUserSendPrompt(promptText);
              setPromptText("");
            }
          }}
        >
          Send prompt
        </button>
      </div>
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
