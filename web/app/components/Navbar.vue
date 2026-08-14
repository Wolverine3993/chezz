<template>
  <Disclosure as="nav" class="relative bg-gray-800 dark:bg-gray-800/50 dark:after:pointer-events-none dark:after:absolute dark:after:inset-x-0 dark:after:bottom-0 dark:after:h-px dark:after:bg-white/10" v-slot="{ open }">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="relative flex h-16 items-center justify-between grid grid-cols-3">
        <div class="col-1" />
        <div class="flex flex-1 items-center justify-center sm:items-stretch col-2">
          <div @click="router.push('/')" class="flex shrink-0 items-center hover:cursor-pointer">
            <h1 class="font-bold text-xl dark:text-white m-1 select-none">Chezz</h1>
            <ChezzLogo class="h-8 w-auto text-blue-600" />
          </div>
        </div>
            <div v-if="signedIn">
                <div class="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0 col-3">
                <button type="button" class="relative rounded-full p-1 text-gray-400 focus:outline-2 focus:outline-offset-2 focus:outline-indigo-500 dark:hover:text-white">
                    <span class="absolute -inset-1.5"></span>
                    <span class="sr-only">View notifications</span>
                    <BellIcon class="size-6" aria-hidden="true" />
                </button>

                <!-- Profile dropdown -->
                <Menu as="div" class="relative ml-3">
                    <MenuButton class="relative flex rounded-full focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500">
                    <span class="absolute -inset-1.5"></span>
                    <span class="sr-only">Open user menu</span>
                    <img class="size-8 rounded-full bg-gray-800 outline -outline-offset-1 outline-white/10" src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80" alt="" />
                    </MenuButton>

                    <transition enter-active-class="transition ease-out duration-100" enter-from-class="transform opacity-0 scale-95" enter-to-class="transform scale-100" leave-active-class="transition ease-in duration-75" leave-from-class="transform scale-100" leave-to-class="transform opacity-0 scale-95">
                    <MenuItems class="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg outline outline-black/5 dark:bg-gray-800 dark:shadow-none dark:-outline-offset-1 dark:outline-white/10">
                        <MenuItem v-slot="{ active }">
                        <a href="#" @click="router.push('friends')" :class="[active ? 'bg-gray-100 outline-hidden dark:bg-white/5' : '', 'block px-4 py-2 text-sm text-gray-700 dark:text-gray-300']">Friends</a>
                        </MenuItem>
                        <MenuItem v-slot="{ active }">
                        <a href="#" @click="signOut" :class="[active ? 'bg-gray-100 outline-hidden dark:bg-white/5' : '', 'block px-4 py-2 text-sm text-gray-700 dark:text-gray-300']">Sign out</a>
                        </MenuItem>
                    </MenuItems>
                    </transition>
                </Menu>
                </div>
            </div>
            <div v-else-if="signedIn == false">
              <a href="#" @click="router.push('signin')" class="max-w-25 -mx-3 block rounded-lg px-3 py-2.5 text-base/7 font-semibold text-gray-900 hover:bg-gray-50 dark:text-white dark:hover:bg-white/5">Log in <span aria-hidden="true">&rarr;</span></a>
            </div>
        </div>
    </div>

  </Disclosure>
</template>

<script setup lang="ts">
import { Disclosure, Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/vue'
import { BellIcon } from '@heroicons/vue/24/outline'

const router = useRouter();
const signedIn: Ref<boolean | undefined> = ref(undefined);

api.Identity_GetInfo(undefined)
.then(() => {
    signedIn.value = true;
})
.catch(() => {
    signedIn.value = false;
})

async function signOut() {
    await api.Identity_Logout(undefined);

    router.push("signin");
}

</script>