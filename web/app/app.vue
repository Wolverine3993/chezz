<template>
  <div>
{{ thingy }}
  </div>
</template>

<script setup lang="ts">
import { createApiClient } from "~~/composables/api";

const client = createApiClient("/");

const thingy = await client.GetWeatherForecast();
</script>
