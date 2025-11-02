import type {
  CreateTestDto,
  FullTestDto,
  PromptQuestionsDto,
  TestMetadataDto,
  TestPreviewDto,
  UpdateOptionDto,
  UpdateOptionImageDto,
  UpdateQuestionDto,
  UpdateQuestionImageDto,
  UpdateQuestionsDto,
  UpdateQuestionsResultDto,
  UpdateTestMetadataDto,
  UserTestsResultDto,
} from "../types/test";
import api from "./api";

export const create = async (
  data: CreateTestDto,
  coverImage?: File | null
): Promise<void> => {
  try {
    const formData = new FormData();

    formData.append("title", data.title);
    formData.append("description", data.description);
    formData.append("isPublic", String(data.isPublic));
    formData.append(
      "timeLimitMinutes",
      data.timeLimitMinutes ? String(data.timeLimitMinutes) : ""
    );

    if (coverImage) {
      formData.append("coverImage", coverImage);
    }

    await api.post("/tests/create", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  } catch (error: any) {
    const errorMessage =
      error.response?.data?.message ?? "Failed to create test";

    throw new Error(errorMessage);
  }
};

export const deleteTest = async (id: string): Promise<void> => {
  try {
    await api.delete(`/tests/${id}`);
  } catch (error: any) {
    const errorMessage =
      error.response?.data?.message ?? "Failed to delete profile";

    throw new Error(errorMessage);
  }
};

export const updateTestData = async (
  dto: UpdateTestMetadataDto
): Promise<void> => {
  const formData = new FormData();
  formData.append("id", dto.id);
  formData.append("title", dto.title);
  formData.append("isPublic", `${dto.isPublic}`);
  if (dto.timeLimitInMinutes)
    formData.append("timeLimitInMinutes", `${dto.timeLimitInMinutes}`);
  if (dto.description) formData.append("description", dto.description);
  if (dto.coverImage) formData.append("coverImage", dto.coverImage);

  await api.put(`/tests/metadata`, formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
};

export const getUpdateQuestionDtoFromPrompt = async (
  dto: PromptQuestionsDto
): Promise<UpdateQuestionsDto> => {
  const res = await api.get<UpdateQuestionsDto>(`/tests/prompt`, {
    params: dto,
  });
  return res.data;
};

export const updateTestQuestions = async (
  dto: UpdateQuestionsDto
): Promise<UpdateQuestionsResultDto> => {
  const dtoWithoutFiles = {
    ...dto,
    questionsToAdd: dto.questionsToAdd.map((q) => ({
      ...q,
      imageFile: undefined,
      options: q.options.map((o) => ({ ...o, imageFile: undefined })),
    })),
    questionsToUpdate: dto.questionsToUpdate.map((q) => ({
      ...q,
      imageFile: undefined,
      options: q.options.map((o) => ({ ...o, imageFile: undefined })),
    })),
  };

  const res = await api.put<UpdateQuestionsResultDto>(
    "/tests/questions",
    dtoWithoutFiles
  );

  console.log(dtoWithoutFiles);

  return res.data;
};

export const updateQuestionsAndOptionsImages = async (
  dto: UpdateQuestionsDto
): Promise<void> => {
  const uploadPromises: Promise<void>[] = [];

  const handleQuestionImages = (question: UpdateQuestionDto) => {
    if (question.imageFile) {
      const qDto: UpdateQuestionImageDto = {
        testId: dto.testId,
        id: question.id,
        imageFile: question.imageFile,
      };
      uploadPromises.push(updateQuestionImage(qDto));
    }

    question.options.forEach((option: UpdateOptionDto) => {
      if (option.imageFile) {
        const oDto: UpdateOptionImageDto = {
          testId: dto.testId,
          questionId: question.id,
          id: option.id,
          imageFile: option.imageFile,
        };
        uploadPromises.push(updateOptionImage(oDto));
      }
    });
  };

  dto.questionsToAdd.forEach(handleQuestionImages);
  dto.questionsToUpdate.forEach(handleQuestionImages);

  await Promise.all(uploadPromises);
};

export const updateQuestionImage = async (
  dto: UpdateQuestionImageDto
): Promise<void> => {
  const formData = new FormData();
  formData.append("testId", dto.testId);
  formData.append("id", dto.id);
  if (dto.imageFile) {
    formData.append("imageFile", dto.imageFile);
  }

  await api.post("/tests/question/image", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
};

export const updateOptionImage = async (
  dto: UpdateOptionImageDto
): Promise<void> => {
  const formData = new FormData();
  formData.append("testId", dto.testId);
  formData.append("questionId", dto.questionId);
  formData.append("id", dto.id);
  if (dto.imageFile) {
    formData.append("imageFile", dto.imageFile);
  }

  await api.post("/tests/question/option/image", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
};

export const getUserTests = async (): Promise<TestMetadataDto[]> => {
  const res = await api.get<UserTestsResultDto>("/tests");
  return res.data.tests;
};

export const getTestPreview = async (id: string): Promise<TestPreviewDto> => {
  const res = await api.get<TestPreviewDto>(`/tests/preview/${id}`);
  return res.data;
};

export const getById = async (id: string): Promise<FullTestDto> => {
  const res = await api.get<FullTestDto>(`/tests/${id}`);
  return res.data;
};
