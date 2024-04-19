import { defineStore } from 'pinia'

import settings from '@/config/settings.js'
import i18n from '@/locale/index.js'
import { log } from '@/utils/index.js'
import { getUrl } from '@/utils/request.js'

export default defineStore('app', {
  state: () => ({
    settings: { ...settings },
    connection: null,
  }),
  actions: {
    async getLocale() {
      log('fetch locale')
      try {
        const response = await fetch(getUrl('locale'), { method: 'POST' })
        if (response.ok) {
          const result = await response.json()
          this.locale = result.data
          i18n.global.locale.value = this.locale.locale
          Object.keys(this.locale.messages).forEach(key => {
            i18n.global.mergeLocaleMessage(key, this.locale.messages[key])
          })
        } else {
          throw new Error(response.statusText)
        }
      } catch (e) {
        log(e)
      }
    },
    async getMenus() {
      log('fetch menus')
      try {
        const response = await fetch(getUrl('menu'), { method: 'POST' })
        if (response.ok) {
          const result = await response.json()
          this.menus = result.data
        } else {
          throw new Error(response.statusText)
        }
      } catch (e) {
        log(e)
      }
    },
  },
})
