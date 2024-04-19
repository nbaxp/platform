<template>
  <template v-if="!schema.hidden && showItem()">
    <template v-if="schema.type === 'object'" />
    <template
      v-else-if="schema.type !== 'array' || schema.items?.type !== 'array'"
    >
      <el-form-item
        :label="parentSchema.labelWidth === 0 ? '' : $t(schema.title ?? prop)"
        :prop="getProp(prop)"
        :rules="getDisabled() ? [] : getRules(parentSchema, prop, model)"
        :error="error"
      >
        <app-form-input
          v-model="model"
          :schema="schema"
          :prop="prop"
          :mode="mode"
        />
      </el-form-item>
    </template>
  </template>
</template>

<script setup>
import { computed, reactive, watch } from 'vue'

import AppFormInput from '@/components/form/form-input.vue'
import { getRules } from '@/utils/validation.js'

const props = defineProps([
  'modelValue',
  'mode',
  'parentSchema',
  'schema',
  'prop',
  'errors',
])
const emit = defineEmits(['update:modelValue'])

const model = reactive(props.modelValue)
watch(model, value => {
  emit('update:modelValue', value)
})
/* start */
const showItem = () => {
  if (props.schema.hidden) {
    return false
  }
  if (
    props.mode === 'query' &&
    (props.schema.hideInQuery || props.schema.input === 'upload')
  ) {
    return false
  }
  return true
}
const getDisabled = () => {
  if (props.mode === 'details' || props.mode === 'query') {
    return true
  }
  if (props.mode === 'update' && props.schema.readOnly) {
    return true
  }
  return false
}
//
const getProp = prop => {
  return prop
}
//
const error = computed(() => {
  return props.mode === 'query' ? null : props.errors[props.prop]
})
</script>
