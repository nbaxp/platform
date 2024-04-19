<template>
  <div style="display: inline-flex">
    <el-input v-model="model">
      <template #prefix>
        <el-icon v-if="icon" class="el-input__icon">
          <svg-icon :name="icon" />
        </el-icon>
      </template>
    </el-input>
    <el-image
      :title="$t('clickRefresh')"
      :src="src"
      style="cursor: pointer; max-height: 30px; margin-left: 10px"
      @click="onClick"
    >
      <template #placeholder>
        <span />
      </template>
      <template #error>
        <span />
      </template>
    </el-image>
  </div>
</template>
<script setup>
import { ref, watch, onMounted } from 'vue'
import request from '@/utils/request.js'
import SvgIcon from '@/components/icon/index.vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  icon: {
    type: String,
    default: '',
  },
  method: {
    type: String,
    default: 'POST',
  },
  url: {
    type: String,
    default: '',
  },
  authCode: {
    type: String,
    default: 'authCode',
  },
  codeHash: {
    type: String,
    default: 'codeHash',
  },
})
const emit = defineEmits(['update:modelValue', 'callback'])

const model = ref(props.modelValue)
watch(model, value => {
  emit('update:modelValue', value)
})

const src = ref('')
const load = async () => {
  const result = await request(props.method, props.url)
  src.value = result.data[props.authCode]
  emit('callback', result.data[props.codeHash])
}

const onClick = async () => {
  await load()
}

onMounted(async () => {
  await load()
})
</script>
