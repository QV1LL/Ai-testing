import type { AnswerOptionDto } from "./test";

export interface TestAttemptMetadataDto {
  testId: string;
}

export interface AddTestAttemptDto {
  testId: string;
  userId?: string;
  guestName?: string;
  startedAt: Date;
}

export interface AddTestAttemptResultDto {
  attemptId: string;
}

export interface FinishTestAttemptDto {
  attemptId: string;
  answers: AttemptAnswerDto[];
}

export interface AttemptAnswerDto {
  questionId: string;
  selectedOptions: AnswerOptionDto[];
  writtenAnswer?: string;
}

export interface TestAttemptResultDto {
  testTitle: string;
  displayUsername: string;
  startedAt: Date;
  finishedAt: Date;
  score: number;
}
