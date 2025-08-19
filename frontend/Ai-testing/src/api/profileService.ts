import api from "./api";
import type { ProfileDto, UpdateProfileDto } from "../types/profile";

export const getProfile = async (): Promise<ProfileDto> => {
  const res = await api.get<ProfileDto>("/users/profile");
  return res.data;
};

export const updateProfile = async (dto: UpdateProfileDto): Promise<void> => {
  try {
    await api.put("/users/profile", dto);
  } catch (error: any) {
    const errorMessage =
      error.response?.data?.message ?? "Failed to update profile";

    throw new Error(errorMessage);
  }
};

export const deleteProfile = async (): Promise<void> => {
  try {
    await api.delete("/users/profile");
  } catch (error: any) {
    const errorMessage =
      error.response?.data?.message ?? "Failed to delete profile";

    throw new Error(errorMessage);
  }
};

export const logout = (): void => {
  localStorage.removeItem("access_token");
  localStorage.removeItem("refresh_token");
  localStorage.removeItem("userId");
  localStorage.removeItem("displayName");
};
