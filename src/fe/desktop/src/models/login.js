export default function () {
  return {
    properties: {
      passwordLogin: {
        title: '密码登录',
        url: 'token/create',
        labelWidth: 0,
        submitStyle: 'width:100%',
        properties: {
          tenantNumber: {
            title: '租户',
            input: 'select',
            url: 'tenant/search',
            value: 'number',
            //hidden: true,
          },
          userName: {
            title: '用户名',
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
          authCode: {
            title: '验证码',
            input: 'image-captcha',
            url: 'captcha/image',
            rules: [
              {
                required: true,
              },
            ],
          },
          codeHash: {
            hidden: true,
          },
          rememberMe: {
            title: '记住我',
            type: 'boolean',
            showLabel: true,
          },
        },
      },
      smsLogin: {
        title: '短信登录',
        url: 'token/create',
        method: 'POST',
        labelWidth: 0,
        submitStyle: 'width:100%',
        properties: {
          phoneNumber: {
            title: '手机号码',
            icon: 'user',
            rules: [
              {
                required: true,
              },
            ],
          },
          verifyCode: {
            title: '验证码',
            input: 'password',
            icon: 'password',
            rules: [
              {
                required: true,
              },
            ],
          },
        },
      },
      externalLogin: [
        {
          name: 'qq',
          icon: 'qq',
        },
        {
          name: 'weixin',
          icon: 'weixin',
        },
        {
          name: 'weibo',
          icon: 'weibo',
        },
      ],
    },
  }
}
