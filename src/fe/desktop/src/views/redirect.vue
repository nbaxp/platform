<template>...</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const redirect = router.currentRoute.value.query?.redirect ?? '/'
onMounted(() => {
  router.push(redirect)
})
</script>
