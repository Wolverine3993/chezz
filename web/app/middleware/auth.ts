export default defineNuxtRouteMiddleware(async () => {
  const { user, refresh } = useUser();
  if (!user.value) {
    await refresh();
  }
  if (!user.value) {
    return navigateTo("/signin");
  }
});
