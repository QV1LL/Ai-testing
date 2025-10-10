import type {
  AddTestAttemptDto,
  AddTestAttemptResultDto,
  FinishTestAttemptDto,
  TestAttemptMetadataDto,
  TestAttemptResultDto,
} from "../types/testAttempt";
import api from "./api";

export const getAttemptById = async (
  id: string
): Promise<TestAttemptMetadataDto> => {
  const res = await api.get<TestAttemptMetadataDto>(`/testAttempts/${id}`);
  return res.data;
};

export const createAttempt = async (
  data: AddTestAttemptDto
): Promise<AddTestAttemptResultDto> => {
  const res = await api.post<AddTestAttemptResultDto>("/testAttempts", data);
  return res.data;
};

export const finishAttempt = async (
  data: FinishTestAttemptDto
): Promise<TestAttemptResultDto> => {
  const res = await api.put<TestAttemptResultDto>("/testAttempts", data);
  return res.data;
};
