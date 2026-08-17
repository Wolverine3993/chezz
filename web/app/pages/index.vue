<template>
    <div class="min-h-screen bg-zinc-900 flex-1">
        <Navbar />
        <h1 class="text-white">{{ username }}</h1>
        <h1 class="text-white">Hello there</h1>
        <button @click="matchmake" type="button" class="rounded-md bg-indigo-600 px-3.5 py-2.5 text-sm font-semibold text-white shadow-xs hover:bg-indigo-500 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 dark:bg-indigo-500 dark:shadow-none dark:hover:bg-indigo-400 dark:focus-visible:outline-indigo-500">Matchmake</button><br>
        <button @click="notification.pollNotifications" type="button" class="rounded-md bg-indigo-600 px-3.5 py-2.5 text-sm font-semibold text-white shadow-xs hover:bg-indigo-500 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 dark:bg-indigo-500 dark:shadow-none dark:hover:bg-indigo-400 dark:focus-visible:outline-indigo-500">Get Notifications</button>

        <QueryNotification ref="notification" />
    </div>
</template>

<script setup lang="ts">
import Navbar from "~/components/Navbar.vue";
import { api } from "../composables/api"
import QueryNotification from "~/components/Notifications/QueryNotification.vue";

const username = ref("");
api.Identity_GetInfo(undefined)
.then((user) => {
    username.value = user.username ?? "Failed for some reason";
})
.catch(() => {
    username.value = "Not authorized";
});

const notification = ref();
const router = useRouter();
function matchmake() {
    api.Chess_Matchmake(undefined)
    .then(lobbyId => {
        router.push(`/chess/game/${lobbyId}`)
    });
}

</script>