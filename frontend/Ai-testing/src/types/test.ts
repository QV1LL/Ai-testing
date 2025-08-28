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
  timeLimitInMinutes?: number | null;
  isPublic: boolean;
  coverImage?: File | null;
}

export interface UpdateQuestionsDto {
  testId: string;
  questionsToAdd: QuestionDto[];
  questionsToDeleteIds: string[];
  questionsToUpdate: QuestionDto[];
}

export interface UpdateQuestionImageDto {
  id: string;
  testId: string;
  imageFile: File;
}

export interface UpdateOptionImageDto {
  id: string;
  questionId: string;
  testId: string;
  imageFile: File;
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
  imageFile?: File | null;
  order: number;
}

export enum QuestionState {
  Unchanged,
  Added,
  Changed,
  Deleted,
}

export interface QuestionDto {
  id: string;
  text: string;
  imageFile?: File | null;
  type: QuestionType;
  order: number;
  options: AnswerOptionDto[];
  correctAnswers: AnswerOptionDto[];
  correctTextAnswer?: string | null;
}

export interface EditableQuestionDto extends QuestionDto {
  state: QuestionState;
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
