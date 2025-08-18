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
  token: string;
  userId: string;
  displayName: string;
  email: string;
}

export interface ProfileDto {
  name: string;
  email: string;
}
