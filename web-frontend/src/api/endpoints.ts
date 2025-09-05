import { api } from "./client";
import { z } from "zod";

export const LoginDto = z.object({ username: z.string().min(1), password: z.string().min(1) });
export type LoginDto = z.infer<typeof LoginDto>;

export type User = {
  id: string | number;
  name?: string | null;
  email?: string | null;
  role?: string | null;
};

export type AuthResponse = { token: string; user?: User | null };

export async function login(dto: LoginDto) {
  const { data } = await api.post("/auth/login", dto);
  const token = (data as any)?.token as string | undefined;
  if (!token) throw new Error("Invalid login response");
  localStorage.setItem("token", token);
  const user = (data as any)?.user ?? null;
  if (user) localStorage.setItem("user", JSON.stringify(user));
  return { token, user } as AuthResponse;
}

export async function me() {
  const { data } = await api.get("/auth/me");
  return data as any;
}

export const Patient = z.object({
  id: z.union([z.string(), z.number()]).transform(String),
  firstName: z.string().optional(),
  lastName: z.string().optional(),
  dateOfBirth: z.string().optional(),
  gender: z.string().optional(),
  phoneNumber: z.string().optional(),
  email: z.string().optional(),
  address: z.string().optional(),
});
export type Patient = z.infer<typeof Patient>;

export const CBCResult = z.object({
  id: z.union([z.string(), z.number()]).transform(String),
  patientId: z.union([z.string(), z.number()]).transform(String).optional(),
  testDate: z.string().optional(),
  wbc: z.number().optional(),
  rbc: z.number().optional(),
  hemoglobin: z.number().optional(),
  hematocrit: z.number().optional(),
  plateletCount: z.number().optional(),
});
export type CBCResult = z.infer<typeof CBCResult>;

export async function getPatients(params?: { limit?: number; offset?: number; search?: string }) {
  const { data } = await api.get("/patients", { params });
  return data as { patients: Patient[]; total: number; limit: number; offset: number };
}

export async function getCBC(params?: { patientId?: string; limit?: number; offset?: number; startDate?: string; endDate?: string }) {
  const { data } = await api.get("/cbc-results", { params });
  return data as { results: CBCResult[]; total: number; limit: number; offset: number };
}

export async function analyzeImage(file: File) {
  const form = new FormData();
  form.append("file", file);
  const { data } = await api.post("/ai/sperm/analyze-image", form, { headers: { "Content-Type": "multipart/form-data" } });
  return data as any;
}

export async function analyzeVideo(file: File, durationSec?: number) {
  const form = new FormData();
  form.append("file", file);
  if (durationSec) form.append("durationSec", String(durationSec));
  const { data } = await api.post("/ai/sperm/analyze-video", form, { headers: { "Content-Type": "multipart/form-data" } });
  return data as any;
}

export const Patient = z.object({
  id: z.union([z.string(), z.number()]).transform(String),
  firstName: z.string().optional(),
  lastName: z.string().optional(),
  dateOfBirth: z.string().optional(),
  gender: z.string().optional(),
  phoneNumber: z.string().optional(),
  email: z.string().optional(),
  address: z.string().optional(),
});
export type Patient = z.infer<typeof Patient>;

export async function getPatients(params?: { limit?: number; offset?: number; search?: string }) {
  const { data } = await api.get("/patients", { params });
  return data as { patients: Patient[]; total: number; limit: number; offset: number };
}

export const CBCResult = z.object({
  id: z.union([z.string(), z.number()]).transform(String),
  patientId: z.union([z.string(), z.number()]).transform(String).optional(),
  testDate: z.string().optional(),
  wbc: z.number().optional(),
  rbc: z.number().optional(),
  hemoglobin: z.number().optional(),
  hematocrit: z.number().optional(),
  plateletCount: z.number().optional(),
});
export type CBCResult = z.infer<typeof CBCResult>;

export async function getCBC(params?: { patientId?: string; limit?: number; offset?: number; startDate?: string; endDate?: string }) {
  const { data } = await api.get("/cbc-results", { params });
  return data as { results: CBCResult[]; total: number; limit: number; offset: number };
}

export async function analyzeImage(file: File) {
  const form = new FormData();
  form.append("file", file);
  const { data } = await api.post("/ai/sperm/analyze-image", form, { headers: { "Content-Type": "multipart/form-data" } });
  return data as any;
}

export async function analyzeVideo(file: File, durationSec?: number) {
  const form = new FormData();
  form.append("file", file);
  if (durationSec) form.append("durationSec", String(durationSec));
  const { data } = await api.post("/ai/sperm/analyze-video", form, { headers: { "Content-Type": "multipart/form-data" } });
  return data as any;
}
