export default function (
  properties = {},
  includeAll = false,
  orderBy = '',
  filters = [],
) {
  return {
    type: 'object',
    title: 'query',
    properties: {
      pageIndex: {
        type: 'number',
        default: 1,
      },
      pageSize: {
        type: 'number',
        default: 10,
      },
      filters: {
        type: 'array',
        properties: {},
        default: filters,
      },
      orderBy: {
        type: 'string',
        default: orderBy,
      },
      includeAll: {
        type: 'boolean',
        default: includeAll,
      },
      query: {
        type: 'object',
        properties,
      },
      totalCount: {
        type: 'number',
        default: 0,
      },
      pageSizeOptions: {
        type: 'array',
        default: [10, 100, 1000],
      },
    },
  }
}
