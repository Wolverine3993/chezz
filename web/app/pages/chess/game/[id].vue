<template>
  <div class="grow relative">
    <div
      class="absolute top-0 right-0 px-2 py-1 rounded-bl-sm z-50 bg-zinc-900 inline-flex items-center gap-x-2 text-sm text-zinc-400"
    >
      Connected?
      <div
        :class="[
          websocket.connected ? 'bg-green-600' : 'bg-red-600',
          'w-2 h-2 rounded-full',
        ]"
      />
    </div>

    <div
      v-if="currentState.status === 'lobby'"
      class="flex min-h-[calc(100vh-4rem)] flex-col items-center justify-center gap-4 text-center"
    >
      <div class="text-3xl font-semibold text-white">In Lobby</div>
      <label for="private" class="flex items-center gap-2 text-sm text-zinc-300">
        <input
          id="private"
          type="checkbox"
          v-model="check"
          @change="changePrivacy"
        />
        Private
      </label>
    </div>

    <div v-if="currentState.status === 'game'" class="grid grid-cols-4 gap-8 p-8">
      <div>
        <div class="flow-root rounded-md bg-zinc-900 p-3">
          <h1 class="text-base font-semibold text-white">Moves</h1>
          <div class="mt-2 max-h-[28rem] overflow-y-auto">
            <table class="relative min-w-full divide-y divide-white/15">
              <thead>
                <tr>
                  <th
                    scope="col"
                    class="py-2 pr-3 pl-3 text-left text-sm font-semibold text-white sm:pl-3"
                  >
                    #
                  </th>
                  <th
                    scope="col"
                    class="px-3 py-2 text-left text-sm font-semibold text-white"
                  >
                    White
                  </th>
                  <th
                    scope="col"
                    class="px-3 py-2 text-left text-sm font-semibold text-white"
                  >
                    Black
                  </th>
                </tr>
              </thead>
              <tbody class="bg-zinc-900">
                <tr
                  v-for="row in moveHistoryRows"
                  :key="row.number"
                  class="even:bg-zinc-800/50"
                >
                  <td
                    class="py-1.5 pr-3 pl-3 text-sm font-medium whitespace-nowrap text-zinc-500 tabular-nums sm:pl-3"
                  >
                    {{ row.number }}
                  </td>
                  <td
                    class="px-3 py-1.5 text-sm whitespace-nowrap text-white"
                  >
                    {{ row.white }}
                  </td>
                  <td
                    class="px-3 py-1.5 text-sm whitespace-nowrap text-zinc-400"
                  >
                    {{ row.black ?? "" }}
                  </td>
                </tr>
                <tr v-if="!moveHistoryRows.length">
                  <td
                    colspan="3"
                    class="px-3 py-1.5 text-sm whitespace-nowrap text-zinc-400"
                  >
                    No moves yet
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <div class="col-span-2">
        <div v-if="topPlayer" class="mb-3 flex items-center gap-x-3">
          <img
            :src="topPlayer.imageUrl"
            alt=""
            class="size-10 rounded-full bg-zinc-800 outline -outline-offset-1 outline-white/10"
          />
          <div>
            <div class="text-sm font-semibold text-white">
              {{ topPlayer.name }}
            </div>
            <div class="text-xs text-zinc-400">ELO {{ topPlayer.elo }}</div>
          </div>
        </div>
        <div v-if="board" class="grid grid-cols-8 w-full">
          <div
            v-for="(col, colIdx) in viewColour === 'White'
              ? board
              : board.toReversed()"
            class="flex flex-col"
          >
            <div
              v-for="(square, rowIdx) in viewColour === 'White'
                ? col
                : col.toReversed()"
              :class="[
                'relative w-full aspect-square flex items-center justify-center',
                (colIdx + rowIdx) % 2 === 0
                  ? 'bg-[#8f9cba]'
                  : 'bg-[#2c3654]',
                selectedView &&
                selectedView.colIdx === colIdx &&
                selectedView.rowIdx === rowIdx
                  ? 'ring-4 ring-inset ring-sky-400/80'
                  : '',
              ]"
              @click="selectPiece(colIdx, rowIdx)"
              @dragover="(e) => onSquareDragOver(e, colIdx, rowIdx)"
              @drop="(e) => onSquareDrop(e, colIdx, rowIdx)"
            >
              <img
                v-if="square"
                :src="square.imageUrl"
                :draggable="isDraggable(colIdx, rowIdx)"
                @dragstart="(e) => onPieceDragStart(e, colIdx, rowIdx)"
                @dragend="onPieceDragEnd"
                :class="[
                  'w-full h-full object-contain select-none',
                  isDraggable(colIdx, rowIdx)
                    ? 'cursor-grab active:cursor-grabbing'
                    : '',
                  isDragSource(colIdx, rowIdx) ? 'opacity-40' : '',
                ]"
              />
              <template v-if="squareAt(colIdx, rowIdx)">
                <div
                  v-if="isPickerOpen(colIdx, rowIdx)"
                  @click.stop
                  class="absolute inset-0 z-50 flex items-center justify-center bg-zinc-900/80"
                >
                  <div class="flex flex-wrap gap-0.5 w-full justify-center">
                    <button
                      v-for="m in squareAt(colIdx, rowIdx)?.moves ?? []"
                      :key="m.moveId"
                      @click.stop="(e) => makeMove(e, m.moveId)"
                      class="size-7 bg-zinc-100 rounded hover:bg-white flex items-center justify-center"
                    >
                      <img
                        v-if="m.promotionPiece"
                        :src="m.promotionPiece.imageUrl"
                        class="size-7"
                      />
                    </button>
                  </div>
                </div>
                <div
                  v-else
                  @click.stop="(e) => onDotClick(e, colIdx, rowIdx)"
                  class="absolute inset-0 flex items-center justify-center z-50"
                >
                  <div
                    class="size-[30%] rounded-full bg-black/20"
                  />
                </div>
              </template>
            </div>
          </div>
        </div>
        <div v-if="bottomPlayer" class="mt-3 flex items-center gap-x-3">
          <img
            :src="bottomPlayer.imageUrl"
            alt=""
            class="size-10 rounded-full bg-zinc-800 outline -outline-offset-1 outline-white/10"
          />
          <div>
            <div class="text-sm font-semibold text-white">
              {{ bottomPlayer.name }}
            </div>
            <div class="text-xs text-zinc-400">ELO {{ bottomPlayer.elo }}</div>
          </div>
        </div>
      </div>

      <div>
        <ChezzButton @click="viewColour = 'White'">View as white</ChezzButton>
        <ChezzButton @click="viewColour = 'Black'">View as black</ChezzButton>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { unpackBoard } from "~/composables/chess";

const route = useRoute();

type GameState =
  | { status: "lobby"; playerUsernames: string[] }
  | {
      status: "game";
      playerUsernames: string[];
      gameId: string;
      gameState?: Awaited<ReturnType<typeof api.Chess_GetGameStatus>>;
    };
type ViewColour = "White" | "Black";
type ChessMove = Awaited<ReturnType<typeof api.Chess_GetMoves>>[number];
type PromotionPiece = Extract<
  ChessMove,
  { kind: "Promotion" }
>["promotionPiece"];

const id = route.params.id!.toString();

const currentState = ref<GameState>({ status: "lobby", playerUsernames: [] });
const viewColour = ref<ViewColour>("White");
const currentMoves = ref<Array<ChessMove>>([]);
const currentSelectedPiece = ref<ChessMove["from"] | null>(null);
const promotionPickerAt = ref<{ colIdx: number; rowIdx: number } | null>(null);
const board = computed(() =>
  currentState.value.status === "game" && currentState.value.gameState
    ? unpackBoard(currentState.value.gameState.packedBoard)
    : undefined,
);

const bottomPlayer = computed(() => {
  if (currentState.value.status !== "game") return null;
  const gs = currentState.value.gameState;
  if (!gs) return null;
  return viewColour.value === "White" ? gs.whitePlayer : gs.blackPlayer;
});

const topPlayer = computed(() => {
  if (currentState.value.status !== "game") return null;
  const gs = currentState.value.gameState;
  if (!gs) return null;
  return viewColour.value === "White" ? gs.blackPlayer : gs.whitePlayer;
});

const moveHistoryRows = computed(() => {
  const moves =
    currentState.value.status === "game"
      ? (currentState.value.gameState?.moveHistory ?? [])
      : [];
  const rows: Array<{ number: number; white: string; black: string | null }> =
    [];
  for (let i = 0; i < moves.length; i += 2) {
    rows.push({
      number: i / 2 + 1,
      white: moves[i]!,
      black: moves[i + 1] ?? null,
    });
  }
  return rows;
});

const check = ref();
api.Chess_GetLobbyPrivacy({ params: { lobbyId: id } }).then((isPrivate) => {
  check.value = isPrivate;
});

type HighlightSquare = {
  colIdx: number;
  rowIdx: number;
  isPromotion: boolean;
  moves: Array<{ moveId: string; promotionPiece: PromotionPiece | null }>;
};

const highlightMoves = computed<Array<HighlightSquare> | null>(() => {
  const selected = currentSelectedPiece.value;
  if (selected === null) return null;
  const bySquare = new Map<string, HighlightSquare>();
  for (const v of currentMoves.value) {
    if (v.from.x !== selected.x || v.from.y !== selected.y) continue;
    const [colIdx, rowIdx] = boardToView(v.to.x, v.to.y);
    const key = `${colIdx},${rowIdx}`;
    let entry = bySquare.get(key);
    if (entry === undefined) {
      entry = { colIdx, rowIdx, isPromotion: false, moves: [] };
      bySquare.set(key, entry);
    }
    if (v.kind === "Promotion") {
      entry.isPromotion = true;
      entry.moves.push({ moveId: v.moveId, promotionPiece: v.promotionPiece });
    } else {
      entry.moves.push({ moveId: v.moveId, promotionPiece: null });
    }
  }
  return Array.from(bySquare.values());
});

function squareAt(colIdx: number, rowIdx: number): HighlightSquare | null {
  return (
    highlightMoves.value?.find(
      (v) => v.colIdx === colIdx && v.rowIdx === rowIdx,
    ) ?? null
  );
}

function isPickerOpen(colIdx: number, rowIdx: number): boolean {
  const picker = promotionPickerAt.value;
  return (
    picker !== null && picker.colIdx === colIdx && picker.rowIdx === rowIdx
  );
}

function onDotClick(e: Event, colIdx: number, rowIdx: number) {
  e.preventDefault();
  const square = squareAt(colIdx, rowIdx);
  if (square === null) return;
  if (square.isPromotion) {
    promotionPickerAt.value = { colIdx, rowIdx };
    return;
  }
  makeMove(e, square.moves[0]?.moveId ?? null);
}

const selectedView = computed<{ colIdx: number; rowIdx: number } | null>(() => {
  const selected = currentSelectedPiece.value;
  if (selected === null) return null;
  const [colIdx, rowIdx] = boardToView(selected.x, selected.y);
  return { colIdx, rowIdx };
});

const websocket = createWebsocket(
  `ws://localhost:5281/api/games/chess/lobby/${id}/ws`,
);
const removeListener = websocket.addListener(async () => {
  if (currentState.value.status === "lobby") {
    const lobbyStatus = await api.Chess_GetLobbyStatus({
      params: { lobbyId: id },
    });
    if (lobbyStatus.playerUsernames) {
      currentState.value.playerUsernames = lobbyStatus.playerUsernames;
    }
    if (lobbyStatus.gameId) {
      const oldStatus = currentState.value;
      currentState.value = {
        status: "game",
        playerUsernames: oldStatus.playerUsernames,
        gameId: lobbyStatus.gameId,
      };
    }
  }

  if (currentState.value.status === "game") {
    const gameState = await api.Chess_GetGameStatus({
      params: { gameId: currentState.value.gameId },
    });
    currentState.value.gameState = gameState;
    viewColour.value = gameState.yourColor;
    if (currentState.value.gameState.yourTurn) {
      currentMoves.value = await api.Chess_GetMoves({
        params: { gameId: currentState.value.gameId },
      });
    } else {
      currentMoves.value = [];
    }
  }
});

onScopeDispose(() => removeListener());

function viewToBoard(colIdx: number, rowIdx: number): [number, number] {
  if (viewColour.value === "Black") return [7 - colIdx, 7 - rowIdx];
  return [colIdx, rowIdx];
}

function boardToView(x: number, y: number): [number, number] {
  return viewToBoard(x, y);
}

function selectPiece(colIdx: number, rowIdx: number) {
  const [x, y] = viewToBoard(colIdx, rowIdx);
  promotionPickerAt.value = null;
  if (
    currentSelectedPiece.value?.x === x &&
    currentSelectedPiece.value.y === y
  ) {
    currentSelectedPiece.value = null;
  } else {
    currentSelectedPiece.value = { x, y };
  }
}

const isDragging = ref(false);

const movableFromKeys = computed(() => {
  const keys = new Set<string>();
  for (const m of currentMoves.value) {
    const [fromCol, fromRow] = boardToView(m.from.x, m.from.y);
    keys.add(`${fromCol},${fromRow}`);
  }
  return keys;
});

function isDraggable(colIdx: number, rowIdx: number): boolean {
  return movableFromKeys.value.has(`${colIdx},${rowIdx}`);
}

function isDragSource(colIdx: number, rowIdx: number): boolean {
  return (
    isDragging.value &&
    selectedView.value?.colIdx === colIdx &&
    selectedView.value?.rowIdx === rowIdx
  );
}

function onPieceDragStart(e: DragEvent, colIdx: number, rowIdx: number) {
  const [x, y] = viewToBoard(colIdx, rowIdx);
  promotionPickerAt.value = null;
  currentSelectedPiece.value = { x, y };
  isDragging.value = true;
  if (e.dataTransfer) {
    e.dataTransfer.effectAllowed = "move";
    e.dataTransfer.setData("text/plain", `${x},${y}`);
  }
}

function onSquareDragOver(e: DragEvent, colIdx: number, rowIdx: number) {
  if (squareAt(colIdx, rowIdx) !== null) e.preventDefault();
}

function onSquareDrop(e: DragEvent, colIdx: number, rowIdx: number) {
  e.preventDefault();
  isDragging.value = false;
  const square = squareAt(colIdx, rowIdx);
  if (square === null) return;
  if (square.isPromotion) {
    promotionPickerAt.value = { colIdx, rowIdx };
    return;
  }
  makeMove(e, square.moves[0]?.moveId ?? null);
}

function onPieceDragEnd() {
  isDragging.value = false;
}

async function makeMove(e: Event, moveId: string | null) {
  e.preventDefault();
  if (moveId === null) return;
  if (currentState.value.status !== "game") return;
  if (currentSelectedPiece.value === null) return;
  await api.Chess_MakeMove(
    { moveId },
    { params: { gameId: currentState.value.gameId } },
  );
  promotionPickerAt.value = null;
  currentSelectedPiece.value = null;
}

async function changePrivacy() {
  await api.Chess_ChangeLobbyPrivacy(undefined, {
    queries: { isPrivate: check.value },
    params: { lobbyId: id },
  });
  check.value = await api.Chess_GetLobbyPrivacy({ params: { lobbyId: id } });
}
</script>
