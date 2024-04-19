<template>
  <div>{{ $t('中文') }}</div>
  <el-row><md name="home" /></el-row>
  <el-row>
    <editor v-model="model" upload-url="file/upload" />
  </el-row>
  <el-row>
    <chart :options="options" width="400px" height="200px" />
  </el-row>
  <h2>CKEditor</h2>
</template>
<script>
export default { name: '/home' }
</script>
<script setup>
import { ref, watch } from 'vue'

import Chart from '@/components/chart/index.vue'
import Editor from '@/components/editor/index.vue'
import Md from '@/components/markdown/index.vue'

const model = ref('')
watch(model, () => {
  console.log(model.value)
})

const options = {
  title: {
    text: '基础折线图',
  },
  xAxis: {
    type: 'category',
    data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
  },
  yAxis: {
    type: 'value',
  },
  series: [
    {
      data: [150, 230, 224, 218, 135, 147, 260],
      type: 'line',
    },
  ],
}
</script>
