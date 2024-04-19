import { createI18n } from 'vue-i18n'
import config from './config.js'
import { format } from '@/utils/index.js'

const i18n = createI18n(config)

const t = i18n.global.t

i18n.global.t = function (...args) {
  if (i18n.global.locale.value === 'zh-CN') {
    return format(args.shift(), args)
  }
  return t(...args)
}

console.log(i18n.t)

export default i18n
