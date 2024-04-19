<template>
  <div ref="tplRef">
    <div class="source" style="display: none">
      <slot />
    </div>
    <div class="markdown-body" />
  </div>
</template>

<script setup>
import 'cherry-markdown/dist/cherry-markdown.css'

import { useMediaQuery } from '@vueuse/core'
import Cherry from 'cherry-markdown'
import { nextTick, onMounted, ref, watchEffect } from 'vue'

import { useAppStore } from '@/store/index.js'

const props = defineProps({
  name: {
    type: String,
    default: null,
  },
})

const tplRef = ref(null)
const appStore = useAppStore()

const changeTheme = () => {
  const targetSelector = '.cherry-previewer'
  const lightClass = 'theme__default'
  const darkClass = 'theme__dark'
  const target = tplRef.value?.querySelector(targetSelector)
  const isDarkNow = useMediaQuery('(prefers-color-scheme: dark)')
  if (
    (appStore.settings.mode === 'auto' && isDarkNow.value) ||
    appStore.settings.mode === 'dark'
  ) {
    target?.classList.remove(lightClass)
    target?.classList.add(darkClass)
  } else {
    target?.classList.remove(darkClass)
    target?.classList.add(lightClass)
  }
}

const components = import.meta.glob('../../assets/docs/**/*.md', {
  query: '?raw',
})

onMounted(async () => {
  let mdText = tplRef.value.querySelector('.source pre')?.innerText
  if (props.name) {
    console.log(components)
    mdText = (await (await components[`../../assets/docs/${props.name}.md`])())
      .default
  }
  const _ = new Cherry({
    el: tplRef.value.querySelector('.markdown-body'),
    value: mdText,
    toolbars: {
      toolbar: false,
    },
    editor: {
      defaultModel: 'previewOnly',
    },
    previewer: {
      className: 'cherry-markdown head-num',
    },
  })
  tplRef.value.querySelector('.source').remove()
  nextTick(changeTheme)
})

watchEffect(changeTheme)
</script>
