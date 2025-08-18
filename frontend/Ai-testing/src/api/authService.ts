import api from "./api";
import type { LoginDto, RegisterDto, LoginResult } from "../types/user";

export const login = async (data: LoginDto): Promise<LoginResult> => {
  const res = await api.post<LoginResult>("/auth/login", data);
  return res.data;
};

export const register = async (data: RegisterDto): Promise<LoginResult> => {
  const res = await api.post<LoginResult>("/auth/register", data);
  return res.data;
};

export const logout = async (): Promise<void> => {
  return Promise.resolve();
};
