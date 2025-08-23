export interface CreateTestDto {
  title: string;
  description: string;
  isPublic: boolean;
  timeLimitMinutes: number;
}

export interface TestDto {
  id: string;
  title: string;
  description: string;
}

export interface UserTestsResultDto {
  tests: TestDto[];
}
