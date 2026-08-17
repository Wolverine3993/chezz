import type { z } from "zod";
import { api, schemas } from "./api";

type User = z.infer<typeof schemas.InfoResponse>;

export function useUser() {
  const { data, status, error, refresh } = useAsyncData<User | null>(
    "user",
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
    user.value = null;
  }

  const { avatarUrl } = useAvatar();

  const svg = computed(() => {
    if(!user.value) return undefined;
    return avatarUrl(user.value.email);
  });

  return { user, isLoggedIn, svg, status, error, refresh, login, logout };
}
