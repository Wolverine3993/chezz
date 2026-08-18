<template>
    <div class="flex min-h-screen bg-zinc-900 flex-1">
        <div class="flex flex-1 flex-col justify-center px-4 py-12 sm:px-6 lg:flex-none lg:px-20 xl:px-24">
            <div class="mx-auto w-full max-w-sm lg:w-96">
                <div>
                    <ChezzLogo class="h-10 w-auto text-blue-600" />
                    <h2 class="mt-8 text-2xl/9 font-bold tracking-tight text-white">Create your account</h2>
                    <p class="mt-2 text-sm/6 text-zinc-400">
                        Already have one?
                        {{ ' ' }}
                        <NuxtLink href="/signin" class="font-semibold text-blue-400 hover:text-blue-300">Sign in &rarr;
                        </NuxtLink>
                    </p>
                </div>

                <div class="mt-10">
                    <div>
                        <form @submit.prevent="signin" class="space-y-6">
                            <ErrorAlert :messages="errors" />
                            <div>
                                <label for="username" class="block text-sm/6 font-medium text-zinc-100">Username</label>
                                <div class="mt-2">
                                    <input v-model="username" type="username" name="username" id="username" autocomplete="username" required
                                        class="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-zinc-500 focus:outline-2 focus:-outline-offset-2 focus:outline-blue-500 sm:text-sm/6" />
                                </div>
                            </div>

                            <div>
                                <label for="email" class="block text-sm/6 font-medium text-zinc-100">Email
                                    address</label>
                                <div class="mt-2">
                                    <input v-model="email" type="email" name="email" id="email" autocomplete="email" required
                                        class="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-zinc-500 focus:outline-2 focus:-outline-offset-2 focus:outline-blue-500 sm:text-sm/6" />
                                </div>
                            </div>

                            <div>
                                <label for="password" class="block text-sm/6 font-medium text-zinc-100">Password</label>
                                <div class="mt-2">
                                    <input v-model="password" type="password" name="password" id="password" autocomplete="current-password"
                                        required
                                        class="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-zinc-500 focus:outline-2 focus:-outline-offset-2 focus:outline-blue-500 sm:text-sm/6" />
                                </div>
                            </div>

                            <div>
                                <label for="registrationCode" class="block text-sm/6 font-medium text-zinc-100">Registration Code</label>
                                <div class="mt-2">
                                    <input v-model="registrationCode" type="registrationCode" name="registrationCode" id="registrationCode" autocomplete="registrationCode"
                                        class="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-zinc-500 focus:outline-2 focus:-outline-offset-2 focus:outline-blue-500 sm:text-sm/6" />
                                </div>
                            </div>

                            <div>
                                <ChezzButton type="submit" :loading="loading">Create account</ChezzButton>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <div class="relative hidden w-0 flex-1 lg:block">
            <img class="absolute inset-0 size-full object-cover" src="@/assets/background.jpg" alt="" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { api } from "~/composables/api";
import { useUser } from "~/composables/user";
definePageMeta({
    layout: false,
})

const username = ref();
const email = ref();
const password = ref();
const registrationCode = ref();
const loading = ref(false);
const errors = ref<string[]>([]);

const user = useUser();
const router = useRouter();

async function signin() {
    loading.value = true;
    errors.value = [];
    try {
        await api.Identity_Register({ username: username.value, email: email.value, password: password.value, registerCode: registrationCode.value ?? "" });
        await user.login({ username: username.value, password: password.value });
        router.push("/signin");
    } catch(e) {
        errors.value = extractApiErrors(e);
    }
    
    finally {
        loading.value = false;
    }
}
</script>