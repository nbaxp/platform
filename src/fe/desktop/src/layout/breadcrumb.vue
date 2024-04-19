<template>
  <el-row class="breadcrumb">
    <el-breadcrumb>
      <template v-for="item in $route.matched">
        <el-breadcrumb-item
          v-if="!item.meta?.hideInMenu"
          :key="item.path"
          :to="{ path: item.path }"
        >
          {{ $t(getTitle(item)) }}
        </el-breadcrumb-item>
      </template>
    </el-breadcrumb>
  </el-row>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { camelCase } from '@/utils/index.js'

const router = useRouter()
const getTitle = route => {
  if (route.redirect) {
    return router.getRoutes().find(o => o.path === route.redirect)?.meta?.title
  }
  return route.meta?.title
}
</script>
