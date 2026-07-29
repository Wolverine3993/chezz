<template>
    {{ websocket.connected }}

    <div v-if="currentState.status === 'game'">
        <div v-if="currentState.gameState?.board" class="grid grid-cols-8 w-fit ">
            <div v-for="(col, colIdx) in viewColour === 'white' ? currentState.gameState.board : currentState.gameState.board.toReversed()" class="grid grid-rows-8 w-fit">
                <button @click="console.log(transformViewClick(colIdx, rowIdx))" v-for="(square, rowIdx) in viewColour === 'white' ? col : col.toReversed()"
                    :class="['w-16 h-16 text-center text-sm text-blue-600', (colIdx * 7 + rowIdx + (viewColour === 'white' ? 0 : 1)) % 2 == 0 ? 'bg-olive-300' : 'bg-olive-700']">
                    {{ square?.type }} {{ square?.color }}
                </button>
            </div>
        </div>

        <div>
            <button @click="viewColour = 'white'">View as white</button>
            <button @click="viewColour = 'black'">View as black</button>
        </div>
    </div>
</template>

<script setup lang="ts">
const route = useRoute();

type GameState = { status: "lobby", playerUsernames: string[] } | { status: "game", playerUsernames: string[], gameId: string, gameState?: Awaited<ReturnType<typeof api.Chess_GetGameStatus>> };
type ViewColour = "white" | "black";

const id = route.params.id!.toString();

const currentState = ref<GameState>({ status: "lobby", playerUsernames: [] });
const viewColour = ref<ViewColour>("black");

// Nitro's devProxy can't forward WebSocket upgrades (ws:true is only honoured
// for HTTP), so connect straight to the backend. localhost:5281 is same-site
// with the dev server, so the SameSite=Lax auth cookie is still sent.
// TODO: once the devProxy ws fix ships, switch back to a same-origin /api URL.
//   Issue: https://github.com/nitrojs/nitro/issues/4269
//   Fix PR: https://github.com/nitrojs/nitro/pull/4480
const websocket = createWebsocket(
    `ws://localhost:5281/api/games/chess/lobby/${id}/ws`,
);
websocket.addListener(async () => {
    if (currentState.value.status === "lobby") {
        const lobbyStatus = await api.Chess_GetLobbyStatus({ params: { lobbyId: id } });
        if (lobbyStatus.playerUsernames) {
            currentState.value.playerUsernames = lobbyStatus.playerUsernames;
        }
        if (lobbyStatus.gameId) {
            const oldStatus = currentState.value;
            currentState.value = { status: "game", playerUsernames: oldStatus.playerUsernames, gameId: lobbyStatus.gameId, };
            console.log("switched to new");
        }
        return;
    }

    if (currentState.value.status === "game") {
        const gameState = await api.Chess_GetGameStatus({ params: { gameId: currentState.value.gameId } });
        currentState.value.gameState = gameState;
        console.log(currentState.value);
    }
});

function transformViewClick(rowIdx: number, colIdx: number): [number, number] {
    if(viewColour.value === "black") return [7 - rowIdx, 7 - colIdx];
    return [rowIdx, colIdx]
}
</script>