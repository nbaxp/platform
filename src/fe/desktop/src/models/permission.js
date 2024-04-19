import useQuery from './query.js'
import useExport from './export.js'
import useImport from './import.js'

const properties = {
  id: {
    hidden: true,
  },
  name: { title: '名称' },
  number: { title: '编号' },
  icon: {
    name: { title: '图标' },
    input: 'icon',
  },
  order: { title: '序号' },
  disabled: {
    title: '禁用',
    type: 'boolean',
  },
  parentId: {
    title: '上级',
  },
}

const schema = {
  properties: {
    query: useQuery(properties, true, 'order'),
    list: {
      isTree: true,
      key: 'name',
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
