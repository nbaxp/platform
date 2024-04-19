import { emailOrPhoneNumberRegex } from '@/utils/constants.js'

export default function () {
  return {
    url: 'user/forgot-password',
    labelWidth: 0,
    submitStyle: 'width:100%',
    properties: {
      emailOrPhoneNumber: {
        title: '邮箱或手机号码',
        rules: [
          {
            required: true,
          },
          {
            pattern: emailOrPhoneNumberRegex,
          },
          {
            validator: 'remote',
            url: 'user/has-email-or-phone-number',
            message: '{0} not exist',
          },
        ],
      },
      authCode: {
        title: '验证码',
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
      codeHash: {
        hidden: true,
      },
    },
  }
}
