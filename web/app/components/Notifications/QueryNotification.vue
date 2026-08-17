<template>
  <!-- Global notification live region, render this permanently at the end of the document -->
  <div aria-live="assertive" class="pointer-events-none fixed inset-0 flex items-end px-4 py-6 sm:items-start sm:p-6">
    <div class="flex w-full flex-col items-center space-y-4 sm:items-end">
      <!-- Notification panel, dynamically insert this into the live region when it needs to be displayed -->
      <transition enter-active-class="transform ease-out duration-300 transition" enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2" enter-to-class="translate-y-0 sm:translate-x-0" leave-active-class="transition ease-in duration-100" leave-from-class="" leave-to-class="opacity-0">
        <div v-if="show" class="pointer-events-auto w-full max-w-sm rounded-lg bg-zinc-800 shadow-lg outline-1 -outline-offset-1 outline-white/10">
          <div class="p-4">
            <div class="flex items-start">
              <div class="shrink-0">
                <InboxIcon class="size-6 text-zinc-400" aria-hidden="true" />
              </div>
              <div class="ml-3 w-0 flex-1 pt-0.5">
                <p class="text-sm font-medium text-white">{{ title }}</p>
                <p class="mt-1 text-sm text-zinc-400">{{ content }}</p>
                <div class="mt-3 flex space-x-7">
                  <button @click="acceptRequest" :disabled="processing" type="button" class="rounded-md text-sm font-medium text-emerald-400 hover:text-emerald-300 focus:outline-2 focus:outline-offset-2 focus:outline-blue-500 disabled:cursor-not-allowed disabled:opacity-50">Accept</button>
                  <button @click="declineRequest" :disabled="processing" type="button" class="rounded-md text-sm font-medium text-zinc-400 hover:text-white focus:outline-2 focus:outline-offset-2 focus:outline-blue-500 disabled:cursor-not-allowed disabled:opacity-50">Decline</button>
                </div>
              </div>
              <div class="ml-4 flex shrink-0">
                <button type="button" @click="show = false" class="inline-flex rounded-md text-zinc-400 hover:text-white focus:outline-2 focus:outline-offset-2 focus:outline-blue-500">
                  <span class="sr-only">Close</span>
                  <XMarkIcon class="size-5" aria-hidden="true" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { InboxIcon } from '@heroicons/vue/24/outline'
import { XMarkIcon } from '@heroicons/vue/20/solid'

const notification = ref();

const title = computed(() => notification.value?.title);
const content = computed(() => notification.value?.content);

const show = ref(false);
const processing = ref(false);
let fetching = false;
let pollTimer: ReturnType<typeof setTimeout> | undefined;

const router = useRouter();

function schedulePoll() {
  if (pollTimer) clearTimeout(pollTimer);
  pollTimer = setTimeout(() => {
    pollTimer = undefined;
    pollNotifications();
  }, 1000);
}

async function pollNotifications() {
  // A notification is already visible or a fetch is in flight; retry once it clears.
  if (show.value || fetching) {
    schedulePoll();
    return;
  }
  fetching = true;
  try {
    notification.value = await api.Notification_GetNotifications(undefined);
    show.value = true;
  } catch {
    // A 404 means there are no pending notifications.
  } finally {
    fetching = false;
  }
}

async function acceptRequest() {
  if (processing.value || !notification.value) return;
  processing.value = true;
  try {
    if (notification.value.notificationType === 0) {
      await api.UserRelationship_AcceptFriendRequest({
        requestId: notification.value.callbackId,
      });
    } else if (notification.value.notificationType === 1) {
      await router.push(`/chess/game/${notification.value.callbackId}`);
    }
    show.value = false;
  } catch {
    // Keep the notification visible so the action can be retried.
  } finally {
    processing.value = false;
  }
}

async function declineRequest() {
  if (processing.value || !notification.value) return;
  processing.value = true;
  try {
    if (notification.value.notificationType === 0) {
      await api.UserRelationship_DeclineFriendRequest({
        requestId: notification.value.callbackId,
      });
    }
    show.value = false;
  } catch {
    // Keep the notification visible so the action can be retried.
  } finally {
    processing.value = false;
  }
}

onUnmounted(() => {
  if (pollTimer) clearTimeout(pollTimer);
});

defineExpose({
    pollNotifications
});
</script>