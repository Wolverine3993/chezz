<template>
  <ul role="list" class="divide-y divide-white/5 overflow-hidden bg-gray-800/50 outline-1 outline-white/10 sm:rounded-xl sm:-outline-offset-1 min-h-[448px]">
    <div v-if="props.values?.length !== undefined && props.values?.length > 0">
        <li v-for="name in props.values" :key="name" class="relative flex justify-between gap-x-6 px-4 py-5 hover:bg-white/2.5 sm:px-6 border-dashed border-white border-y">
        <div class="flex min-w-0 gap-x-4">
            <img class="size-12 flex-none rounded-full bg-gray-800 outline -outline-offset-1 outline-white/10" :src="imageUrl" alt="" />
            <div class="min-w-0 flex-auto">
            <p class="text-sm/6 font-semibold text-white">
                <span class="absolute inset-x-0 -top-px bottom-0"></span>
                {{ name }}
            </p>
            </div>
        </div>
        <div class="flex shrink-0 items-center gap-x-4">
            <div class="hidden sm:flex sm:flex-col sm:items-end">
            <p class="text-sm/6 text-white">{{ name }}</p>
            </div>
            <ChevronRightIcon class="size-5 flex-none text-gray-500" aria-hidden="true" />
        </div>
        </li>
    </div>
    <div v-else>
        <h1 class="p-6 text-white font-bold">Nothing to display here...</h1>
    </div>
  </ul>
</template>

<script setup lang="ts">
import { ChevronRightIcon } from '@heroicons/vue/20/solid'

const props = defineProps<{
    values?: string[],
    type?: "Friends" | "Requests",
}>();

const imageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80";
</script>