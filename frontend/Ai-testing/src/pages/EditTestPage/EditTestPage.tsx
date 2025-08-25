import { useEffect, useRef, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getById,
  updateTestData,
  updateTestQuestions,
} from "../../api/testService";
import styles from "./EditTestPage.module.css";
import {
  type FullTestDto,
  type QuestionDto,
  type UpdateTestMetadataDto,
  type UpdateQuestionsDto,
  QuestionType,
} from "../../types/test";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import QuestionRow from "../../components/EditTestPage/QuestionRow/QuestionRow";

const EditTestPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [test, setTest] = useState<FullTestDto | null>(null);
  const [changedQuestions, setChangedQuestions] = useState<QuestionDto[]>([]);

  const [preview, setPreview] = useState<string>("");
  const [coverImage, setCoverImage] = useState<File | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const fetchTest = async () => {
      if (!id) return;
      try {
        const data = await getById(id);
        setTest(data);
        setPreview(data.coverImageUrl === undefined ? "" : data.coverImageUrl);
      } catch (error) {
        console.log(error);
      }
    };
    fetchTest();
  }, [id]);

  if (!test) return <div className={styles.loader}>Loading...</div>;

  const handleFileChange = (file: File) => {
    setCoverImage(file);
    setPreview(URL.createObjectURL(file));
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFileChange(e.dataTransfer.files[0]);
    }
  };

  const markQuestionChanged = (questionId: string) => {
    const changedQ = test.questions.find((q) => q.id === questionId)!;
    setChangedQuestions((prev) => {
      const exists = prev.find((q) => q.id === questionId);
      if (exists) return prev.map((q) => (q.id === questionId ? changedQ : q));
      return [...prev, changedQ];
    });
  };

  const handleChangeQuestionText = (questionId: string, newText: string) => {
    setTest((prev) => {
      if (!prev) return prev;
      const updatedQuestions = prev.questions.map((q) =>
        q.id === questionId ? { ...q, text: newText } : q
      );
      markQuestionChanged(questionId);
      return { ...prev, questions: updatedQuestions };
    });
  };

  const handleChangeOptionText = (
    questionId: string,
    optionId: string,
    newText: string
  ) => {
    setTest((prev) => {
      if (!prev) return prev;
      const updatedQuestions = prev.questions.map((q) => {
        if (q.id !== questionId) return q;
        const updatedOptions = q.options.map((o) =>
          o.id === optionId ? { ...o, text: newText } : o
        );
        return { ...q, options: updatedOptions };
      });
      markQuestionChanged(questionId);
      return { ...prev, questions: updatedQuestions };
    });
  };

  const handleSaveTestData = async () => {
    if (!test) return;
    const dto: UpdateTestMetadataDto = {
      id: test.id,
      title: test.title,
      description: test.description,
      coverImage: coverImage,
    };
    await updateTestData(dto);
    alert("Test info saved");
  };

  const handleSaveQuestions = async () => {
    if (!test || changedQuestions.length === 0) return;
    const dto: UpdateQuestionsDto = {
      testId: test.id,
      questionsToUpdate: changedQuestions,
    };
    await updateTestQuestions(dto);
    setChangedQuestions([]);
    alert("Questions saved");
  };

  // 🆕 Додаємо нове питання
  const handleAddQuestion = () => {
    if (!test) return;
    const newQuestion: QuestionDto = {
      id: `temp-${Date.now()}`, // тимчасовий id
      text: "New question",
      options: [
        { id: `opt-${Date.now()}-1`, text: "Option 1", order: 0 },
        { id: `opt-${Date.now()}-2`, text: "Option 2", order: 1 },
      ],
      type: QuestionType.SingleChoice,
      order: 0,
      correctAnswers: [],
    };

    setTest((prev) =>
      prev ? { ...prev, questions: [...prev.questions, newQuestion] } : prev
    );
    setChangedQuestions((prev) => [...prev, newQuestion]);
  };

  return (
    <div className={styles.editTestPage}>
      <Header />
      <div className={styles.wrapper}>
        <div className={styles.container}>
          <div
            className={styles.header}
            style={{
              backgroundImage: test.coverImageUrl
                ? `url(${preview})`
                : "linear-gradient(135deg, #1e3c72, #2a5298)",
            }}
          >
            <div className={styles.overlay}>
              <input
                className={styles.titleInput}
                placeholder="No title"
                value={test.title}
                onChange={(e) => setTest({ ...test, title: e.target.value })}
              />
              <textarea
                className={styles.descriptionInput}
                placeholder="No description"
                value={test.description || ""}
                onChange={(e) =>
                  setTest({ ...test, description: e.target.value })
                }
              />
              <div
                id="dropzone"
                className={styles.dropZone}
                onDrop={handleDrop}
                onDragOver={(e) => e.preventDefault()}
                onClick={() => fileInputRef.current?.click()}
              >
                <p>Drag & drop an image here or click to upload</p>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  style={{ display: "none" }}
                  onChange={(e) => {
                    if (e.target.files && e.target.files[0]) {
                      handleFileChange(e.target.files[0]);
                    }
                  }}
                />
              </div>

              <div className={styles.overlayFooter}>
                <button
                  className={styles.saveButton}
                  onClick={handleSaveTestData}
                >
                  Save Test Info
                </button>
                <button
                  className={styles.saveButton}
                  onClick={handleSaveQuestions}
                >
                  Save Questions
                </button>
                <button
                  className={styles.cancelButton}
                  onClick={() => navigate(-1)}
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>

          <div className={styles.content}>
            {test.questions.map((q) => (
              <QuestionRow
                key={q.id}
                question={q}
                onChangeQuestionText={handleChangeQuestionText}
                onChangeOptionText={handleChangeOptionText}
              />
            ))}

            {/* 🆕 Кнопка для додавання */}
            <button className={styles.addButton} onClick={handleAddQuestion}>
              ➕ Add Question
            </button>
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default EditTestPage;
