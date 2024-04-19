import en from './en-US.js'
import zh from './zh-CN.js'

export default {
  legacy: false,
  locale: 'zh-CN',
  fallbackLocale: 'en-US',
  options: [
    {
      key: 'zh-CN',
      value: '中文（中国）',
    },
    {
      key: 'en-US',
      value: 'English (United States)',
    },
  ],
  messages: {
    'zh-CN': zh,
    'en-US': en,
  },
}
