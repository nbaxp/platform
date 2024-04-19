import useQuery from './query.js'
import useExport from './export.js'
import useImport from './import.js'
import { emailRegex, phoneNumberRegex } from '@/utils/constants.js'

const properties = {
  id: {
    hidden: true,
  },
  userName: {
    title: '用户名',
    readonly: true,
    rules: [{ required: true }],
  },
  password: {
    input: 'password',
    hideInList: true,
  },
  email: {
    title: '邮箱',
    roles: [
      {
        pattern: emailRegex,
      },
    ],
  },
  phoneNumber: {
    title: '手机号',
    roles: [
      {
        pattern: phoneNumberRegex,
      },
    ],
  },
  avatar: {
    title: '头像',
    input: 'upload',
    isImage: true,
    url: 'file/upload',
    accept: '.svg,.png',
  },
  departmentId: {
    title: '部门',
    input: 'cascader',
    url: 'department/search',
    checkStrictly: true,
  },
  roles: {
    title: '角色',
    type: 'array',
    input: 'select',
    multiple: true,
    url: 'role/search',
    hideInList: true,
    hideInQuery: true,
  },
}

const schema = {
  properties: {
    query: useQuery(properties),
    list: {
      properties,
    },
    details: {
      title: '详情',
      properties,
    },
    create: {
      title: '新建',
      properties: Object.assign({}, properties, {
        password: {
          rules: [
            {
              required: true,
            },
          ],
        },
      }),
    },
    update: {
      title: '更新',
      properties,
    },
    export: useExport(),
    import: useImport(),
  },
}

export default function () {
  return schema
}
