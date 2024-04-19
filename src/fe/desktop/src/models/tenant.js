import useQuery from './query.js'
import useExport from './export.js'
import useImport from './import.js'

export default function () {
  const properties = {
    id: {
      hidden: true,
    },
    name: {
      title: '名称',
      rules: [
        {
          required: true,
        },
        {
          pattern: '[^u4e00-u9fa5_a-zA-Z0-9$]+',
        },
      ],
    },
    number: {
      title: '编号',
      readonly: true,
      rules: [
        {
          required: true,
        },
        {
          pattern: '[\\w]{4,64}',
        },
      ],
    },
    disabled: {
      title: '禁用',
      type: 'boolean',
    },
    permissions: {
      title: '权限',
      type: 'array',
      input: 'cascader',
      multiple: true,
      checkStrictly: true,
      label: 'path',
      url: 'permission/search',
      label: 'name',
      value: 'number',
      hideInList: true,
      hideInQuery: true,
    },
  }
  return {
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
        properties,
      },
      update: {
        title: '更新',
        properties,
      },
      export: useExport(),
      import: useImport(),
    },
  }
}
