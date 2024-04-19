import { defineStore } from 'pinia'

import { log } from '@/utils/index.js'
import request from '@/utils/request.js'

import { useTokenStore, useUserStore } from '@/store/index.js'

export default defineStore('user', {
  state: () => ({
    userName: null,
    name: null,
    avatar: null,
    roles: [],
    permissions: [],
  }),
  actions: {
    async getUserInfo() {
      log('fetch user info')
      const tokenStore = useTokenStore()
      if (tokenStore.accessToken) {
        const url = 'user/info'
        const result = await request('POST', url)
        if (result.ok) {
          this.$patch(result.data)
        }
      }
    },
    hasPermission(permission) {
      const userStore = useUserStore()
      const tokenStore = useTokenStore()
      if (/\[\]/.test(permission)) {
        const roles = JSON.parse(permission)
        return userStore.roles.some(o => roles.includes(o))
      }
      if (permission === 'Anonymous') {
        return true
      } else if (permission === 'Authenticated') {
        return tokenStore.isLogin()
      } else {
        return !permission || this.permissions?.some(o => o === permission)
      }
    },
  },
})
