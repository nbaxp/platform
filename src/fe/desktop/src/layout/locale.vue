<template>
  <el-dropdown class="cursor-pointer">
    <span class="el-dropdown-link flex">
      <el-icon :size="18">
        <i class="i-ion-language" />
      </el-icon>
    </span>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item
          v-for="locale in $i18n.availableLocales"
          :key="locale"
          @click="change(locale)"
        >
          {{ options.find(o => o.key === locale).value }}
          <el-icon v-if="locale === $i18n.locale" class="el-icon--right">
            <i class="i-ep-select" />
          </el-icon>
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup>
import { useTitle } from '@vueuse/core'
import { watchEffect } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import { camelCase } from '@/utils/index.js'

import { useAppStore } from '@/store/index.js'

const appStore = useAppStore()

const { options } = appStore.locale

const i18n = useI18n()
const change = locale => {
  i18n.locale.value = locale
}
const route = useRoute()
watchEffect(() => {
  if (route.meta.title) {
    useTitle().value = i18n.t(camelCase(route.meta?.title))
  }
})
</script>
