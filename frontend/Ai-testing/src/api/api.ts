import axios from "axios";
import type { LoginResult } from "../types/user";
import { logout } from "./profileService";

export const ACCESS_TOKEN_KEY = "access_token";
export const REFRESH_TOKEN_KEY = "refresh_token";

const BASE_URL = "http://localhost:5177/api";

export const getAccessToken = () => localStorage.getItem(ACCESS_TOKEN_KEY);
export const getRefreshToken = () => localStorage.getItem(REFRESH_TOKEN_KEY);

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// === Request Interceptor ===
api.interceptors.request.use(
  (config) => {
    const token = getAccessToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// === Response Interceptor ===
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const newToken = await refreshToken();
        if (newToken) {
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return api(originalRequest);
        }
      } catch {}
    }

    return Promise.reject(error);
  }
);

// === Refresh Token ===
const authApi = axios.create({
  baseURL: BASE_URL,
  headers: { "Content-Type": "application/json" },
});

export const refreshToken = async (): Promise<string | null> => {
  const token = getRefreshToken();
  if (!token) return null;

  try {
    const res = await authApi.post<LoginResult>("/auth/refresh", {
      refreshToken: token,
    });
    const { accessToken, refreshToken: newRefreshToken } = res.data;

    if (accessToken) localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    if (newRefreshToken)
      localStorage.setItem(REFRESH_TOKEN_KEY, newRefreshToken);

    return accessToken ?? null;
  } catch (error) {
    console.error("Refresh token failed", error);
    logout();
    return null;
  }
};

export default api;
