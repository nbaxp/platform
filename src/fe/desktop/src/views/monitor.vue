<template>
  <el-row :gutter="20" style="margin-bottom: 20px">
    <el-col :span="24">
      <el-card class="box-card">
        <template #header>
          <div class="card-header">
            <span> {{ t('主机') }}</span>
          </div>
        </template>
        <el-descriptions border>
          <el-descriptions-item :label="t('服务器时间')">
            {{ dayjs(model.serverTime).format('YYYY-MM-DD HH:mm:ss') }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('CPU总数')">
            {{ model.cpuCount }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('内存合计')">
            {{ bytesFormat(model.memoryTotal) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('OS架构')">
            {{ model.osArchitecture }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('OS简介')">
            {{ model.osDescription }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('主机名称')">
            {{ model.hostName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('运行时标识')">
            {{ model.runtimeIdentifier }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('用户名')">
            {{ model.userName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('进程数')">
            {{ model.processCount }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('主机地址')">
            {{ model.hostAddresses }}
          </el-descriptions-item>
        </el-descriptions>
      </el-card>
    </el-col>
  </el-row>
  <el-row :gutter="20" style="margin-bottom: 20px">
    <el-col :span="12">
      <el-card class="box-card">
        <chart :option="cpuModel" height="300px" />
      </el-card>
    </el-col>
    <el-col :span="12">
      <el-card class="box-card">
        <chart :option="memoryModel" height="300px" />
      </el-card>
    </el-col>
  </el-row>
  <el-row :gutter="20" style="margin-bottom: 20px">
    <el-col :span="24">
      <el-card class="box-card">
        <template #header>
          <div class="card-header">
            <span> {{ t('进程') }}</span>
          </div>
        </template>
        <el-descriptions border :column="3">
          <el-descriptions-item :label="t('进程架构')">
            {{ model.processArchitecture }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('进程Id')">
            {{ model.processId }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('进程名称')">
            {{ model.processName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('进程参数')">
            {{ model.processArguments }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('句柄数量')">
            {{ model.processHandleCount }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('进程文件')">
            {{ model.processFileName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('驱动器名称')">
            {{ model.driveName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('驱动器大小')">
            {{ bytesFormat(model.drivieTotalSize) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('剩余空间')">
            {{ bytesFormat(model.driveAvailableFreeSpace) }}
          </el-descriptions-item>
        </el-descriptions>
      </el-card>
    </el-col>
  </el-row>
  <el-row :gutter="20" style="margin-bottom: 20px">
    <el-col :span="24">
      <el-card class="box-card">
        <template #header>
          <div class="card-header">
            <span> {{ t('Framework') }}</span>
          </div>
        </template>
        <el-descriptions border :column="3">
          <el-descriptions-item :label="t('版本')">
            {{ model.framework }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('异常数量')">
            {{ model.exceptionCount }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('请求总数')">
            {{ model.totalRequests }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('接收数据')">
            {{ bytesFormat(model.bytesReceived) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('发送数据')">
            {{ bytesFormat(model.bytesSent) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('请求总数')">
            {{ persentFormat(model.totalRequests) }}
          </el-descriptions-item>
        </el-descriptions>
      </el-card>
    </el-col>
  </el-row>
</template>

<script setup>
import { reactive, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import Chart from '@/components/chart/index.vue'
import { persentFormat, bytesFormat } from '@/utils/index.js'
import { dayjs } from 'element-plus'

const hasEventSource = !!window.EventSource
const { t } = useI18n()

const model = reactive({})

const cpuModel = reactive({
  title: {
    text: t('cpuUsage'),
  },
  xAxis: {
    type: 'category',
    data: Object.keys(Array(30).fill()),
  },
  yAxis: {
    type: 'value',
    min: 0,
    max: 100,
  },
  series: [
    {
      data: [],
      type: 'line',
      smooth: true,
    },
  ],
})

const memoryModel = reactive({
  title: {
    text: t('memoryUsage'),
  },
  xAxis: {
    type: 'category',
    data: Object.keys(Array(30).fill()),
  },
  yAxis: {
    type: 'value',
    min: 0,
    max: 100,
  },
  series: [
    {
      data: [],
      type: 'line',
      smooth: true,
    },
  ],
})

const update = data => {
  Object.assign(model, data)
  // cpu
  if (cpuModel.series[0].data.length > 60) {
    cpuModel.series[0].data.shift()
  }
  cpuModel.title.text = `${t('cpuUsage')}:${persentFormat(model.cpuUsage / 100)}`
  cpuModel.series[0].data.push(data.cpuUsage)
  // memory
  if (memoryModel.series[0].data.length > 60) {
    memoryModel.series[0].data.shift()
  }
  memoryModel.title.text = `${t('memoryUsage')}:${persentFormat(model.memoryUsage / 100)}`
  memoryModel.series[0].data.push(data.memoryUsage)
}

const connect = () => {
  const es = new EventSource('/api/monitor/index')
  es.onmessage = event => {
    update(JSON.parse(event.data))
  }
  es.onerror = e => {
    es.close()
    console.log(e)
    setTimeout(connect, 5 * 1000)
  }
}

onMounted(() => {
  if (hasEventSource) {
    try {
      connect()
    } catch (e) {
      console.log(e)
      setTimeout(connect, 5 * 1000)
    }
  }
})
</script>
