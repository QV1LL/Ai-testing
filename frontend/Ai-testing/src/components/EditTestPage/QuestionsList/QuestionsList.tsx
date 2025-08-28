import React from "react";
import { v4 as uuidv4 } from "uuid";
import {
  type EditableQuestionDto,
  type AnswerOptionDto,
  QuestionType,
  QuestionState,
} from "../../../types/test";
import styles from "./QuestionsList.module.css";
import {
  DragDropContext,
  Droppable,
  Draggable,
  type DropResult,
} from "@hello-pangea/dnd";

interface Props {
  questions: EditableQuestionDto[];
  setQuestions: (qs: EditableQuestionDto[]) => void;
  setSelectedQuestion: (q: EditableQuestionDto | null) => void;
  setSelectedOption: (o: AnswerOptionDto | null) => void;
}

const QuestionsList: React.FC<Props> = ({
  questions,
  setQuestions,
  setSelectedQuestion,
  setSelectedOption,
}) => {
  const visibleQuestions = [...questions]
    .filter((q) => q.state !== QuestionState.Deleted)
    .sort((a, b) => a.order - b.order);

  const addQuestion = () => {
    const newQuestion: EditableQuestionDto = {
      id: uuidv4(),
      text: "",
      imageFile: null,
      type: QuestionType.SingleChoice,
      order: visibleQuestions.length,
      options: [],
      correctAnswers: [],
      correctTextAnswer: null,
      state: QuestionState.Added,
    };
    setQuestions([...questions, newQuestion]);
    setSelectedQuestion(newQuestion);
    setSelectedOption(null);
  };

  const addOption = (q: EditableQuestionDto) => {
    if (q.type === QuestionType.OpenEnded) return;
    const newOption: AnswerOptionDto = {
      id: uuidv4(),
      text: "",
      imageFile: null,
      order: q.options.length,
    };
    const updatedQuestion: EditableQuestionDto = {
      ...q,
      options: [...q.options, newOption],
      state: q.state === QuestionState.Added ? q.state : QuestionState.Changed,
    };
    const next = questions.map((qq) => (qq.id === q.id ? updatedQuestion : qq));
    setQuestions(next);
    setSelectedQuestion(updatedQuestion);
    setSelectedOption(newOption);
  };

  const onDragEnd = (result: DropResult) => {
    if (!result.destination) return;
    const reordered = Array.from(visibleQuestions);
    const [moved] = reordered.splice(result.source.index, 1);
    reordered.splice(result.destination.index, 0, moved);
    const updatedOrders = new Map<string, number>();
    reordered.forEach((q, i) => updatedOrders.set(q.id, i));
    const next = questions.map((q) => {
      if (!updatedOrders.has(q.id)) return q;
      const newOrder = updatedOrders.get(q.id)!;
      if (q.order === newOrder) return q;
      return {
        ...q,
        order: newOrder,
        state:
          q.state === QuestionState.Added ? q.state : QuestionState.Changed,
      };
    });
    setQuestions(next);
  };

  return (
    <div className={styles.list}>
      <div className={styles.headerRow}>
        <h3>Questions</h3>
        <button
          type="button"
          className={styles.addQuestionBtn}
          onClick={addQuestion}
        >
          + Add question
        </button>
      </div>
      <DragDropContext onDragEnd={onDragEnd}>
        <Droppable droppableId="questions">
          {(provided) => (
            <div {...provided.droppableProps} ref={provided.innerRef}>
              {visibleQuestions.map((q, index) => (
                <Draggable key={q.id} draggableId={q.id} index={index}>
                  {(providedDraggable) => (
                    <div
                      ref={providedDraggable.innerRef}
                      {...providedDraggable.draggableProps}
                      {...providedDraggable.dragHandleProps}
                      className={styles.item}
                      onClick={() => {
                        setSelectedQuestion(q);
                        setSelectedOption(null);
                      }}
                    >
                      <div className={styles.title}>
                        {q.text?.trim() ? q.text : "Untitled Question"}
                        {q.state === QuestionState.Added && (
                          <span className={styles.badge}>new</span>
                        )}
                        {q.state === QuestionState.Changed && (
                          <span className={styles.badge}>edited</span>
                        )}
                      </div>
                      {q.type !== QuestionType.OpenEnded && (
                        <div className={styles.options}>
                          {q.options.map((o, i) => (
                            <div
                              key={o.id}
                              className={styles.optionRow}
                              onClick={(e) => {
                                e.stopPropagation();
                                setSelectedQuestion(q);
                                setSelectedOption(o);
                              }}
                              title="Edit in sidebar"
                            >
                              <span className={styles.optionIndex}>
                                {i + 1}.
                              </span>
                              <span className={styles.optionText}>
                                {o.text?.trim() ? o.text : "Untitled option"}
                              </span>
                            </div>
                          ))}
                          <input
                            type="text"
                            className={styles.addOptionInput}
                            placeholder="+ Add option"
                            onClick={(e) => {
                              e.stopPropagation();
                              addOption(q);
                            }}
                          />
                        </div>
                      )}
                    </div>
                  )}
                </Draggable>
              ))}
              {provided.placeholder}
            </div>
          )}
        </Droppable>
      </DragDropContext>
    </div>
  );
};

export default QuestionsList;
