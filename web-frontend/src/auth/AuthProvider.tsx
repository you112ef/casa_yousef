import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { login as apiLogin, me, type User } from "../api/endpoints";

type AuthContextType = {
  user: User | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
};

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(localStorage.getItem("token"));
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const init = async () => {
      if (!token) return;
      try {
        const u = await me();
        setUser(u as any);
      } catch {
        const cached = localStorage.getItem("user");
        if (cached) setUser(JSON.parse(cached));
        // Keep token; backend may be waking up or missing /auth/me
      }
    };
    init();
  }, [token]);

  const value = useMemo<AuthContextType>(() => ({
    user,
    token,
    login: async (username: string, password: string) => {
      setLoading(true);
      try {
        const { token: t, user: u } = await apiLogin({ username, password });
        setToken(t);
        setUser(u ?? null);
      } finally {
        setLoading(false);
      }
    },
    logout: () => {
      localStorage.removeItem("token");
      setToken(null);
      setUser(null);
      location.href = "/login";
    },
    loading,
  }), [user, token, loading]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
