export interface CreateTestDto {
  title: string;
  description: string;
  isPublic: boolean;
  timeLimitMinutes: number;
}

export interface TestDto {
  id: string;
  title: string;
  description?: string;
}

export interface UpdateTestMetadataDto {
  id: string;
  title: string;
  description?: string;
  coverImage?: File | null;
}

export interface UpdateQuestionsDto {
  testId: string;
  questionsToUpdate: QuestionDto[];
}

export interface UserTestsResultDto {
  tests: TestDto[];
}

export enum QuestionType {
  SingleChoice,
  MultipleChoice,
  OpenEnded,
}

export interface AnswerOptionDto {
  id: string;
  text: string;
  imageUrl?: string;
  order: number;
}

export interface QuestionDto {
  id: string;
  text: string;
  imageUrl?: string;
  type: QuestionType;
  order: number;
  options: AnswerOptionDto[];
  correctAnswers: AnswerOptionDto[];
}

export interface TestAttemptDto {
  id: string;
  userId: string;
  score: number;
  startedAt: Date;
  finishedAt?: Date;
}

export interface FullTestDto {
  id: string;
  title: string;
  description?: string;
  coverImageUrl?: string;
  isPublic: boolean;
  timeLimitMinutes?: number;
  questions: QuestionDto[];
  testAttempts: TestAttemptDto[];
  attemptsCount: number;
  averageScore: number;
}
