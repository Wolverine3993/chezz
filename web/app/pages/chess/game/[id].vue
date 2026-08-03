<template>
    {{ websocket.connected }}

    <div v-if="currentState.status === 'game'">
        <div v-if="currentState.gameState?.board" class="grid grid-cols-8 w-fit ">
            <div v-for="(col, colIdx) in viewColour === 'White' ? currentState.gameState.board : currentState.gameState.board.toReversed()"
                class="grid grid-rows-8 w-fit">
                <div v-for="(square, rowIdx) in viewColour === 'White' ? col : col.toReversed()"
                    :class="['relative w-16 h-16 flex items-center justify-center',
                        (colIdx * 7 + rowIdx + (viewColour === 'White' ? 0 : 1)) % 2 == 0 ? 'bg-olive-300' : 'bg-olive-700',
                        selectedView && selectedView.colIdx === colIdx && selectedView.rowIdx === rowIdx ? 'bg-olive-900/30' : '']" @click="selectPiece(colIdx, rowIdx)">
                    <img v-if="square" :src="square.imageUrl" class="w-16 h-16" />
                    <div v-if="moveIdAt(colIdx, rowIdx)" @click="(e) => makeMove(e, moveIdAt(colIdx, rowIdx))"
                        class="absolute inset-0 flex items-center justify-center z-50">
                        <div class="size-2 bg-olive-700 ring-2 ring-olive-300 rounded-full" />
                    </div>
                </div>
            </div>
        </div>

        <div>
            <button @click="viewColour = 'White'">View as white</button>
            <button @click="viewColour = 'Black'">View as black</button>

            <br />
            {{ highlightMoves }}
            <br />
            {{ currentMoves }}
            <br />
            {{ currentSelectedPiece }}
        </div>
    </div>
</template>

<script setup lang="ts">
const route = useRoute();

type GameState = { status: "lobby", playerUsernames: string[] } | { status: "game", playerUsernames: string[], gameId: string, gameState?: Awaited<ReturnType<typeof api.Chess_GetGameStatus>> };
type ViewColour = "White" | "Black";
type ChessMove = Awaited<ReturnType<typeof api.Chess_GetMoves>>[number];

const id = route.params.id!.toString();

const currentState = ref<GameState>({ status: "lobby", playerUsernames: [] });
const viewColour = ref<ViewColour>("White");
const currentMoves = ref<Array<ChessMove>>([]);
const currentSelectedPiece = ref<ChessMove["from"] | null>(null);

const highlightMoves = computed<Array<{ colIdx: number, rowIdx: number, moveId: string }> | null>(() => {
    const selected = currentSelectedPiece.value;
    if (selected === null) return null;
    return currentMoves.value
        .filter((v) => v.from.x === selected.x && v.from.y === selected.y)
        .map((v) => {
            const [colIdx, rowIdx] = boardToView(v.to.x, v.to.y);
            return { colIdx, rowIdx, moveId: v.moveId };
        });
})

function moveIdAt(colIdx: number, rowIdx: number): string | null {
    return highlightMoves.value?.find((v) => v.colIdx === colIdx && v.rowIdx === rowIdx)?.moveId ?? null;
}

const selectedView = computed<{ colIdx: number, rowIdx: number } | null>(() => {
    const selected = currentSelectedPiece.value;
    if (selected === null) return null;
    const [colIdx, rowIdx] = boardToView(selected.x, selected.y);
    return { colIdx, rowIdx };
})

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
        }
        return;
    }

    if (currentState.value.status === "game") {
        const gameState = await api.Chess_GetGameStatus({ params: { gameId: currentState.value.gameId } });
        currentState.value.gameState = gameState;
        viewColour.value = gameState.yourColor;
        currentMoves.value = await api.Chess_GetMoves({ params: { gameId: currentState.value.gameId } });
    }
});

function viewToBoard(colIdx: number, rowIdx: number): [number, number] {
    if (viewColour.value === "Black") return [7 - colIdx, 7 - rowIdx];
    return [colIdx, rowIdx];
}

function boardToView(x: number, y: number): [number, number] {
    return viewToBoard(x, y);
}

function selectPiece(colIdx: number, rowIdx: number) {
    const [x, y] = viewToBoard(colIdx, rowIdx);
    if (currentSelectedPiece.value?.x === x && currentSelectedPiece.value.y === y) {
        currentSelectedPiece.value = null;
    } else {
        currentSelectedPiece.value = { x, y };
    }
}

async function makeMove(e: Event, moveId: string | null) {
    e.preventDefault();
    if (moveId === null) return;
    if (currentState.value.status !== "game") return;
    if (currentSelectedPiece.value === null) return;
    await api.Chess_MakeMove({ moveId }, { params: { gameId: currentState.value.gameId } });
}
</script>