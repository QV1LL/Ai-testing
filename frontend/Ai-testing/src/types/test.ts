export interface CreateTestDto {
  title: string;
  description: string;
  isPublic: boolean;
  timeLimitMinutes: number;
}

export interface TestMetadataDto {
  id: string;
  title: string;
  description?: string;
}

export interface TestPreviewDto {
  id: string;
  title: string;
  description?: string;
  coverImageUrl?: string;
  timeLimitMinutes?: number;
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
  questionsToAdd: UpdateQuestionDto[];
  questionsToUpdate: UpdateQuestionDto[];
  questionsToDeleteIds: string[];
}

export interface UpdateQuestionsResultDto {
  questionTempToRegularIds: Record<string, string>;
  optionTempToRegularIds: Record<string, string>;
}

export interface UpdateQuestionDto {
  id: string;
  text: string;
  type: QuestionType;
  order: number;
  imageFile?: File | null;
  imageUrl?: string | null;
  options: UpdateOptionDto[];
  correctAnswers: UpdateOptionDto[];
  correctTextAnswer?: string | null;
}

export interface UpdateOptionDto {
  id: string;
  text: string;
  imageFile?: File | null;
  imageUrl?: string | null;
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
  tests: TestMetadataDto[];
}

export enum QuestionType {
  SingleChoice,
  MultipleChoice,
  OpenEnded,
}

export interface AnswerOptionDto {
  id: string;
  text: string;
  imageUrl?: string | null;
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
  imageUrl?: string | null;
  type: QuestionType;
  order: number;
  options: AnswerOptionDto[];
  correctAnswers: AnswerOptionDto[];
  correctTextAnswer?: string | null;
}

export interface EditableQuestionDto extends UpdateQuestionDto {
  state: QuestionState;
}

export interface TestAttemptDto {
  id: string;
  userDisplayName: string;
  userAvatarUrl: string;
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
  joinId: string;
  questions: QuestionDto[];
  testAttempts: TestAttemptDto[];
  attemptsCount: number;
  averageScore: number;
}

export interface TestForAttemptDto {
  id: string;
  questions: QuestionDto[];
}

export interface PromptQuestionsDto {
  prompt: string;
  testId: string;
}
