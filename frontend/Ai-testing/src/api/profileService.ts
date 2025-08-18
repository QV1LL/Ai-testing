import api from "./api";
import type { ProfileDto } from "../types/user";

export const getProfile = async (): Promise<ProfileDto> => {
  const res = await api.get<ProfileDto>("/users/profile");
  return res.data;
};

export const logout = (): void => {
  localStorage.removeItem("access_token");
  localStorage.removeItem("refresh_token");
  localStorage.removeItem("userId");
  localStorage.removeItem("displayName");
};
