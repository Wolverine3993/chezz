import type { z } from "zod";
import { api, schemas } from "./api";

type User = z.infer<typeof schemas.InfoResponse>;

import { Style, Avatar } from "@dicebear/core";
import lorelei from "@dicebear/styles/lorelei.json";

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

  const style = new Style(lorelei);

  const svg = computed(() => {
    if(!user.value) return undefined;
    const avatar = new Avatar(style, {
      seed: user.value.email,
    });

    return `data:image/svg+xml;utf8,${encodeURIComponent(avatar.toString())}`;
  });

  return { user, isLoggedIn, svg, status, error, refresh, login, logout };
}
