<template>
  <i class="i-ep-setting cursor-pointer" @click="show = !show" />
  <el-drawer
    v-model="show"
    :title="$t('设置')"
    append-to-body
    destroy-on-close
    size="auto"
  >
    <app-form
      ref="formRef"
      v-model="appStore.settings"
      :schema="schema"
      :hide-button="true"
    />
    <template #footer>
      <el-button type="primary" @click="copySettings">
        {{ $t('复制') }}
      </el-button>
      <el-button type="primary" @click="reset">
        {{ $t('重置') }}
      </el-button>
    </template>
  </el-drawer>
</template>

<script setup>
import { useClipboard, useMediaQuery } from '@vueuse/core'
import { ElMessage } from 'element-plus'
import { ref, watchEffect } from 'vue'

import AppForm from '@/components/form/index.vue'
import useModel from '@/models/settings.js'
import { useAppStore } from '@/store/index.js'

const show = ref(false)
const formRef = ref(null)
const schema = useModel()
const appStore = useAppStore()
const { copy } = useClipboard()
const copySettings = async () => {
  try {
    await formRef.value.validate()
    const text = JSON.stringify(appStore.settings, null, 2)
    await copy(text)
    ElMessage({
      message: 'config/settings.json',
      type: 'success',
    })
  } catch (error) {
    console.log(error)
  }
}
const reset = async () => {
  await formRef.value.reset()
}
watchEffect(() => {
  document.documentElement.classList[
    appStore.settings.useDarkNav ? 'add' : 'remove'
  ]('dark-nav')
})
watchEffect(() => {
  document.documentElement.style.setProperty(
    '--el-color-primary',
    appStore.settings.color,
  )
})
watchEffect(() => {
  const darkClass = 'dark'
  const toDark = () => document.documentElement.classList.add(darkClass)
  const toLight = () => document.documentElement.classList.remove(darkClass)
  const isDarkNow = useMediaQuery('(prefers-color-scheme: dark)')
  if (
    (appStore.settings.mode === 'auto' && isDarkNow.value) ||
    appStore.settings.mode === 'dark'
  ) {
    toDark()
  } else {
    toLight()
  }
})
</script>
