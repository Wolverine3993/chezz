<template>
    <div v-if="currentChessState == 'waiting'"
        class="grow grid grid-cols-2 divide-y-2 divide-zinc-700">
        <form @submit.prevent="createLobby" class="flex flex-col items-center justify-center">
            <div class="sm:mx-auto sm:w-full sm:max-w-sm">
                <ChezzLogo class="mx-auto h-10 w-auto text-blue-600" />
                <h2 class="mt-10 text-center text-2xl/9 font-bold tracking-tight text-white">Create a lobby
                </h2>
            </div>

            <div class="mt-10 space-y-6 sm:mx-auto sm:w-full sm:max-w-sm">

                <p class="block text-sm/6 font-medium text-gray-100 text-center">Lobby lobby lobby, lobby lobby lobby.
                    Lobby lobby
                    lboby.</p>
                <div>
                    <ChezzButton :loading="createLobbyLoading">Create lobby</ChezzButton>
                </div>

            </div>
        </form>
        <form @submit.prevent="joinLobby" class="flex flex-col items-center justify-center">
            <div class="sm:mx-auto sm:w-full sm:max-w-sm">
                <ChezzLogo class="mx-auto h-10 w-auto text-blue-600" />
                <h2 class="mt-10 text-center text-2xl/9 font-bold tracking-tight text-white">Join a lobby
                </h2>
            </div>

                    <div>
                        <label for="lobby" class="block text-sm/6 font-medium text-gray-100">Lobby ID</label>
                        <div class="mt-2">
                            <input type="text" name="lobby" id="lobby" required
                                placeholder="d5ece5ba-a2a4-43cf-9d16-b04c8893769a"
                                v-model="lobbyId"
                                class="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-gray-500 focus:outline-2 focus:-outline-offset-2 focus:outline-blue-500 sm:text-sm/6" />
                        </div>
                    </div>
                    <div>
                        <ChezzButton>Join Lobby</ChezzButton>
                    </div>
                </form>
    </div>
</template>

<script setup lang="ts">
import { ZodError } from 'zod';

type ChessState = "waiting" | "lobby" | "game";


const currentChessState = ref<ChessState>("waiting");

const router = useRouter();

const createLobbyLoading = ref(false);
async function createLobby() {
    createLobbyLoading.value = true;
    try {
        const lobbyID = await api.Chess_CreateLobby(undefined);
        router.push(`/chess/game/${lobbyID}`);
    } catch (e) {
        console.error(e);
    }
    createLobbyLoading.value = true;
}

const lobbyId = ref();
async function joinLobby(){
    const uuidRegex = /^([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-8][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}|00000000-0000-0000-0000-000000000000|ffffffff-ffff-ffff-ffff-ffffffffffff)$/
    if(!uuidRegex.test(lobbyId.value)) {
        console.log("Invalid UUID");
        return;
    }

    try {
        await api.Chess_GetLobbyStatus({params: {lobbyId: lobbyId.value}});
        router.push(`/chess/game/${lobbyId.value}`);
    } catch (e) {
        console.log("Lobby not found");
    }
}
</script>