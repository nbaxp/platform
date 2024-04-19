<template>
  <div style="display: inline-flex; width: 100%">
    <el-input
      v-model="model"
      style="min-width: 50%"
      :placeholder="$t('验证码')"
    >
      <template #prefix>
        <el-icon v-if="icon" class="el-input__icon">
          <svg-icon :name="icon" />
        </el-icon>
      </template>
    </el-input>
    <el-button
      :title="$t('clickRefresh')"
      style="max-height: 30px; margin-left: 10px"
      :disabled="disabled"
      @click="onClick"
    >
      <template v-if="!loading">
        {{ $t('发送验证码') }}
      </template>
      <template v-else>
        {{ $t('resend', [seconds]) }}
      </template>
    </el-button>
  </div>
</template>
<script setup>
import { ref, watch, onMounted, computed } from 'vue'
import { delay } from '@/utils/index.js'
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
    default: 'captcha/code',
  },
  code: {
    type: String,
    default: 'code',
  },
  codeHash: {
    type: String,
    default: 'codeHash',
  },
  timeout: {
    type: Number,
    default: 120,
  },
  query: {
    type: String,
    default: '',
  },
  regexp: {
    type: String,
    default: '',
  },
})
const emit = defineEmits(['update:modelValue', 'callback'])

const model = ref(props.modelValue)
watch(model, value => {
  emit('update:modelValue', value)
})

//1.附加字段为空不可点击
//2.获取验证码 | 120后重新获取（验证信息：验证码已过期，请重新获取）
//3.120秒内输入有效
//4.template:【company】xxxx (登录验证码)

const loading = ref(false)
const seconds = ref(props.timeout)
const disabled = computed(() => {
  return new RegExp(props.regexp).test(props.query) === false || loading.value
})

const onClick = async () => {
  if (!loading.value) {
    loading.value = true
    try {
      const result = await request(props.method, props.url, props.query)
      emit('callback', result.data[props.codeHash])
      const expires = new Date(result.data.expires)
      seconds.value = props.timeout
      let now = new Date()
      while (now < expires) {
        await delay(500)
        seconds.value = parseInt((expires - now) / 1000)
        now = new Date()
      }
    } finally {
      loading.value = false
    }
  }
}
</script>
