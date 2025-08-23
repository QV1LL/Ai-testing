import type { CreateTestDto, TestDto, UserTestsResultDto } from "../types/test";
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

export const getUserTests = async (): Promise<TestDto[]> => {
  const res = await api.get<UserTestsResultDto>("/tests");
  return res.data.tests;
};
