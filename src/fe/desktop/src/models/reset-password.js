export default function () {
  return {
    url: 'user/reset-password',
    properties: {
      currentPassword: {
        title: '当前密码',
        input: 'password',
        rules: [
          {
            required: true,
          },
        ],
      },
      newPassword: {
        title: '新密码',
        input: 'password',
        rules: [
          {
            required: true,
          },
        ],
      },
      confirmNewPassword: {
        title: '确认新密码',
        input: 'password',
        rules: [
          {
            validator: 'compare',
            compare: 'newPassword',
          },
        ],
      },
    },
  }
}
