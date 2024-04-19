import { dayjs } from 'element-plus'

import Mock from '@/lib/better-mock/mock.browser.esm.js'
import { log } from '@/utils/index.js'

function createItem() {
  return Mock.mock({
    id: Mock.Random.guid(),
    name: Mock.Random.name(),
    number: Mock.Random.string(),
    order: Mock.Random.inc(),
    isReadonly: false,
    createdAt: dayjs(Mock.Random.datetime(), 'YYYY-MM-DD HH:mm:ss').format(),
    modifiedAt: dayjs(Mock.Random.datetime(), 'YYYY-MM-DD HH:mm:ss').format(),
    rowVersion: Mock.Random.guid(),
  })
}

export default function () {
  Mock.mock('/api/role', 'POST', request => {
    log(`mock:${request.url}`)
    const { pageIndex = 1, pageSize = 10 } = JSON.parse(request.body ?? '{}')
    const totalCount = 10100
    const list = [
      {
        name: '管理员',
        number: 'admin',
        order: 0,
        isReadonly: true,
        createdAt: dayjs(
          Mock.Random.datetime(),
          'YYYY-MM-DD HH:mm:ss',
        ).format(),
        modifiedAt: dayjs(
          Mock.Random.datetime(),
          'YYYY-MM-DD HH:mm:ss',
        ).format(),
        rowVersion: Mock.Random.guid(),
      },
    ]
    for (let i = 0; i < totalCount - 1; i += 1) {
      list.push(createItem())
    }
    const result = {
      code: 0,
      data: {
        items: list.splice((pageIndex - 1) * pageSize, pageSize),
        pageIndex,
        pageSize,
        totalCount,
      },
    }
    return JSON.parse(JSON.stringify(result))
  })
}
