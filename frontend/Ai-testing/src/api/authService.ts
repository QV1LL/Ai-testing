import api, { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY } from "./api";
import type { LoginDto, RegisterDto, LoginResult } from "../types/user";

export const login = async (data: LoginDto): Promise<LoginResult> => {
  const res = await api.post<LoginResult>("/auth/login", data);

  localStorage.setItem(ACCESS_TOKEN_KEY, res.data.accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, res.data.refreshToken);

  return res.data;
};

export const register = async (data: RegisterDto): Promise<LoginResult> => {
  const res = await api.post<LoginResult>("/auth/register", data);

  localStorage.setItem(ACCESS_TOKEN_KEY, res.data.accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, res.data.refreshToken);

  return res.data;
};

export const logout = async (): Promise<void> => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
};
