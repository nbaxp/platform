<template>
  <div class="flex items-center justify-between">
    <div class="flex items-center justify-center">
      <layout-logo />
      <el-icon style="margin: 0 20px" @click="toggleMenuCollapse">
        <i v-if="appStore.settings.isMenuCollapse" class="i-uiw-menu-unfold" />
        <i v-else class="i-uiw-menu-fold" />
      </el-icon>
      <el-menu
        mode="horizontal"
        :default-active="$route.matched[0].path"
        :ellipsis="false"
      >
        <template v-for="route in routes">
          <el-menu-item
            v-if="!route.meta?.hideInMenu"
            :key="route.path"
            :index="route.path"
            @click="click(route, $event)"
          >
            <template #title>
              <svg-icon v-if="route.meta?.icon" :name="route.meta.icon" />
              <span>{{ $t(route.meta?.title ?? route.path) }}</span>
            </template>
          </el-menu-item>
        </template>
      </el-menu>
    </div>
    <div class="flex">
      <el-space size="large">
        <el-select
          v-show="showSearch"
          ref="searchRef"
          v-model="searchModel"
          placeholder="search"
          filterable
          remote
          :remote-method="searchMenu"
          :loading="searchLoading"
          style="width: 200px"
          @blur="hideSearch"
        >
          <el-option
            v-for="item in searchOptions"
            :key="item.path"
            :value="item.path"
            :label="$t(camelCase(item.meta.title))"
            @click="searchChange(item)"
          />
        </el-select>
        <el-icon @click="clickSearch">
          <i class="i-ep-search" />
        </el-icon>
        <el-icon @click="toggleFullscreen">
          <i v-if="isFullscreen" class="i-dashicons-fullscreen-exit-alt" />
          <i v-else class="i-dashicons-fullscreen-alt" />
        </el-icon>
        <el-dropdown v-if="tokenStore.accessToken">
          <span class="el-dropdown-link flex">
            <el-avatar
              v-if="userStore.avatar"
              :size="18"
              class="el-icon--left"
              :src="userStore.avatar"
            />
            <el-icon v-else class="el-icon--left">
              <i class="i-ep-user" />
            </el-icon>
            <span>{{ userStore.name ?? userStore.userName }}</span>
            <el-icon class="el-icon--right">
              <b class="i-ep-arrow-down" />
            </el-icon>
          </span>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item>
                <router-link to="/account">
                  <el-icon> <b class="i-ep-user" /> </el-icon>
                  <span>{{ $t('用户中心') }}</span>
                </router-link>
              </el-dropdown-item>
              <el-dropdown-item divided @click="confirmLogout">
                <el-icon> <b class="i-ep-switch-button" /> </el-icon>
                <span>{{ $t('注销') }}</span>
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
        <layout-locale />
        <layout-settings />
      </el-space>
    </div>
  </div>
</template>

<script setup>
import { useFullscreen } from '@vueuse/core'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { camelCase } from '@/utils/index.js'
import { orderBy } from 'lodash-es'

import SvgIcon from '@/components/icon/index.vue'
import {
  useAppStore,
  useTokenStore,
  useUserStore,
  useTabsStore,
} from '@/store/index.js'

import LayoutLocale from './locale.vue'
import LayoutLogo from './logo.vue'
import LayoutSettings from './settings.vue'

const router = useRouter()
const { t } = useI18n()
const appStore = useAppStore()
const tabsStore = useTabsStore()
const tokenStore = useTokenStore()
const userStore = useUserStore()
//
const routes = computed(() =>
  orderBy(
    router
      .getRoutes()
      .filter(o => o.path.startsWith('/') && o.redirect)
      .map(o => ({ ...o, order: o.meta.order ?? 0 })),
    ['order'],
  ),
)
const searchRef = ref(null)
const searchLoading = ref(false)
const searchModel = ref('')
const searchOptions = ref([])
const showSearch = ref(false)
const hideSearch = () => {
  showSearch.value = false
}
const clickSearch = () => {
  showSearch.value = !showSearch.value
  if (showSearch.value) {
    searchRef.value.focus()
  }
}
const searchMenu = query => {
  if (query) {
    try {
      searchLoading.value = true
      searchOptions.value = router
        .getRoutes()
        .filter(
          o =>
            !o.meta?.hideInMenu &&
            !o.children?.length &&
            t(camelCase(o.meta?.title)).indexOf(query) >= 0,
        )
    } finally {
      searchLoading.value = false
    }
  }
}
const searchChange = route => {
  if (!route.meta?.isExternal) {
    router.push(route.path)
    searchModel.value = ''
    searchOptions.value = []
    showSearch.value = false
  } else {
    window.open(route.path)
  }
}
//
const toggleMenuCollapse = () => {
  appStore.settings.isMenuCollapse = !appStore.settings.isMenuCollapse
}
//
const { isFullscreen, toggle: toggleFullscreen } = useFullscreen(
  document.documentElement,
)
const confirmLogout = async () => {
  try {
    await ElMessageBox.confirm(t('确认退出'), t('警告'), { type: 'warning' })
    await tokenStore.logout()
  } catch (error) {
    if (error === 'cancel') {
      ElMessage({
        type: 'info',
        message: t('cancel'),
      })
    }
  }
}
//

const click = (route, event) => {
  if (route.path.startsWith('http')) {
    window.open(props.node.path)
  } else {
    const path =
      tabsStore.routes.findLast(o => o.matched[0].path === route.path)?.path ??
      route.path
    router.push(path)
  }
}
</script>
