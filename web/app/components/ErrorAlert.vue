<template>
  <div v-if="messages.length" class="rounded-md bg-red-500/15 p-4 outline outline-red-500/25">
    <div class="flex">
      <div class="shrink-0">
        <XCircleIcon class="size-5 text-red-400" aria-hidden="true" />
      </div>
      <div class="ml-3">
        <h3 class="text-sm font-medium text-red-200">{{ heading }}</h3>
        <ul v-if="messages.length > 1" class="mt-2 list-disc space-y-1 pl-5 text-sm text-red-300">
          <li v-for="(message, i) in messages" :key="i">{{ message }}</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { XCircleIcon } from "@heroicons/vue/20/solid";

const props = withDefaults(
  defineProps<{
    messages?: string[];
    title?: string;
  }>(),
  {
    messages: () => [],
  },
);

const heading = computed(() => {
  if (props.title) return props.title;
  if (props.messages.length === 1) return props.messages[0] ?? "";
  return `There were ${props.messages.length} errors with your submission`;
});
</script>
