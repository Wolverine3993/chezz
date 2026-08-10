<template>
    <h1>Hello there</h1>
    <button @click="matchmake">Matchmake</button>
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
</script>