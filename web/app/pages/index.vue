<template>
    {{ notifText }}
    <h1>Hello there</h1>
    <button @click="matchmake">Matchmake</button>
    <button @click="pollNotifications">Get Notifications</button>
</template>

<script setup lang="ts">
import { api } from "../composables/api"

const router = useRouter();
function matchmake() {
    api.Chess_Matchmake(undefined)
    .then(lobbyId => {
        router.push(`/chess/game/${lobbyId}`)
    });
}

const notifText = ref("Not received...");

async function pollNotifications() {
    let notification = await api.Notification_GetNotifications();
    notifText.value = notification.content ?? "Null value";
}
setTimeout
</script>