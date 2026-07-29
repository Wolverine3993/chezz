import type { z } from "zod";
import { api, schemas } from "./api";

type User = z.infer<typeof schemas.InfoResponse>;

export function useUser() {
  const { data, status, error, refresh } = useAsyncData<User | null>(
    "user",
    // 401/404 treated as "no user"
    () => api.Identity_GetInfo().catch(() => null),
    { default: () => null },
  );

  const user = data;
  const isLoggedIn = computed(() => user.value !== null);

  async function login(body: z.infer<typeof schemas.LoginRequest>) {
    await api.Identity_Login(body, { queries: { useCookies: true } });
    await refresh();
  }

  async function logout() {
    // No logout endpoint in the API yet; clear the cookie server-side when one
    // exists. For now just drop the local state.
    user.value = null;
  }

  return { user, isLoggedIn, status, error, refresh, login, logout };
}