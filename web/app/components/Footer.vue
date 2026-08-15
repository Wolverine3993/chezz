<template>
  <nav v-if="props.page !== undefined && props.count !== 0" class="flex items-center justify-between border-t border-white/10 px-4 py-3 sm:px-6" aria-label="Pagination">
    <div class="hidden sm:block">
      <p class="text-sm text-gray-300 px-5">
        Showing
        {{ ' ' }}
        <span class="font-medium">{{ page ? page * 5 + 1 : 1 }}</span>
        {{ ' ' }}
        to
        {{ ' ' }}
        <span class="font-medium">{{ page ? Math.min(page * 5 + 5, count ? count : 1) : 1 }}</span>
        {{ ' ' }}
        of
        {{ ' ' }}
        <span class="font-medium">{{ count ? count : 5 }}</span>
        {{ ' ' }}
        results
      </p>
    </div>
    <div class="flex flex-1 justify-between sm:justify-end">
      <button @click="$emit('prevPressed')" class="relative inline-flex items-center rounded-md bg-white/10 px-3 py-2 text-sm font-semibold text-gray-200 inset-ring inset-ring-white/5 hover:bg-white/20 disabled:bg-white/1 disabled:text-gray-400">Previous</button>
      <button @click="$emit('nextPressed')" class="relative ml-3 inline-flex items-center rounded-md bg-white/10 px-3 py-2 text-sm font-semibold text-gray-200 inset-ring inset-ring-white/5 hover:bg-white/20 disabled:bg-white/1 disabled:text-gray-400">Next</button>
    </div>
  </nav>
</template>

<script setup lang="ts">
const emit = defineEmits(['nextPressed', 'prevPressed']);
const props = defineProps<{
    count?: number,
    page?: number,
}>();

const count = computed(() => props.count);
const page = computed(() => props.page);
</script>