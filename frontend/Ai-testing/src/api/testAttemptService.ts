import type { AddTestAttemptDto } from "../types/testAttempt";
import api from "./api";

export const createAttempt = async (data: AddTestAttemptDto) => {
  await api.post<AddTestAttemptDto>("/testAttempts", data);
};
