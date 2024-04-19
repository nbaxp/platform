<template>
  <v-chart
    v-if="renderChart"
    :option="options"
    :autoresize="autoresize"
    :style="{ width, height }"
  />
</template>

<script setup>
import {
  BarChart,
  CandlestickChart,
  FunnelChart,
  LineChart,
  ParallelChart,
  PieChart,
  RadarChart,
  ScatterChart,
  TreemapChart,
} from 'echarts/charts'
import {
  DataZoomComponent,
  GraphicComponent,
  GridComponent,
  LegendComponent,
  TitleComponent,
  ToolboxComponent,
  TooltipComponent,
} from 'echarts/components'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { nextTick, ref } from 'vue'
import VChart from 'vue-echarts'

defineProps({
  options: {
    type: Object,
    default: () => {},
  },
  autoresize: {
    type: Boolean,
    default: true,
  },
  width: {
    type: String,
    default: '100%',
  },
  height: {
    type: String,
    default: '100%',
  },
})

const renderChart = ref(false)
nextTick(() => {
  renderChart.value = true
})
use([
  CanvasRenderer,
  BarChart,
  TreemapChart,
  ParallelChart,
  LineChart,
  PieChart,
  RadarChart,
  FunnelChart,
  ScatterChart,
  CandlestickChart,
  GridComponent,
  TooltipComponent,
  LegendComponent,
  DataZoomComponent,
  GraphicComponent,
  ToolboxComponent,
  TitleComponent,
])
</script>
