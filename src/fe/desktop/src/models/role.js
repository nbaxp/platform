import useQuery from './query.js'
import useExport from './export.js'
import useImport from './import.js'

const properties = {
  id: {
    hidden: true,
  },
  name: { title: '名称' },
  number: { title: '编号' },
  permissions: {
    type: 'array',
    input: 'cascader',
    multiple: true,
    //checkStrictly: true,
    label: 'path',
    url: 'permission/search',
    label: 'name',
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

export default function () {
  return schema
}
