<template>
  <el-icon>
    <component :is="svg" />
  </el-icon>
</template>

<script setup>
import { computed, defineAsyncComponent } from 'vue'

const props = defineProps({
  name: {
    type: String,
    default: '',
  },
})
const components = import.meta.glob('../../assets/icons/**/*.svg')
const svg = computed(() => {
  return defineAsyncComponent(
    components[`../../assets/icons/${props.name}.svg`] ??
      components['../../assets/icons/file.svg'],
  )
})
</script>
