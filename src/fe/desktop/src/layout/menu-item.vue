<template>
  <template v-if="!node.meta?.hideInMenu && hasPermission">
    <el-sub-menu
      v-if="node.children && node.children.some(o => !o.meta?.hideInMenu)"
      :index="path"
    >
      <template #title>
        <svg-icon :name="node.meta.icon ?? 'folder'" />
        <span>{{ $t(camelCase(node.meta?.title)) }}</span>
      </template>
      <template v-for="item in node.children" :key="item.path">
        <menu-item :node="item" :parent="path" />
      </template>
    </el-sub-menu>
    <el-menu-item v-else :index="path" @click="click(node, $event)">
      <svg-icon :name="node.meta.icon ?? 'file'" />
      <template #title>
        <span>{{ $t(camelCase(node.meta?.title)) }}</span>
      </template>
    </el-menu-item>
  </template>
</template>

<script setup>
import { camelCase } from '@/utils/index.js'
import { ElMessageBox } from 'element-plus'
import { computed } from 'vue'

import SvgIcon from '@/components/icon/index.vue'
import { useAppStore, useTabsStore, useUserStore } from '@/store/index.js'

const props = defineProps({
  node: {
    type: Object,
    default: null,
  },
  parent: {
    type: String,
    default: '',
  },
})

const appStore = useAppStore()
const tabsStore = useTabsStore()
const userStore = useUserStore()

const hasPermission = computed(() => {
  return userStore.hasPermission(props.node.meta.permission)
})

const path = computed(() => {
  return (
    props.parent + (props.parent.endsWith('/') ? '' : '/') + props.node.path
  )
})

const click = (route, event) => {
  if (!route.path.startsWith('http')) {
    if (
      appStore.settings.useTabs &&
      tabsStore.routes.length >= (appStore.settings.maxTabs ?? 10)
    ) {
      ElMessageBox.alert(
        `页签达到最大限制${appStore.settings.maxTabs ?? 10},请关闭不再使用的页签`,
        `提示`,
      )
      event.preventDefault()
    }
  } else {
    event.preventDefault()
    window.open(props.node.path)
  }
}
</script>
