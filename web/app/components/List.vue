<template>
  <ul role="list" class="divide-y divide-white/5 overflow-hidden bg-gray-800/50 outline-1 outline-white/10 sm:rounded-xl sm:-outline-offset-1 min-h-[448px]">
    <div v-if="listLength !== undefined && listLength > 0">
        <li v-if="props.type === 'Friends'" v-for="name in props.users" :key="name" class="relative flex justify-between gap-x-6 px-4 py-5 sm:px-6 border-dashed border-white border-y">
        <div class="flex min-w-0 gap-x-4">
            <img class="size-12 flex-none rounded-full bg-gray-800 outline -outline-offset-1 outline-white/10" :src="imageUrl" alt="" />
            <div class="min-w-0 flex-auto">
            <p class="text-sm/6 font-semibold text-white">
                <span class="absolute inset-x-0 bottom-0"></span>
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

        <li v-else-if="props.type === 'Requests'" v-for="request in props.requests" :key="request.requestId" class="relative flex justify-between gap-x-6 px-4 py-5 sm:px-6 border-dashed border-white border-y">
        <div class="flex min-w-0 gap-x-4">
            <img class="size-12 flex-none rounded-full bg-gray-800 outline -outline-offset-1 outline-white/10" :src="imageUrl" alt="" />
            <div class="min-w-0 flex-auto">
            <p class="text-sm/6 font-semibold text-white">
                <span class="absolute inset-x-0 bottom-0"></span>
                {{ request.username }}
            </p>
            </div>
        </div>
        <div class="flex shrink-0 items-center gap-x-4">
            <button @click="accept(request.requestId)" type="button" class="inline-flex items-center gap-x-1.5 hover:cursor-pointer rounded-md bg-green-500 px-3 py-2 text-sm font-semibold text-white hover:bg-green-400 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500">
                Accept
                <CheckCircleIcon class="-mr-0.5 size-5" aria-hidden="true" />
            </button>
            <button @click="decline(request.requestId)" type="button" class="inline-flex items-center gap-x-1.5 hover:cursor-pointer rounded-md bg-red-500 px-3 py-2 text-sm font-semibold text-white hover:bg-red-400 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500">
                Decline
                <XCircleIcon class="-mr-0.5 size-5" aria-hidden="true" />
            </button>
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
import { ChevronRightIcon, CheckCircleIcon, XCircleIcon } from '@heroicons/vue/20/solid'

const props = defineProps<{
    users?: string[],
    requests?: {
        username: string,
        requestId: string,
    }[],
    type?: "Friends" | "Requests",
}>();

const imageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80";
const router = useRouter();

const listLength = computed(() => props.type === "Friends" ? props.users?.length : props.requests?.length);

async function matchmake() {
    // to do
}

async function unfriend(username: string) {
    await api.UserRelationship_RemoveFriend({
        username: username
    });
    emit("reload");
}

async function accept(requestId: string) {
    await api.UserRelationship_AcceptFriendRequest({
        requestId: requestId
    });
    emit("reload");
}

async function decline(requestId: string) {
    await api.UserRelationship_DeclineFriendRequest({
        requestId: requestId
    });
    emit("reload");
}

const emit = defineEmits(["reload"]);
</script>