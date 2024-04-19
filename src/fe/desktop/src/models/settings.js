const rules = [
  {
    required: true,
  },
]

export default function () {
  return {
    title: 'settings',
    properties: {
      isDebug: {
        title: '调试模式',
        type: 'boolean',
      },
      useMock: {
        title: '模拟请求',
        type: 'boolean',
      },
      baseURL: {
        rules,
      },
      serverLocale: {
        title: '服务端区域设置',
        type: 'boolean',
      },
      serverRoute: {
        title: '服务端路由模式',
        type: 'boolean',
      },
      maxTabs: {
        title: '标签页数量',
        type: 'integer',
        rules,
      },
      isMenuCollapse: {
        title: '菜单栏折叠',
        rules,
        type: 'boolean',
      },
      size: {
        title: '控件大小',
        input: 'select',
        options: [
          { value: 'large', label: 'large' },
          { value: 'default', label: 'default' },
          { value: 'small', label: 'small' },
        ],
      },
      color: {
        title: '主题色',
        input: 'color',
      },
      mode: {
        title: '主题模式',
        input: 'select',
        options: [
          { value: 'auto', label: 'system', icon: 'platform' },
          { value: 'light', label: 'light', icon: 'sunny' },
          { value: 'dark', label: 'dark', icon: 'moon' },
        ],
      },
      useDarkNav: {
        title: '暗色导航',
        type: 'boolean',
      },
      useTabs: {
        title: '启用 Tab 页签',
        type: 'boolean',
      },
      showBreadcrumb: {
        title: '显示站内导航',
        type: 'boolean',
      },
      showCopyright: {
        title: '显示页脚',
        type: 'boolean',
      },
    },
  }
}
