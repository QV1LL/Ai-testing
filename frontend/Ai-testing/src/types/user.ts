export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  name: string;
}

export interface LoginResult {
  accessToken: string;
  refreshToken: string;
  userId: string;
  displayName: string;
  email: string;
}
