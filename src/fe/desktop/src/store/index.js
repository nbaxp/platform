import { createPinia } from 'pinia'

import useAppStore from './app.js'
import useTabsStore from './tabs.js'
import useTokenStore from './token.js'
import useUserStore from './user.js'

const store = createPinia()

export { useAppStore, useTabsStore, useTokenStore, useUserStore }
export default store
