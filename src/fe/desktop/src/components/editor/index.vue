<template>
  <wang-editor v-if="show" v-model="model" :mode="mode" />
</template>

<script setup>
import { nextTick, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

import WangEditor from './wangEditor.vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  mode: {
    type: String,
    default: 'default',
  },
})

const emit = defineEmits(['update:modelValue'])

const model = ref(props.modelValue)
watch(model, value => emit('update:modelValue', value))

const show = ref(true)
const i18n = useI18n()

watch(i18n.locale, () => {
  show.value = false
  nextTick(() => {
    show.value = true
  })
})
</script>
