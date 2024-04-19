import qs from 'qs'

import settings from '@/config/settings.js'
import i18n from '@/locale/index.js'
import router from '@/router/index.js'
import { useTokenStore } from '@/store/index.js'
import { getFileNameFromContentDisposition } from '@/utils/index.js'

const messages = new Map([
  [200, '操作成功'],
  [201, '已创建'],
  [204, '无返回值'],
  [301, '永久重定向'],
  [302, '临时重定向'],
  [400, '请求参数错误'],
  [401, '未登录'],
  [403, '权限不足'],
  [415, '不支持的内容类型'],
  [500, '服务器异常'],
  [503, '服务不可用'],
])

function getUrl(url) {
  if (url.startsWith('http') || url.startsWith('/')) {
    return url
  }
  return url.startsWith('api/') ? url : `${settings.baseURL}/${url}`
}

const getOptions = async (method, url, data, customOptions, isUrlEncoded) => {
  url = getUrl(url)
  // 设置默认值
  const options = {
    method: method ?? 'POST',
    headers: {
      'Accept-Language': i18n.global.locale.value,
    },
  }
  // 合并自定义配置
  if (customOptions) {
    Object.assign(options, customOptions)
  }
  // 添加Token
  const tokenStore = useTokenStore()
  if (await tokenStore.isLogin()) {
    options.headers.Authorization = `Bearer ${tokenStore.accessToken}`
  }
  if (options.method === 'GET') {
    // GET 拼接URL参数
    if (data) {
      if (data instanceof String) {
        url = `${url}?${data}`
      } else {
        url = `${url}?${qs.stringify(data)}`
      }
    }
  } else if (data instanceof FormData) {
    // 上传参数
    delete options.headers['Content-Type']
    options.body = data
  } else if (isUrlEncoded) {
    // urlencoded
    options.headers['Content-Type'] = 'application/x-www-form-urlencoded'
    options.body = qs.stringify(data)
  } else {
    // json
    options.headers['Content-Type'] = 'application/json'
    options.body = JSON.stringify(data ?? {})
  }
  return {
    fullUrl: url,
    options,
  }
}

const getResult = async response => {
  const result = {
    ok: false,
    status: response.status,
  }
  let data = null
  const message = messages.get(response.status) ?? response.statusText
  if (response.status === 400) {
    // 400输入错误
    data = await response.json()
  } else if (response.status === 401) {
    // 401未登录
    // result = { code: response.status, message: messages.get(response.status) ?? response.statusText };
    router.push({
      path: '/login',
      query: { redirect: router.currentRoute.value.fullPath },
    })
  } else if (response.status === 403) {
    // 403权限不足
    // result = { code: response.status, message: messages.get(response.status) ?? response.statusText };
    router.push({
      path: '/403',
      query: { redirect: router.currentRoute.value.fullPath },
    })
  } else if (response.status === 500) {
    // 500服务端错误
    data = await response.json()
  } else {
    const contentType = response.headers.get('Content-Type')
    if (contentType?.indexOf('application/json') > -1) {
      data = await response.json()
    } else {
      const contentDisposition = response.headers.get('Content-Disposition')
      if (contentDisposition) {
        data = {
          file: await response.blob(),
          name: getFileNameFromContentDisposition(contentDisposition),
        }
      } else {
        data = await response.text()
      }
    }
  }
  if (Object.prototype.hasOwnProperty.call(data, 'code')) {
    result.code = data.code
    result.message = data.message
    result.data = data.data
  } else {
    result.data = data
    result.message = message
  }
  if (response.ok) {
    if (!result.code || result.code === 0) {
      result.ok = true
    }
  }
  return result
}

async function request(method, url, data, customOptions, isUrlEncoded = false) {
  // 规范化请求参数
  const { fullUrl, options } = await getOptions(
    method,
    url,
    data,
    customOptions,
    isUrlEncoded,
  )
  try {
    // 发送请求
    const response = await fetch(fullUrl, options)
    // 规范化返回值格式
    const result = await getResult(response)
    return result
  } catch (error) {
    return { ok: false, code: error.name, message: error.message, error }
  }
}

export default request
export { getUrl }
