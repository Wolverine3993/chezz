<template>
  <div class="w-full bg-zinc-950">
    <div class="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
      <div v-if="username !== ''" class="space-y-6">
        <div class="rounded-xl border border-white/10 bg-zinc-900/70 p-6">
          <div class="flex items-center justify-between gap-4">
            <div>
              <p class="text-sm text-zinc-400">Welcome back</p>
              <h1 class="mt-1 text-3xl font-semibold text-white">{{ username }}</h1>
            </div>
            <div class="rounded-full border border-emerald-500/30 bg-emerald-500/10 px-3 py-1 text-xs font-medium text-emerald-300">
              Online
            </div>
          </div>
        </div>

        <div class="rounded-xl border border-white/10 bg-zinc-900/70 p-6">
          <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            <ChezzButton @click="matchmake">Matchmake</ChezzButton>
          </div>
          <ErrorAlert :messages="matchmakeErrors" class="mt-4" />
        </div>
      </div>

      <div v-else class="mx-auto max-w-xl rounded-xl border border-white/10 bg-zinc-900/70 p-8 text-center">
        <ChezzLogo class="mx-auto h-10 w-auto text-blue-600" />
        <h1 class="mt-5 text-2xl font-semibold text-white">Sign in... for CHEZZ</h1>
        <p class="mt-2 text-sm text-zinc-400">
            Be prepared for the ABSOLUTE BEST CHESS... one click away.
        </p>
        <div class="mt-6 flex justify-center gap-3">
          <ChezzButton to="/signin">Sign in</ChezzButton>
          <ChezzButton to="/register" class="border-white/10 bg-white/5 hover:bg-white/10">Register</ChezzButton>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { api } from "../composables/api";

const username = ref("");
api
  .Identity_GetInfo(undefined)
  .then((user) => {
    username.value = user.username ?? "";
  })
  .catch(() => {
    username.value = "";
  });

const router = useRouter();
const matchmakeErrors = ref<string[]>([]);
async function matchmake() {
  matchmakeErrors.value = [];
  try {
    const lobbyId = await api.Chess_Matchmake(undefined);
    router.push(`/chess/game/${lobbyId}`);
  } catch (e) {
    matchmakeErrors.value = extractApiErrors(e);
  }
}
</script>
