// Runs once on app startup (client-only since ssr is disabled) to prime the
// shared "user" state. Any component can then read it via useUser() without
// re-fetching, and call refresh()/login()/logout() to update it.
export default defineNuxtPlugin(async () => {
  await useUser().refresh();
});
