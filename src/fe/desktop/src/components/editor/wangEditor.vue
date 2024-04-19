<template>
  <div style="border: 1px solid #ccc; z-index: 1000">
    <Toolbar
      style="border-bottom: 1px solid #ccc"
      :editor="editorRef"
      :default-config="toolbarConfig"
      :mode="mode"
    />
    <Editor
      v-model="model"
      style="height: 500px; overflow-y: hidden"
      :default-config="editorConfig"
      :mode="mode"
      @on-created="handleCreated"
    />
  </div>
</template>

<script setup>
import '@wangeditor/editor/dist/css/style.css'

import { i18nChangeLanguage } from '@wangeditor/editor'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import { onBeforeUnmount, ref, shallowRef, watch } from 'vue'
import { useI18n } from 'vue-i18n'

import request from '@/utils/request.js'

const props = defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  mode: {
    type: String,
    default: 'default', // simple
  },
  uploadUrl: {
    type: String,
    default: null,
  },
})

const emit = defineEmits(['update:modelValue'])

const model = ref(props.modelValue)
watch(model, value => emit('update:modelValue', value))

const customUpload = async (file, insertFn) => {
  const method = 'POST'
  const url = 'file/upload'
  const data = new FormData()
  data.append('file', file)
  const result = await request(method, url, data)
  insertFn(result.data)
}

const editorRef = shallowRef()
const toolbarConfig = {}
const editorConfig = {
  placeholder: '...',
  MENU_CONF: {
    uploadImage: { customUpload },
    uploadVideo: { customUpload },
  },
}

onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor == null) return
  editor.destroy()
})

const handleCreated = editor => {
  editorRef.value = editor
}

const i18n = useI18n()
i18nChangeLanguage(i18n.locale.value)
</script>

<style>
html.dark {
  --w-e-textarea-bg-color: #333;
  --w-e-textarea-color: #fff;
}
</style>
