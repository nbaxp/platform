<template>
  <el-menu
    :collapse="appStore.settings.isMenuCollapse"
    :collapse-transition="false"
    :default-active="$route.path"
    router
  >
    <menu-item
      v-for="item in items"
      :key="item.path"
      :node="item"
      :parent="parent"
    />
  </el-menu>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

import { useAppStore } from '../store/index.js'
import MenuItem from './menu-item.vue'

const appStore = useAppStore()
const route = useRoute()
const parent = computed(() => {
  return route.matched[0].path
})
const items = computed(() => {
  return route.matched[0].children
})
</script>
