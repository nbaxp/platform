export default [
  {
    name: 'default',
    path: '/',
    redirect: '/home',
    meta: { title: 'Home Page', icon: 'home' },
    children: [
      {
        path: 'home',
        component: 'home',
        meta: {
          title: 'Home Page',
          icon: 'home',
        },
      },
      {
        path: 'user',
        meta: {
          title: 'User Center',
        },
        children: [
          {
            path: 'info',
            component: 'list',
            meta: {
              title: 'User Info',
            },
          },
          {
            path: 'settings',
            component: 'list',
            meta: {
              title: 'User Settings',
            },
          },
        ],
      },
      {
        path: 'system',
        meta: {
          title: 'System Management',
        },
        children: [
          {
            path: 'menu',
            component: 'list',
            meta: {
              title: 'Menu Management',
            },
          },
          {
            path: 'user',
            component: 'list',
            meta: {
              title: 'User Management',
            },
          },
          {
            type: 'page',
            path: 'role',
            component: 'list',
            meta: {
              title: 'Role Management',
              model: 'system/role',
            },
            children: [
              {
                type: 'button',
                meta: {
                  title: 'search',
                  action: 'search',
                  method: 'POST',
                  url: 'role/index',
                },
              },
              {
                type: 'button',
                meta: {
                  title: 'create',
                  action: 'create',
                  method: 'POST',
                  url: 'role',
                },
              },
              {
                type: 'button',
                meta: {
                  position: 'row',
                  title: 'update',
                  action: 'update',
                  method: 'POST',
                  url: 'role',
                },
              },
              {
                type: 'button',
                meta: {
                  title: 'delete',
                  action: 'delete',
                  method: 'POST',
                  url: 'role',
                },
              },
              {
                type: 'button',
                meta: {
                  title: '导入',
                  action: 'import',
                  method: 'POST',
                  url: 'role',
                },
              },
              {
                type: 'button',
                meta: {
                  title: '导出',
                  action: 'export',
                  method: 'POST',
                  url: 'role',
                },
              },
            ],
          },
          {
            path: 'department',
            component: 'list',
            meta: {
              title: 'Department Management',
            },
          },
          {
            path: 'dictionary',
            component: 'list',
            meta: {
              title: 'Dictionary Management',
            },
          },
          {
            path: 'monitor',
            component: 'list',
            meta: {
              title: 'System Monitor',
            },
          },
        ],
      },
    ],
  },
]
