import { createRouter, createWebHashHistory } from 'vue-router'
import { useAppStore } from '@/store/index.js'
import { listToTree } from '@/utils/index.js'
import { defineAsyncComponent, markRaw } from 'vue'

import docs from './docs.js'
import { afterEach, beforeEach } from './guard.js'

const layouts = import.meta.glob('../layout/**/*.vue')
const views = import.meta.glob('../views/**/*.vue')

const layout = name => {
  return layouts[`../layout/${name}.vue`]
}

const view = (file, name = null) => {
  return markRaw({
    name: name ?? `/${file}`,
    components: {
      AppPage: defineAsyncComponent(views[`../views/${file}.vue`]),
    },
    template: `<app-page />`,
  })
}

const routes = [
  {
    path: '/register',
    component: view('register'),
    meta: {
      title: '注册',
      hideInMenu: true,
    },
  },
  {
    path: '/forgot-password',
    component: view('forgot-password'),
    meta: {
      title: '忘记密码',
      hideInMenu: true,
    },
  },
  {
    path: '/login',
    component: view('login'),
    meta: {
      title: '登录',
      hideInMenu: true,
    },
  },
  {
    path: '/403',
    component: view('403'),
    meta: {
      title: '权限不足',
      hideInMenu: true,
    },
  },
  {
    path: '/redirect',
    component: view('redirect'),
    meta: {
      title: '跳转',
      hideInMenu: true,
    },
  },
  {
    path: '/:pathMatch(.*)*',
    component: view('404'),
    meta: {
      title: '未找到',
      hideInMenu: true,
    },
  },
  docs,
]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
})

const convert = (list, parent = null) => {
  list.forEach(o => {
    if (o.meta.type === 'menu') {
      o.meta.buttons = o.children
      delete o.children
    }
    o.name = (parent ? parent.name + '/' : '/') + o.path
    const file = o.component
    if (o.redirect) {
      o.component = layout(file ?? 'index')
    } else if (file) {
      o.component = view(file, o.name)
    }
    if (o.children?.length) {
      convert(o.children, o)
    }
  })
  return list
}

async function refreshRouter() {
  const appStore = useAppStore()
  await appStore.getMenus()
  const tree = convert(
    listToTree(
      appStore.menus.map(o => {
        return {
          id: o.id,
          parentId: o.parentId,
          path: o.routerPath,
          redirect: o.redirect,
          component: o.component,
          meta: {
            type: o.type,
            buttonType: o.buttonType,
            title: o.name,
            icon: o.icon,
            classList: o.classList,
            order: o.order,
            noCache: o.noCache,
            permission: o.number,
            authorize: o.authorize,
            method: o.method,
            url: o.url,
            command: o.command,
            hidden: o.hidden,
            schema: o.schema,
          },
        }
      }),
    ),
  )
  const route = router.getRoutes().find(o => o.name === 'layout')
  if (route) {
    router.removeRoute(route.name)
  }
  router.addRoute('/', {
    name: 'layout',
    path: '/',
    redirect: '/home',
    component: layout('index'),
    meta: {
      title: '首页',
      icon: 'home',
      order: 1,
    },
    children: tree,
  })
}

router.beforeEach(beforeEach)
router.afterEach(afterEach)

export default router

export { refreshRouter }
