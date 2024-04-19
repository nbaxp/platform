<template>
  <el-card style="min-width: 20vw; width: 400px">
    <app-form v-model="model" :schema="schema" @success="success" />
  </el-card>
</template>

<script setup>
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessageBox } from 'element-plus'
import useSchema from '@/models/reset-password.js'
import { schemaToModel } from '@/utils/index.js'
import AppForm from '@/components/form/index.vue'

const schema = ref(useSchema())
const model = ref(schemaToModel(schema.value))
const { t } = useI18n()
const success = result => {
  ElMessageBox.alert(t('success'), t('tip'))
}
</script>
