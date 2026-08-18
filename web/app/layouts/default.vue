<template>
  <div class="w-full min-h-screen bg-zinc-950 flex flex-col">
    <Disclosure
      as="nav"
      class="relative bg-zinc-900 after:pointer-events-none after:absolute after:inset-x-0 after:bottom-0 after:h-px after:bg-white/10"
      v-slot="{ open }"
    >
      <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div class="flex h-16 justify-between">
          <div class="flex">
            <div class="mr-2 -ml-2 flex items-center md:hidden">
              <!-- Mobile menu button -->
              <DisclosureButton
                class="relative inline-flex items-center justify-center rounded-md p-2 text-zinc-400 hover:bg-white/5 hover:text-white focus:outline-2 focus:-outline-offset-1 focus:outline-blue-500"
              >
                <span class="absolute -inset-0.5"></span>
                <span class="sr-only">Open main menu</span>
                <Bars3Icon
                  v-if="!open"
                  class="block size-6"
                  aria-hidden="true"
                />
                <XMarkIcon v-else class="block size-6" aria-hidden="true" />
              </DisclosureButton>
            </div>
            <div
              class="flex shrink-0 items-center gap-x-2 text-zinc-100 font-bold text-xl"
            >
              <ChezzLogo class="h-8 w-auto text-blue-600" />
              Chezz
            </div>
            <div class="hidden md:ml-6 md:flex md:items-center md:space-x-4">
              <NuxtLink
                v-for="(item, itemIdx) in navigation"
                :key="item.name"
                :href="item.href"
                :class="[
                  itemIdx == currentNavIndex
                    ? 'bg-zinc-950/50 text-white'
                    : 'text-zinc-300 hover:bg-white/5 hover:text-white',
                  'rounded-md px-3 py-2 text-sm font-medium',
                ]"
                :aria-current="itemIdx == currentNavIndex ? 'page' : undefined"
                >{{ item.name }}</NuxtLink
              >
            </div>
          </div>
          <div v-if="svg" class="flex items-center">
            <div class="shrink-0">
              <button
                type="button"
                @click="matchmake"
                class="relative inline-flex items-center gap-x-1.5 rounded-md bg-blue-500 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-400 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-500"
              >
                <PlayIcon class="-ml-0.5 size-5" aria-hidden="true" />
                Play
              </button>
            </div>
            <div class="hidden md:ml-4 md:flex md:shrink-0 md:items-center">
              <button
                type="button"
                @click="checkNotifications"
                class="relative rounded-full p-1 text-zinc-400 hover:text-white focus:outline-2 focus:outline-offset-2 focus:outline-blue-500"
              >
                <span class="absolute -inset-1.5"></span>
                <span class="sr-only">View notifications</span>
                <BellIcon class="size-6" aria-hidden="true" />
              </button>

              <!-- Profile dropdown -->
              <Menu as="div" class="relative ml-3">
                <MenuButton
                  class="relative flex rounded-full focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-500"
                >
                  <span class="absolute -inset-1.5"></span>
                  <span class="sr-only">Open user menu</span>
                  <img
                    class="size-8 rounded-full bg-zinc-800 outline -outline-offset-1 outline-white/10"
                    :src="svg"
                    alt=""
                  />
                </MenuButton>

                <transition
                  enter-active-class="transition ease-out duration-200"
                  enter-from-class="transform opacity-0 scale-95"
                  enter-to-class="transform scale-100"
                  leave-active-class="transition ease-in duration-75"
                  leave-from-class="transform scale-100"
                  leave-to-class="transform opacity-0 scale-95"
                >
                  <MenuItems
                    class="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-zinc-800 py-1 outline-1 -outline-offset-1 outline-white/10"
                  >
                    <MenuItem
                      v-for="item in userNavigation"
                      :key="item.name"
                      v-slot="{ active }"
                    >
                      <a
                        href="#"
                        @click="item.onclick"
                        :class="[
                          active ? 'bg-white/5 outline-hidden' : '',
                          'block px-4 py-2 text-sm text-zinc-200',
                        ]"
                        >{{ item.name }}</a
                      >
                    </MenuItem>
                  </MenuItems>
                </transition>
              </Menu>
            </div>
          </div>
          <div v-else class="flex items-center">
            <ChezzButton to="/signin">Sign in &rarr;</ChezzButton>
          </div>
        </div>
      </div>

      <DisclosurePanel class="md:hidden">
        <div class="space-y-1 px-2 pt-2 pb-3 sm:px-3">
          <DisclosureButton
            v-for="(item, itemIdx) in navigation"
            :key="item.name"
            :as="NuxtLink"
            :href="item.href"
            :class="[
              itemIdx == currentNavIndex
                ? 'bg-zinc-950/50 text-white'
                : 'text-zinc-300 hover:bg-white/5 hover:text-white',
              'block rounded-md px-3 py-2 text-base font-medium',
            ]"
            :aria-current="itemIdx == currentNavIndex ? 'page' : undefined"
            >{{ item.name }}</DisclosureButton
          >
        </div>
        <div v-if="user && svg" class="border-t border-white/10 pt-4 pb-3">
          <div class="flex items-center px-5 sm:px-6">
            <div class="shrink-0">
              <img
                class="size-10 rounded-full bg-zinc-800 outline -outline-offset-1 outline-white/10"
                :src="svg"
                alt=""
              />
            </div>
            <div class="ml-3">
              <div class="text-base font-medium text-white">
                {{ user.username }}
              </div>
              <div class="text-sm font-medium text-zinc-400">
                {{ user.email }}
              </div>
            </div>
            <button
              type="button"
              @click="checkNotifications"
              class="relative ml-auto shrink-0 rounded-full p-1 text-zinc-400 hover:text-white focus:outline-2 focus:outline-offset-2 focus:outline-blue-500"
            >
              <span class="absolute -inset-1.5"></span>
              <span class="sr-only">View notifications</span>
              <BellIcon class="size-6" aria-hidden="true" />
            </button>
          </div>
          <div class="mt-3 space-y-1 px-2 sm:px-3">
            <DisclosureButton
              v-for="item in userNavigation"
              :key="item.name"
              as="a"
              href="#",
              @click="item.onclick"
              class="block rounded-md px-3 py-2 text-base font-medium text-zinc-400 hover:bg-white/5 hover:text-white"
              >{{ item.name }}</DisclosureButton
            >
          </div>
        </div>
        <div v-else class="border-t border-white/10 pt-4 pb-3 px-4">
          <ChezzButton to="/signin">Sign in &rarr;</ChezzButton>
        </div>
      </DisclosurePanel>
    </Disclosure>

    <div class="flex grow">
      <slot />
    </div>

    <QueryNotification ref="notificationRef" />
  </div>
</template>

<script setup lang="ts">
import {
  Disclosure,
  DisclosureButton,
  DisclosurePanel,
  Menu,
  MenuButton,
  MenuItem,
  MenuItems,
} from "@headlessui/vue";
import { Bars3Icon, BellIcon, XMarkIcon } from "@heroicons/vue/24/outline";
import { PlayIcon } from "@heroicons/vue/20/solid";
import { NuxtLink } from "#components";
import QueryNotification from "~/components/Notifications/QueryNotification.vue";

const { user, svg } = await useUser();

const notificationRef = ref();

function checkNotifications() {
  notificationRef.value?.pollNotifications();
}

if (user.value) {
  const notificationSocket = createWebsocket(
    "ws://localhost:5281/api/notificationList/ws",
  );
  const removeNotifyListener = notificationSocket.addListener(async () => {
    checkNotifications();
  });
  onScopeDispose(() => removeNotifyListener());
}

const navigation = [
  { name: "Home", href: "/" },
  { name: "Friends", href: "/friends" },
  { name: "Play Chess", href: "/chess" },
];

const route = useRoute();
const router = useRouter();

function matchmake() {
  api.Chess_Matchmake(undefined).then((lobbyId) => {
    router.push(`/chess/game/${lobbyId}`);
  });
}

const currentNavIndex = computed(() => {
  let choice = 0;
  let choiceLength = 0;
  for (let i = 0; i < navigation.length; i++) {
    const href = navigation[i]!.href;
    if (route.path.startsWith(href) && choiceLength < href.length) {
      choice = i;
      choiceLength = href.length;
    }
  }

  return choice;
});

const userNavigation = [
  { name: "Sign out", onclick: signout },
];

async function signout() {
  await api.Identity_Logout(undefined);
  router.push("/signin");
}
</script>
