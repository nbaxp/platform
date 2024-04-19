import useQuery from './query.js'
import useExport from './export.js'
import useImport from './import.js'

const properties = {
  id: {
    hidden: true,
  },
  name: {
    title: '名称',
  },
  number: {
    title: '编号',
    readonly: true,
  },
  parentId: {
    title: '上级',
    type: 'string',
    input: 'cascader',
    checkStrictly: true,
    url: 'department/search',
    hideInList: true,
  },
}

const schema = {
  properties: {
    query: useQuery(properties, true),
    list: {
      isTree: true,
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
