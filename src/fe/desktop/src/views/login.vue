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
          <el-row :gutter="40" style="width: 600px">
            <el-col
              :span="10"
              style="
                border-right: 1px solid var(--el-border-color);
                display: flex;
                align-items: center;
                justify-content: center;
              "
            >
              <div style="text-align: center">
                <div>{{ $t('扫码登录') }}</div>
                <img
                  v-if="qrcode"
                  style="cursor: pointer"
                  :src="qrcode"
                  @click="getQrCode"
                />
              </div>
            </el-col>
            <el-col :span="14">
              <el-tabs>
                <el-tab-pane :label="$t('密码登录')">
                  <app-form
                    ref="passwordForm"
                    v-model="passwordModel"
                    :schema="config.properties.passwordLogin"
                    @success="success"
                    @error="error"
                  />
                </el-tab-pane>
                <el-tab-pane :label="$t('短信登录')">
                  <app-form
                    v-model="smsModel"
                    :schema="config.properties.smsLogin"
                    @success="success"
                  />
                </el-tab-pane>
              </el-tabs>
              <div
                style="
                  display: flex;
                  align-items: center;
                  justify-content: space-between;
                  height: 50px;
                "
              >
                <router-link style to="/register">
                  {{ $t('注册') }}
                </router-link>
                <router-link style to="/forgot-password">
                  {{ $t('忘记密码') }}
                </router-link>
              </div>
              <el-divider>{{ $t('其他登录') }}</el-divider>
              <el-space
                :size="24"
                style="
                  display: flex;
                  align-items: center;
                  justify-content: center;
                "
              >
                <svg-icon
                  v-for="item in config.properties.externalLogin"
                  :key="item.name"
                  size="24"
                  :name="item.icon"
                  class="is-circle"
                />
              </el-space>
            </el-col>
          </el-row>
        </el-card>
        <layout-footer />
      </div>
    </el-main>
  </el-container>
</template>

<script setup>
import QRCode from 'qrcode'
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

import AppForm from '@/components/form/index.vue'
import SvgIcon from '@/components/icon/index.vue'
import LayoutFooter from '@/layout/footer.vue'
import LayoutLocale from '@/layout/locale.vue'
import LayoutLogo from '@/layout/logo.vue'
import useModel from '@/models/login.js'
import { useAppStore, useTokenStore } from '@/store/index.js'
import { schemaToModel } from '@/utils/index.js'

const config = ref(useModel())
const passwordForm = ref(null)
const qrcode = ref(null)
const passwordModel = ref(schemaToModel(config.value.properties.passwordLogin))
const smsModel = ref(schemaToModel(config.value.properties.smsLogin))
const router = useRouter()
const appStore = useAppStore()
const tokenStore = useTokenStore()
const success = async result => {
  tokenStore.setToken(result.data.access_token, result.data.refresh_token)
  // await useUserStore().getUserInfo();
  await appStore.connection?.invoke('Login')
  const redirect = router.currentRoute.value.query?.redirect ?? '/'
  router.push(redirect)
}
const error = () => {
  console.log(passwordForm.value)
}
const getQrCode = async () => {
  const result = await QRCode.toDataURL(new Date().toString())
  return result
}
passwordModel.value.userName = 'admin'
passwordModel.value.password = '123456'

onMounted(async () => {
  qrcode.value = await getQrCode()
})
</script>
