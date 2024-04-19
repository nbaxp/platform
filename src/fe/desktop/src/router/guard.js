import { useTitle } from '@vueuse/core'
import NProgress from 'nprogress'

import i18n from '@/locale/index.js'
import { refreshRouter } from '@/router/index.js'
import {
  useAppStore,
  useTabsStore,
  useTokenStore,
  useUserStore,
} from '@/store/index.js'
import { log, camelCase } from '@/utils/index.js'

NProgress.configure({ showSpinner: false })

const beforeEach = async (to, from, next) => {
  log(`before route: ${from.fullPath}-->${to.fullPath}`)
  NProgress.start()
  const appStore = useAppStore()
  const tokenStore = useTokenStore()
  const userStore = useUserStore()
  const isLogin = await tokenStore.isLogin()
  let refresh = false
  if (appStore.settings.serverLocale && !appStore.locale) {
    // 加载资源文件
    await appStore.getLocale()
  }
  if (appStore.settings.serverRoute && !appStore.menus) {
    // 加载服务端菜单
    await refreshRouter()
    refresh = true
  }
  if (to.path !== '/register' && to.path !== '/login' && to.path !== '/403') {
    if (isLogin && !userStore.userName) {
      // 加载用户信息
      await userStore.getUserInfo()
    }
  }
  // 认证和授权
  if (refresh) {
    next({ path: to.fullPath })
  } else if (
    to.path !== '/register' &&
    to.path !== '/forgot-password' &&
    to.path !== '/login' &&
    to.path !== '/403'
  ) {
    if (!isLogin) {
      next({ path: '/login', query: { redirect: to.fullPath } })
    } else if (!userStore.hasPermission(to.meta.authorize)) {
      next({ path: '/403', query: { redirect: to.fullPath } })
    } else {
      next()
    }
  } else {
    next()
  }
}

const afterEach = (to, from) => {
  log(`after route: ${from.fullPath}-->${to.fullPath}`)
  try {
    const appStore = useAppStore()
    if (appStore.settings.useTabs && !to.meta?.hideInMenu) {
      const tabsStore = useTabsStore()
      tabsStore.addRoute(to)
    }
    if (to.meta?.title) {
      useTitle().value = i18n.global.t(camelCase(to.meta.title))
    }
  } finally {
    NProgress.done()
  }
}

export { afterEach, beforeEach }
