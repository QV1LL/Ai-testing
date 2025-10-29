import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getById,
  getUpdateQuestionDtoFromPrompt,
  updateQuestionsAndOptionsImages,
  updateTestData,
  updateTestQuestions,
} from "../../api/testService";
import styles from "./EditTestPage.module.css";
import {
  type FullTestDto,
  type UpdateTestMetadataDto,
  type UpdateQuestionsDto,
  type UpdateOptionDto,
  type PromptQuestionsDto,
} from "../../types/test";
import { type EditableQuestionDto, QuestionState } from "../../types/test";

import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import TestHeader from "../../components/EditTestPage/TestHeader/TestHeader";
import QuestionsList from "../../components/EditTestPage/QuestionsList/QuestionsList";
import SidebarEditor from "../../components/EditTestPage/SidebarEditor/SidebarEditor";

const EditTestPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [test, setTest] = useState<FullTestDto | null>(null);
  const [questions, setQuestions] = useState<EditableQuestionDto[]>([]);
  const [selectedQuestion, setSelectedQuestion] =
    useState<EditableQuestionDto | null>(null);
  const [selectedOption, setSelectedOption] = useState<UpdateOptionDto | null>(
    null
  );
  const [preview, setPreview] = useState<string>("");

  const fetchTest = async () => {
    if (!id) return;
    try {
      const data = await getById(id);

      setQuestions(
        data.questions.map((q) => ({
          ...q,
          state: QuestionState.Unchanged,
          options: q.options.map((o) => ({
            id: o.id,
            text: o.text,
            imageFile: null,
            imageUrl: o.imageUrl ?? null,
          })),
        }))
      );

      setTest(data);
      setPreview(data.coverImageUrl ?? "");
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    fetchTest();
  }, [id]);

  if (!test) return <div className={styles.loader}>Loading...</div>;

  const handleSaveTest = async (metadata: UpdateTestMetadataDto) => {
    await handleSaveTestData(metadata);
    await handleSaveQuestions();
  };

  const handleSaveTestData = async (dto: UpdateTestMetadataDto) => {
    await updateTestData(dto);
    alert("Test info saved");
  };

  const handleSaveQuestions = async () => {
    if (!test) return;

    let dto = getSaveQuestionsDto(questions);

    if (dto == null) return;

    const updatedQuestionsResultDto = await updateTestQuestions(dto);

    const updatedQuestions = questions
      .filter((q) => q.state !== QuestionState.Deleted)
      .map((q) => {
        const newQuestionId =
          updatedQuestionsResultDto.questionTempToRegularIds[q.id];

        const updatedOptions = q.options.map((o) => {
          const newOptionId =
            updatedQuestionsResultDto.optionTempToRegularIds[o.id];
          return newOptionId ? { ...o, id: newOptionId } : o;
        });

        const updatedCorrectAnswers = q.correctAnswers.map((ca) => {
          const newAnswerId =
            updatedQuestionsResultDto.optionTempToRegularIds[ca.id];
          return newAnswerId ? { ...ca, id: newAnswerId } : ca;
        });

        return {
          ...q,
          id: newQuestionId ?? q.id,
          options: updatedOptions,
          correctAnswers: updatedCorrectAnswers,
        };
      });

    console.log(updatedQuestions);

    setQuestions(
      updatedQuestions.map((uq) => ({ ...uq, state: QuestionState.Unchanged }))
    );
    dto = getSaveQuestionsDto(updatedQuestions);

    alert("Questions saved");

    if (dto == null) return;

    await updateQuestionsAndOptionsImages(dto);
  };

  const onUserSendPrompt = async (prompt: string): Promise<void> => {
    const dto: PromptQuestionsDto = {
      prompt: prompt,
      testId: test.id,
    };

    const data = await getUpdateQuestionDtoFromPrompt(dto);

    console.log(data);
  };

  const getSaveQuestionsDto = (
    questions: EditableQuestionDto[]
  ): UpdateQuestionsDto | null => {
    const questionsToAdd = questions.filter(
      (q) => q.state === QuestionState.Added
    );

    const questionsToUpdate = questions.filter(
      (q) => q.state === QuestionState.Changed
    );

    const questionsToDeleteIds = questions
      .filter((q) => q.state === QuestionState.Deleted)
      .map((q) => q.id);

    if (
      questionsToAdd.length === 0 &&
      questionsToUpdate.length === 0 &&
      questionsToDeleteIds.length === 0
    ) {
      return null;
    }

    return {
      testId: test.id,
      questionsToAdd,
      questionsToUpdate,
      questionsToDeleteIds,
    };
  };

  return (
    <div className={styles.editTestPage}>
      <Header />

      <div className={styles.wrapper}>
        <div className={styles.container}>
          <div className={styles.content}>
            <TestHeader
              metadata={{
                id: test.id,
                title: test.title,
                description: test.description,
                timeLimitInMinutes: test.timeLimitMinutes,
                isPublic: test.isPublic,
                coverImage: null,
              }}
              preview={preview}
              onSaveTest={handleSaveTest}
              onCancel={() => navigate(-1)}
            />

            <QuestionsList
              questions={questions}
              setQuestions={setQuestions}
              setSelectedQuestion={setSelectedQuestion}
              setSelectedOption={setSelectedOption}
            />
          </div>

          <SidebarEditor
            questions={questions}
            setQuestions={setQuestions}
            selectedQuestion={selectedQuestion}
            selectedOption={selectedOption}
            setSelectedQuestion={setSelectedQuestion}
            setSelectedOption={setSelectedOption}
            onUserSendPrompt={onUserSendPrompt}
          />
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default EditTestPage;
