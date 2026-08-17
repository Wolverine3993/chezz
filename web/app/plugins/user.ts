export default defineNuxtPlugin(async () => {
  await useUser().refresh();
});
