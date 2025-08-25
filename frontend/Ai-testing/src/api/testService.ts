import type {
  CreateTestDto,
  FullTestDto,
  TestDto,
  UpdateQuestionsDto,
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
  if (dto.description) formData.append("description", dto.description);
  if (dto.coverImage) formData.append("coverImage", dto.coverImage);

  await api.put(`/tests/${dto.id}/metadata`, formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
};

export const updateTestQuestions = async (
  dto: UpdateQuestionsDto
): Promise<void> => {
  await api.put(`/tests/${dto.testId}/questions`, dto);
};

export const getUserTests = async (): Promise<TestDto[]> => {
  const res = await api.get<UserTestsResultDto>("/tests");
  return res.data.tests;
};

export const getById = async (id: string): Promise<FullTestDto> => {
  const res = await api.get<FullTestDto>(`/tests/${id}`);
  return res.data;
};
