import type { AnswerOptionDto, QuestionDto } from "./test";

export interface TestAttemptMetadataDto {
  testId: string;
  guestName?: string;
}

export interface AddTestAttemptDto {
  testJoinId: string;
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
  questions: QuestionDto[];
  answers: AttemptAnswerDto[];
  score: number;
}
