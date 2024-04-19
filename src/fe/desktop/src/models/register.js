import { emailOrPhoneNumberRegex } from '@/utils/constants.js'

export default function () {
  const properties = {}
  return {
    url: 'user/register',
    labelWidth: 0,
    submitStyle: 'width:100%',
    properties: {
      userName: {
        title: '用户名',
        rules: [
          {
            required: true,
          },
          {
            pattern: '[a-zA-Z0-9-_]{4,64}',
          },
          {
            validator: 'remote',
            url: 'user/no-user',
            message: '{0} has already used',
          },
        ],
      },
      password: {
        title: '密码',
        input: 'password',
        rules: [
          {
            required: true,
          },
        ],
      },
      confirmPassword: {
        title: '确认密码',
        input: 'password',
        rules: [
          {
            validator: 'compare',
            compare: 'password',
          },
        ],
      },
      emailOrPhoneNumber: {
        title: '邮箱或手机号',
        rules: [
          {
            required: true,
          },
          {
            pattern: emailOrPhoneNumberRegex,
          },
          {
            validator: 'remote',
            url: 'user/no-email-or-phone-number',
            message: '{0} 已存在',
          },
        ],
      },
      authCode: {
        title: '验证码',
        icon: 'auth',
        input: 'code-captcha',
        url: 'captcha/code',
        timeout: 120,
        query: 'emailOrPhoneNumber',
        regexp: emailOrPhoneNumberRegex,
        rules: [
          {
            required: true,
          },
        ],
      },
      codeHash: {
        hidden: true,
      },
    },
  }
}
