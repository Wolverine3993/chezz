<template>
    <button
        :disabled="loading || disabled"
        class="relative flex h-12 w-full items-center justify-center rounded-md bg-blue-500 px-3 text-sm/6 font-semibold text-white hover:bg-blue-400 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-500 disabled:cursor-not-allowed disabled:opacity-60">
        <span v-if="loading" class="size-5 animate-spin rounded-full border-2 border-white/30 border-t-white" />
        <slot v-else />
    </button>
</template>

<script setup lang="ts">
withDefaults(
    defineProps<{
        loading?: boolean;
        disabled?: boolean;
    }>(),
    { loading: false, disabled: false },
);
</script>
