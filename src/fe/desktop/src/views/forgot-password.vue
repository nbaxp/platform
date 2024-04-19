<template>
  <el-container>
    <el-main
      style="display: flex; align-items: center; justify-content: center"
    >
      <div class="login">
        <div
          style="display: flex; align-items: center; justify-content: center"
        >
          <el-space><layout-logo /> <layout-locale /></el-space>
        </div>
        <el-card class="box-card">
          <el-row :gutter="40" style="min-width: 400px">
            <el-col>
              <app-form v-model="model" :schema="schema" @success="success" />
              <div
                style="
                  display: flex;
                  align-items: center;
                  justify-content: space-between;
                  height: 50px;
                "
              >
                <router-link style to="/login">
                  {{ $t('登录') }}
                </router-link>
                <router-link style to="/register">
                  {{ $t('注册') }}
                </router-link>
              </div>
            </el-col>
          </el-row>
        </el-card>
        <layout-footer />
      </div>
    </el-main>
  </el-container>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

import AppForm from '@/components/form/index.vue'
import LayoutFooter from '@/layout/footer.vue'
import LayoutLocale from '@/layout/locale.vue'
import LayoutLogo from '@/layout/logo.vue'
import useSchema from '@/models/forgot-password.js'
import { schemaToModel } from '@/utils/index.js'

const schema = useSchema()
const model = ref(schemaToModel(schema))
const router = useRouter()
const success = result => {
  router.push('/login')
}
</script>
