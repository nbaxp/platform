<template>
  <a href="javascript:;" class="logo" @click="reload">
    <div class="flex items-center">
      <img :src="logoUrl" />
      <h1 v-if="!appStore.settings.isMenuCollapse">{{ $t('应用程序') }}</h1>
    </div>
  </a>
</template>
<script setup>
import { reload } from '@/utils/index.js'

import { useAppStore } from '../store/index.js'
import logoUrl from '../assets/logo.svg?url'

const appStore = useAppStore()
</script>
