<template>
  <app-list
    v-if="config"
    style="height: 100%"
    :config="config"
    @command="onCommand"
  />
</template>
<script setup>
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'

import AppList from '@/components/list/index.vue'

const modules = import.meta.glob('../models/**/*.js')
const route = useRoute()
const config = ref(null)
const onCommand = async (item, rows) => {
  console.log(item.path, item, rows)
}
onMounted(async () => {
  const defaultExport = (
    await (
      await modules[`../models/${route.meta.schema}.js`]
    )()
  ).default
  config.value =
    typeof defaultExport === 'function' ? defaultExport() : defaultExport
})
</script>
