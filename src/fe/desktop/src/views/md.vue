<template>
  <md :name="name" />
</template>

<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'

import Md from '@/components/markdown/index.vue'

const name = ref(useRoute().meta.file)
</script>
