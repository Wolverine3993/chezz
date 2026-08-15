<template>
  <!--
    This example requires updating your template:

    ```
    <html class="h-full">
    <body class="h-full">
    ```
  -->
  <div class="min-h-screen bg-zinc-900">
    <Navbar />

    <div class="py-5">
      <main>
        <div class="mx-auto max-w-7xl min-w-screen px-4 sm:px-6 lg:px-8">
          <div class="grid-cols-2 flex">
            <div class="col-1 mx-5 flex-1">
                <div class="grid-cols-2 flex content-start">
                    <h1 class="text-3xl font-bold tracking-tight text-white m-5 col-1 flex-1">Friends</h1>
                    <UserSearch class="col-2 flex-4"/>
                </div>
                <List :values="listNames" :type="'Friends'"/>
                <Footer @next-pressed="nextPage('Friends')" @prev-pressed="prevPage('Friends')" :page="friendsPage" :count="testProps.values.length"/>
            </div>
            <div class="col-2 mx-5 flex-1">
                <h1 class="text-3xl font-bold tracking-tight text-white m-5">Friend Requests</h1>
                <List />
                <Footer />
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">

const requestArray = ref();
api.UserRelationship_GetFriendRequests(undefined)
.then((res) => {
    requestArray.value = res;
});

const testProps = {
    values: [
        "a",
        "john",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
    ],
    type: "Friends"
}

const friendsPage = ref(0);
let requestsPage = 0;

const listNames = ref(testProps.values.slice(0, 5));

function nextPage(window: "Friends" | "Requests") {
    if (window === "Friends") {
        let listLength = Math.floor(testProps.values.length / 5);
        if (listLength <= friendsPage.value) return;

        friendsPage.value += 1;
        listNames.value = testProps.values.slice(5 * friendsPage.value, 5 * friendsPage.value + 5);
    }
}

function prevPage(window: "Friends" | "Requests") {
    if (window === "Friends") {
        if (friendsPage.value == 0) return;

        friendsPage.value -= 1;
        listNames.value = testProps.values.slice(5 * friendsPage.value, 5 * friendsPage.value + 5);
    }
}
</script>