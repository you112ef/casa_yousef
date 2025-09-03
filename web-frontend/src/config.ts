export const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string) || "http://localhost:8080/api/v1";
export const AUTH_LOGIN_PATH = (import.meta.env.VITE_AUTH_LOGIN_PATH as string) || "/auth/login";
export const AUTH_PROFILE_PATH = (import.meta.env.VITE_AUTH_PROFILE_PATH as string) || "/auth/me";
export const AI_ANALYZE_IMAGE_PATH = (import.meta.env.VITE_AI_ANALYZE_IMAGE_PATH as string) || "/ai/sperm/analyze-image";
export const AI_ANALYZE_VIDEO_PATH = (import.meta.env.VITE_AI_ANALYZE_VIDEO_PATH as string) || "/ai/sperm/analyze-video";
export const MAX_UPLOAD_MB = Number(import.meta.env.VITE_MAX_UPLOAD_MB || 200);
