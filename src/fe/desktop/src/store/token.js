import { jwtDecode } from 'jwt-decode'
import { defineStore } from 'pinia'

import router from '@/router/index.js'
import { log } from '@/utils/index.js'
import { getUrl } from '@/utils/request.js'

import useUserStore from './user.js'

const REFRESH_TOKEN_KEY = 'refresh_token'

export default defineStore('token', {
  state: () => ({
    accessToken: null,
    refreshToken: localStorage.getItem(REFRESH_TOKEN_KEY),
  }),
  actions: {
    async refresh() {
      log('refresh token')
      if (this.refreshToken) {
        let valid = false
        const exp = new Date(jwtDecode(this.refreshToken).exp * 1000)
        if (exp > new Date()) {
          const response = await fetch(getUrl('token/refresh'), {
            method: 'POST',
            body: JSON.stringify(this.refreshToken),
            headers: {
              'Content-Type': 'application/json',
            },
          })
          if (response.status === 200) {
            const result = await response.json()
            this.setToken(result.data.access_token, result.data.refresh_token)
            valid = true
          }
        }
        if (!valid) {
          this.removeToken()
        }
      }
    },
    async isLogin() {
      if (this.accessToken) {
        const exp = new Date(jwtDecode(this.accessToken).exp * 1000)
        if (exp > new Date()) {
          return true
        }
      }
      await this.refresh()
      return !!this.accessToken
    },
    setToken(accessToken, refreshToken) {
      if (accessToken && refreshToken) {
        this.accessToken = accessToken
        this.refreshToken = refreshToken
        localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
      }
    },
    removeToken() {
      this.accessToken = null
      this.refreshToken = null
      localStorage.removeItem(REFRESH_TOKEN_KEY)
    },
    async logout() {
      this.removeToken()
      const userStore = useUserStore()
      userStore.$reset()
      router.push({
        path: '/redirect',
        query: { redirect: router.currentRoute.value.fullPath },
      })
    },
  },
})
