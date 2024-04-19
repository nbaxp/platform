import Md from '@/components/markdown/index.vue'
import Layout from '@/layout/index.vue'

const md = name => {
  return {
    components: { Md },
    template: `<md name='${name}' />`,
  }
}

export default {
  path: '/docs',
  redirect: '/docs/folder',
  component: Layout,
  meta: {
    title: '文档',
    icon: 'help',
    order: 1000,
  },
  children: [
    {
      path: 'folder',
      component: md('folder'),
      meta: {
        title: '目录结构',
        icon: 'files',
      },
    },
    {
      path: 'i18n',
      component: md('i18n'),
      meta: {
        title: '国际化',
      },
    },
    {
      path: 'router',
      component: md('router'),
      meta: {
        title: '路由',
      },
    },
    {
      path: 'pinia',
      component: md('pinia'),
      meta: {
        title: '状态管理',
      },
    },
    {
      path: 'mock',
      component: md('mock'),
      meta: {
        title: '模拟数据',
      },
    },
    {
      path: 'basic',
      meta: {
        title: '开发基础',
      },
      children: [
        {
          path: 'git',
          component: md('git'),
          meta: {
            title: 'Git',
          },
        },
        {
          path: 'markdown',
          component: md('markdown'),
          meta: {
            title: 'Markdown',
          },
        },
      ],
    },
    {
      path: 'web',
      meta: {
        title: 'Web 基础',
      },
      children: [
        {
          path: 'html',
          component: md('web/html'),
          meta: {
            title: 'HTML',
          },
        },
        {
          path: 'css',
          meta: {
            title: 'CSS',
          },
          children: [
            {
              path: 'flex',
              component: md('web/css/flex'),
              meta: {
                title: 'Flex 布局',
              },
            },
          ],
        },
        {
          path: 'js',
          meta: {
            title: 'JavaScript',
          },
          children: [
            {
              path: 'esm',
              component: md('web/javascript/esm'),
              meta: {
                title: 'ESM 模块化',
              },
            },
            {
              path: 'promise',
              component: md('web/javascript/promise'),
              meta: {
                title: 'Async 异步',
              },
            },
            {
              path: 'fetch',
              component: md('web/javascript/fetch'),
              meta: {
                title: 'Fetch 网络请求',
              },
            },
          ],
        },
      ],
    },
    {
      path: 'vue',
      meta: {
        title: 'Vue 技术栈',
      },
      children: [
        {
          path: 'basic',
          component: md('vue/basic'),
          meta: {
            title: 'Vue 基础',
          },
        },
        {
          path: 'echarts',
          component: md('vue/echarts'),
          meta: {
            title: 'Vue ECharts',
          },
        },
        {
          path: 'ep',
          meta: {
            title: 'Element Plus',
          },
          children: [
            {
              path: 'menu',
              component: md('ep/menu'),
              meta: {
                title: '菜单 Menu',
              },
            },
          ],
        },
      ],
    },
  ],
}
