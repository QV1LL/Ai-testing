import axios from "axios";
import { getToken } from "../utils/storage";

const api = axios.create({
  baseURL: "https://localhost:7062/api",
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Тут можна викликати logout з AuthContext
      console.warn("Unauthorized! Token may be expired.");
    }
    return Promise.reject(error);
  }
);

export default api;
