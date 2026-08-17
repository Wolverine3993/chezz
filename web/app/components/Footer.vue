<template>
  <nav v-if="props.page !== undefined && props.count !== 0" class="flex items-center justify-between border-t border-white/10 px-4 py-3 sm:px-6" aria-label="Pagination">
    <div class="hidden sm:block">
      <p class="text-sm text-gray-300 px-5">
        Showing
        {{ ' ' }}
        <span class="font-medium">{{ startItem }}</span>
        {{ ' ' }}
        to
        {{ ' ' }}
        <span class="font-medium">{{ endItem }}</span>
        {{ ' ' }}
        of
        {{ ' ' }}
        <span class="font-medium">{{ count ? count : pageSize }}</span>
        {{ ' ' }}
        results
      </p>
    </div>
    <div class="flex flex-1 items-center justify-between gap-3 sm:justify-end">
      <button @click="$emit('prevPressed')" :disabled="isFirstPage" class="relative inline-flex items-center rounded-md bg-white/10 px-3 py-2 text-sm font-semibold text-gray-200 inset-ring inset-ring-white/5 hover:bg-white/20 disabled:bg-white/1 disabled:text-gray-400">Previous</button>

      <div class="hidden sm:flex items-center gap-1">
        <button
          v-for="pageNumber in totalPages"
          :key="pageNumber"
          @click="$emit('pageSelected', pageNumber - 1)"
          :class="[
            'inline-flex h-8 min-w-8 items-center justify-center rounded-md px-2 text-sm font-medium inset-ring inset-ring-white/5',
            pageNumber - 1 === currentPage ? 'bg-white text-zinc-900' : 'bg-white/10 text-gray-200 hover:bg-white/20'
          ]"
        >
          {{ pageNumber }}
        </button>
      </div>

      <button @click="$emit('nextPressed')" :disabled="isLastPage" class="relative inline-flex items-center rounded-md bg-white/10 px-3 py-2 text-sm font-semibold text-gray-200 inset-ring inset-ring-white/5 hover:bg-white/20 disabled:bg-white/1 disabled:text-gray-400">Next</button>
    </div>
  </nav>
</template>

<script setup lang="ts">
const emit = defineEmits<{
    nextPressed: []
    prevPressed: []
    pageSelected: [page: number]
}>();
const props = defineProps<{
    count?: number,
    page?: number,
    pageSize?: number,
}>();

const count = computed(() => props.count);
const page = computed(() => props.page);
const pageSize = computed(() => props.pageSize ?? 5);
const currentPage = computed(() => page.value ?? 0);
const totalPages = computed(() => Math.max(1, Math.ceil((count.value ?? 0) / pageSize.value)));
const startItem = computed(() => (count.value && count.value > 0 ? currentPage.value * pageSize.value + 1 : 1));
const endItem = computed(() => {
    if (!count.value || count.value <= 0) return 1;
    return Math.min((currentPage.value + 1) * pageSize.value, count.value);
});
const isFirstPage = computed(() => currentPage.value <= 0);
const isLastPage = computed(() => currentPage.value >= totalPages.value - 1);
</script>