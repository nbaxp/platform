<template>
  <el-form
    ref="formRef"
    v-loading="loading"
    :model="model"
    label-width="auto"
    :inline="inline"
    @keyup.enter="submit"
  >
    <el-form-item v-if="errorMessage" style="margin-bottom: 0">
      <el-text type="danger">
        {{ errorMessage }}
      </el-text>
    </el-form-item>
    <template v-if="schema && schema.properties">
      <template
        v-for="(value, prop) in getProperties(schema.properties)"
        :key="prop"
      >
        <app-form-item
          v-model="model"
          :parent-schema="schema"
          :schema="value"
          :prop="prop"
          :mode="mode"
          :errors="errors"
        />
      </template>
    </template>
    <slot />
    <el-form-item v-if="!hideButton" style="margin-bottom: 0">
      <slot name="submit">
        <el-button
          type="primary"
          :disabled="loading"
          :style="schema.submitStyle"
          @click="submit"
        >
          {{ $t(schema.title ?? '确定') }}
        </el-button>
        <el-button v-if="showReset" :disabled="loading" @click="reset">
          {{ $t('重置') }}
        </el-button>
      </slot>
    </el-form-item>
  </el-form>
</template>

<script setup>
import { reactive, ref, watch } from 'vue'

import request from '@/utils/request.js'

import AppFormItem from './form-item.vue'

const props = defineProps([
  'modelValue',
  'inline',
  'schema',
  'hideButton',
  'showReset',
  'isQueryForm',
  'beforeSubmit',
  'mode',
])
const emit = defineEmits(['update:modelValue', 'success', 'error'])
// init
const model = reactive(props.modelValue)
watch(model, value => {
  emit('update:modelValue', value)
})
// ref
const formRef = ref(null)
const loading = ref(false)
//
const errors = ref({})
const errorMessage = ref(null)
// reset
const reset = () => {
  formRef.value.resetFields()
}
// validate
const validate = async () => {
  return formRef.value.validate()
}
// submit
const submit = async () => {
  try {
    const valid = await validate()
    if (valid) {
      loading.value = true
      const { url } = props.schema
      const method = props.schema.method ?? 'POST'
      errorMessage.value = null
      errors.value = {} // 必须先清空
      const data = props.beforeSubmit
        ? props.beforeSubmit(props.mode, model) ?? model
        : model
      const result = await request(method, url, data)
      if (result.ok) {
        emit('success', { action: props.mode, data: result.data })
      } else {
        if (result.code == 400) {
          errors.value = result.data
        } else if (result.code == 500) {
          errorMessage.value = result.message
        }
        emit('error', { action: props.mode, data: result.data })
      }
    }
  } catch (error) {
    console.log(error)
  } finally {
    loading.value = false
  }
}
const getProperties = properties => {
  return Object.fromEntries(
    Object.entries(properties).sort(([, a], [, b]) => a.order - b.order),
  )
}
defineExpose({ validate, reset, submit })
</script>
