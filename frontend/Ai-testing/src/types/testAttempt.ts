import type { AnswerOptionDto } from "./test";

export interface AddTestAttemptDto {
  testId: string;
  userId?: string;
  guestName?: string;
  startedAt: Date;
  answers: AttemptAnswerDto[];
}

export interface AttemptAnswerDto {
  questionId: string;
  selectedOptions: AnswerOptionDto[];
  writtenAnswer?: string;
}
