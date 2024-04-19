import 'nprogress/nprogress.css'
import 'element-plus/theme-chalk/dark/css-vars.css'
import 'element-plus/theme-chalk/el-message-box.css'
import 'virtual:uno.css'
import '@/styles/site.css'

import { createApp } from 'vue'

import App from '@/app.vue'
import i18n from '@/locale/index.js'
import useMock from '@/mock/index.js'
import router from '@/router/index.js'
import store from '@/store/index.js'
import { delay } from '@/utils/index.js'

//await delay(3 * 1000);
//useMock();
const app = createApp(App)
app.use(store)
app.use(i18n)
app.use(router)
app.mount('#app')
