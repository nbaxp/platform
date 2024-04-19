<template>
  <el-tabs
    v-model="model"
    type="border-card"
    class="router-tabs"
    :closable="tabsStore.routes.length > 1"
    @tab-remove="remove"
    @tab-click="onClick"
  >
    <template v-for="(item, index) in tabsStore.routes" :key="item.fullPath">
      <el-tab-pane v-model="item.fullPath" :name="item.fullPath">
        <template #label>
          <el-dropdown
            :ref="el => setRef(index, el)"
            class="h-full"
            trigger="contextmenu"
            @visible-change="showContextMenu(index, $event)"
          >
            <span class="inline-flex items-center">
              {{ $t(camelCase(item.meta?.title)) }}
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click="refresh(index)">
                  <el-icon><i class="i-ep-refresh" /></el-icon
                  ><span>{{ $t('刷新') }}</span>
                </el-dropdown-item>
                <el-dropdown-item
                  :disabled="index === 0"
                  @click="removeLeft(index)"
                >
                  <el-icon><i class="i-ep-back" /></el-icon
                  ><span>{{ $t('关闭左侧') }}</span>
                </el-dropdown-item>
                <el-dropdown-item
                  :disabled="index === tabsStore.routes.length - 1"
                  @click="removeRight(index)"
                >
                  <el-icon><i class="i-ep-right" /></el-icon
                  ><span>{{ $t('关闭右侧') }}</span>
                </el-dropdown-item>
                <el-dropdown-item
                  :disabled="
                    index === 0 && index === tabsStore.routes.length - 1
                  "
                  @click="removeOthers(index)"
                >
                  <el-icon><i class="i-ep-switch" /></el-icon>
                  <span>{{ $t('关闭其他') }}</span>
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </template>
      </el-tab-pane>
    </template>
  </el-tabs>
</template>

<script setup>
import { watchEffect, nextTick, ref } from 'vue'
import { onBeforeRouteUpdate, useRoute, useRouter } from 'vue-router'
import { camelCase } from '@/utils/index.js'
import { useTabsStore } from '../store/index.js'

const tabsStore = useTabsStore()
const itemRefs = ref([])
const currentRoute = useRoute()
const router = useRouter()
const model = ref('')

// onBeforeRouteUpdate((to) => {
//   model.value = to.fullPath;
// });
watchEffect(() => {
  model.value = currentRoute.fullPath
})

const setRef = (index, el) => {
  if (el) {
    itemRefs.value[index] = el
  } else {
    itemRefs.value.splice(index, 1)
  }
}
const showContextMenu = (index, show) => {
  if (show) {
    itemRefs.value.forEach((item, i) => {
      if (i !== index) {
        item?.handleClose()
      }
    })
  }
}

const refresh = index => {
  const currentIndex = tabsStore.routes.findIndex(
    o => o.fullPath === currentRoute.fullPath,
  )
  const route = tabsStore.routes[index]
  if (index !== currentIndex) {
    router.push({ path: route.fullPath })
  }
  tabsStore.isRefreshing = true
  nextTick(() => {
    tabsStore.isRefreshing = false
  })
}

const deleteItem = (start, end) => {
  tabsStore.routes.splice(start, end)
}

const remove = name => {
  if (tabsStore.routes.length > 1) {
    const index = tabsStore.routes.findIndex(o => o.fullPath === name)
    const currentIndex = tabsStore.routes.findIndex(
      o => o.fullPath === currentRoute.fullPath,
    )
    deleteItem(index, 1)
    if (index === currentIndex) {
      if (tabsStore.routes[index]) {
        router.push(tabsStore.routes[index])
      } else {
        router.push(tabsStore.routes[index - 1])
      }
    }
  }
}

const removeLeft = index => {
  const currentIndex = tabsStore.routes.findIndex(
    o => o.fullPath === currentRoute.fullPath,
  )
  const route = tabsStore.routes[index]
  deleteItem(0, index)
  if (currentIndex < index) {
    router.push(route)
  }
}

const removeRight = index => {
  const currentIndex = tabsStore.routes.findIndex(
    o => o.fullPath === currentRoute.fullPath,
  )
  deleteItem(index + 1, tabsStore.routes.length - index)
  if (currentIndex > index) {
    router.push(tabsStore.routes[index])
  }
}

const removeOthers = index => {
  removeRight(index)
  removeLeft(index)
  if (tabsStore.routes[0].fullPath !== currentRoute.fullPath) {
    router.push(tabsStore.routes[0])
  }
}

const onClick = context => {
  if (!context.active) {
    router.push(context.props.name)
  }
}
</script>
