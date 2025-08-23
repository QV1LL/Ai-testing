export interface TestInfo {
  id: string;
  title: string;
  createdAt: Date;
}

export interface TestAttemptsInfo {
  id: string;
  title: string;
  startedAt: Date;
  score: number;
}

export interface ProfileDto {
  name: string;
  email: string;
  avatarUrl: string;
  tests: TestInfo[];
  testAttempts: TestAttemptsInfo[];
}

export interface UpdateProfileDto {
  name: string;
  email: string;
  avatarImage: File | null;
}
