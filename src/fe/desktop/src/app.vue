<template>
  <el-config-provider
    :locale="options[$i18n.locale]"
    :size="appStore.settings.size"
  >
    <router-view />
  </el-config-provider>
</template>

<script setup>
import en from 'element-plus/dist/locale/en.mjs'
import zh from 'element-plus/dist/locale/zh-cn.mjs'
import { ref, provide, onMounted } from 'vue'
import { HubConnectionBuilder } from '@microsoft/signalr'
import PubSub from 'PubSub'
import { useAppStore, useTokenStore } from '@/store/index.js'

provide('routeData', new Map())
const options = ref({
  'zh-CN': zh,
  'en-US': en,
})
const appStore = useAppStore()
const tokenStore = useTokenStore()
onMounted(() => {
  let connection = new HubConnectionBuilder()
    .withUrl('./api/hub', {
      accessTokenFactory: async () => {
        const isLogin = await tokenStore.isLogin()
        if (isLogin) {
          return tokenStore.accessToken
        }
      },
    })
    .build()

  connection.on('ServerToClient', (method, data) => {
    PubSub.publish(method, data)
  })

  const start = async () => {
    try {
      await connection.start()
      console.log('SignalR Connected.')
      appStore.connection = connection
    } catch (err) {
      console.log(err)
      setTimeout(start, 5000)
    }
  }

  connection.onclose(async () => {
    console.log('SignalR Disconnected.')
    appStore.connection = null
    await start()
  })

  // start();
})
</script>
