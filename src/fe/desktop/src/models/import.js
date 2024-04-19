export default function () {
  return {
    type: 'object',
    title: '导入',
    properties: {
      update: {
        title: '更新已存在',
        type: 'boolean',
      },
      files: {
        title: '文件',
        type: 'array',
        multiple: true,
        input: 'file',
        accept: '.xlsx',
        default: [],
        limit: 10,
        size: 100 * 1024 * 1024,
        rules: [
          {
            required: true,
            trigger: 'change',
          },
        ],
      },
    },
  }
}
