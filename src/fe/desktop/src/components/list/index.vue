<template>
  <div v-loading="loading" style="height: 100%" v-if="!refresh">
    <template v-if="config">
      <el-card style="height: 100%">
        <div style="height: 100%; display: flex; flex-direction: column">
          <app-form
            ref="queryFormRef"
            v-model="queryModel.query"
            inline
            mode="query"
            label-position="left"
            :schema="querySchema.properties.query"
            :hide-button="true"
          />
          <div v-if="tableButtons.length" style="padding-bottom: 20px">
            <template v-for="item in tableButtons" :key="item.path">
              <el-button
                :type="getButtonType(item.meta?.path)"
                @click="
                  click(
                    item,
                    queryModel.items.filter(o => o.checked),
                  )
                "
              >
                <template #default>
                  <svg-icon v-if="item.meta?.icon" :name="item.meta?.icon" />
                  <span v-if="item.meta?.title">{{
                    $t(item.meta?.title)
                  }}</span>
                </template>
              </el-button>
            </template>
            <el-button type="primary" style="float: right" @click="print">{{
              $t('打印')
            }}</el-button>
            <el-button
              type="primary"
              style="float: right"
              @click="drawerVisible = true"
              >{{ $t('过滤') }}</el-button
            >
            <el-button type="primary" style="float: right" @click="reset">{{
              $t('重置')
            }}</el-button>
          </div>
          <div style="flex: 1; overflow: auto">
            <el-auto-resizer>
              <template #default="{ height, width }">
                <el-table-v2
                  id="table"
                  v-model:sort-state="sortModel"
                  :columns="columns"
                  :data="listModel"
                  :expand-column-key="listSchema.key ?? 'name'"
                  :width="width"
                  :height="height"
                  fixed
                  @column-sort="onSort"
                />
              </template>
            </el-auto-resizer>
          </div>
          <el-pagination
            v-if="!queryModel.includeAll"
            v-model:currentPage="queryModel.pageIndex"
            v-model:page-size="queryModel.pageSize"
            :small="appStore.settings.size === 'small'"
            :total="queryModel.totalCount"
            :page-sizes="queryModel.pageSizeOptions"
            :background="true"
            layout="total, sizes, prev, pager, next, jumper"
            style="margin-top: 20px"
            @size-change="onPageSizeChange"
            @current-change="onPageIndexChange"
          />
        </div>
      </el-card>
    </template>
    <el-drawer
      v-model="drawerVisible"
      :title="$t('过滤')"
      size="auto"
      destroy-on-close
    >
      <template v-for="(item, i) in columns" :key="item">
        <div
          v-if="item.title"
          style="display: flex; align-items: center"
          :draggable="item.draggable"
          @dragstart.stop="dragStartIndex = i"
          @dragenter="$event.preventDefault()"
          @dragover="$event.preventDefault()"
          @drop.prevent="drop($event, i)"
        >
          <el-icon
            :style="{ cursor: item.draggable ? 'pointer' : 'not-allowed' }"
            style="margin-right: 5px"
          >
            <i class="i-ri-drag-move-fill" />
          </el-icon>
          <el-checkbox
            v-model="item.show"
            :label="item.title"
            @change="filterChange(item)"
          />
        </div>
      </template>
      <template #footer>
        <el-button type="primary" @click="selectAll">
          {{ $t('全选') }}
        </el-button>
        <el-button type="primary" @click="invertSelect">
          {{ $t('反选') }}
        </el-button>
        <el-button type="primary" @click="resetColumns">{{
          $t('重置')
        }}</el-button>
      </template>
    </el-drawer>
    <el-dialog
      v-if="dialogVisible"
      v-model="dialogVisible"
      :close-on-click-modal="false"
      align-center
      append-to-body
      lock-scroll
      destroy-on-close
    >
      <template #header>{{ $t(dialogSchema.title) }}</template>
      <el-button
        link
        style="height: 50px"
        v-if="
          dialogSchema.action === 'import' &&
          buttons.find(o => o.meta?.command === 'import-template')
        "
        type="primary"
        @click="click(buttons.find(o => o.meta?.command === 'import-template'))"
      >
        {{ $t('模板') }}
      </el-button>
      <app-form
        ref="dialogFormRef"
        v-model="dialogModel"
        :schema="dialogSchema"
        label-position="left"
        :hide-button="true"
        :mode="dialogSchema.action"
        :before-submit="beforeSubmit"
        @success="success"
      />
      <template #footer>
        <span class="dialog-footer">
          <el-button type="primary" @click="dialogConfirm(dialogSchema.action)">
            {{ $t('确定') }}
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>
<script lang="jsx" setup>
import { ElMessage, ElMessageBox } from 'element-plus'
import { inject, provide, onMounted, ref, unref, nextTick } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import { camelCase } from '@/utils/index.js'
import AppForm from '@/components/form/index.vue'
import AppFormInput from '@/components/form/form-input.vue'
import SvgIcon from '@/components/icon/index.vue'
import { useAppStore } from '@/store/index.js'
import { listToTree, log, schemaToModel, downloadFile } from '@/utils/index.js'
import request from '@/utils/request.js'
import printJS from 'print-js'
import html2canvas from 'html2canvas'

const props = defineProps({
  config: {
    type: Object,
    default: null,
  },
  routeValue: {
    type: Array,
    default: null,
  },
})

const routeData = inject('routeData')
const { t } = useI18n()
const route = useRoute()
const appStore = useAppStore()

const refresh = ref(false)
const loading = ref(true)
const queryFormRef = ref(null)
const dialogFormRef = ref(null)
const drawerVisible = ref(false)
const dialogVisible = ref(false)
const dialogModel = ref(null)
const dialogSchema = ref(null)
const buttons = ref((props.routeValue ?? route).meta?.buttons ?? [])
const tableButtons = ref(
  buttons.value?.filter(o => !o.meta?.hidden && o.meta?.buttonType === 'table'),
)
const querySchema = ref(props.config.properties.query)
const queryModel = ref(schemaToModel(querySchema.value))
const listSchema = ref(props.config.properties.list)
const listModel = ref([])
const columns = ref([])
const originalColumns = ref([])
const dragStartIndex = ref(null)
const sortModel = ref(
  (() => {
    const result = {}
    ;(queryModel.value.orderBy ?? '')
      .split(',')
      .map(o => o.trim())
      .filter(o => o)
      .forEach(o => {
        const [columnName, direction = 'asc'] = o.split(' ')
        result[columnName] = direction
      })
    return result
  })(),
)

const drop = (e, index) => {
  if (dragStartIndex.value !== index) {
    const source = columns.value[dragStartIndex.value]
    columns.value.splice(dragStartIndex.value, 1)
    columns.value.splice(index, 0, source)
  }
}

const filterChange = item => {
  item.hidden = !item.show
}

const selectAll = () => {
  columns.value.forEach(o => {
    o.hidden = false
    o.show = !o.hidden
  })
}

const invertSelect = () => {
  columns.value.forEach(o => {
    o.hidden = !o.hidden
    o.show = !o.hidden
  })
}

const resetColumns = () => {
  if (originalColumns.value) {
    columns.value = [...originalColumns.value]
    columns.value.forEach(o => {
      o.hidden = false
      o.show = !o.hidden
    })
  }
}

const buildQuery = () => {
  const data = {}
  Object.entries(unref(queryModel)).forEach(([key, value]) => {
    if (key !== 'totalCount' && key !== 'items' && key !== 'pageSizeOptions') {
      if (value !== null) {
        data[key] = value
      }
    }
  })
  data.orderBy = Object.entries(sortModel.value)
    .map(([key, order]) => `${key} ${order}`)
    .join(',')
  return data
}

const load = async () => {
  loading.value = true
  try {
    const data = buildQuery()
    const method =
      buttons.value.find(button => button.meta?.command === 'search')?.meta
        ?.method ?? 'POST'
    const url = buttons.value.find(button => button.meta?.command === 'search')
      ?.meta?.url

    const result = await request(method, url, data)
    if (result.ok) {
      const { items, pageIndex, pageSize } = result.data
      //避免对querymodel重新赋值导致reset失效
      queryModel.value.pageIndex = pageIndex
      queryModel.value.pageSize = pageSize
      queryModel.value.items = items
      listModel.value = listSchema.value.isTree ? listToTree(items) : items
      const rowNumberWidth = `${pageIndex * pageSize}`.length * 8 + 16
      const rowNumberRow = columns.value.find(o => o.key === 'rowNumber')
      rowNumberRow.width =
        rowNumberWidth > rowNumberRow.width
          ? rowNumberWidth
          : rowNumberRow.width
    } else {
      if (result.code === 500) {
        await ElMessageBox.confirm(result.message, t('tip'), {
          type: 'warning',
        })
      }
    }
  } catch (e) {
    log(e)
  } finally {
    loading.value = false
  }
}

const reload = async () => {
  routeData.clear()
  queryModel.value.pageIndex = 1
  refresh.value = true
  nextTick(async () => {
    refresh.value = false
    await load()
  })
}

const reset = async () => {
  queryFormRef.value.reset()
  await reload()
}

const onPageIndexChange = async () => {
  await load()
}

const onPageSizeChange = async () => {
  await reload()
}

const onSort = async ({ key, order }) => {
  if (!order) {
    sortModel.value[key] = 'asc'
  } else if (sortModel.value[key] === 'asc') {
    sortModel.value[key] = order
  } else {
    delete sortModel.value[key]
  }

  await load()
}

const click = async (button, rows) => {
  const { command } = button.meta
  if (command === 'search') {
    await load()
    return
  }
  if (command === 'import-template') {
    loading.value = true
    try {
      const method = button.meta?.method ?? 'POST'
      const url = button.meta?.url
      const result = await request(method, url)
      if (result.ok) {
        dialogVisible.value = false
        downloadFile(result.data.file, result.data.name)
      } else {
        if (result.code === 500) {
          await ElMessageBox.confirm(result.message, t('tip'), {
            type: 'warning',
          })
        }
      }
    } catch (e) {
      log(e)
    } finally {
      loading.value = false
    }
  }
  if (command === 'delete') {
    const data = rows.filter(o => o.checked).map(o => o.id)
    if (data.length === 0) {
      return
    }
    try {
      loading.value = true
      await ElMessageBox.confirm(
        t('confirmDelete', [data.length]),
        t('warning'),
        {
          confirmButtonText: t('confirm'),
          cancelButtonText: t('cancel'),
          type: 'warning',
        },
      )
      const method = button.meta?.method ?? 'POST'
      const url = button.meta?.url
      const result = await request(method, url, data)
      if (result.ok) {
        await reload()
      }
    } catch (error) {
      if (error === 'cancel') {
        ElMessage({
          type: 'info',
          message: 'Cancel',
        })
      }
    } finally {
      loading.value = false
    }
    return
  }
  if (
    command === 'create' ||
    command === 'update' ||
    command === 'import' ||
    command === 'export' ||
    command === 'details'
  ) {
    const schema = props.config.properties[command]
    schema.action = command
    schema.method = button.meta?.method
    schema.url = button.meta?.url
    dialogSchema.value = schema
    if (command === 'details' || command === 'update') {
      const detailsButton = buttons.value.find(
        button => button.meta?.command === 'details',
      )
      const method = detailsButton?.meta?.method ?? 'POST'
      const url = detailsButton?.meta?.url
      const data = rows[0].id
      try {
        loading.value = true
        const result = await request(method, url, data)
        if (result.ok) {
          dialogModel.value = result.data
        } else {
          if (result.code === 500) {
            await ElMessageBox.confirm(result.message, t('tip'), {
              type: 'warning',
            })
          }
        }
      } finally {
        loading.value = false
      }
    } else {
      dialogModel.value = schemaToModel(schema)
    }
    dialogVisible.value = true
  }
  console.log(button)
  console.log(rows)
}

const beforeSubmit = (action, model) => {
  if (action === 'import') {
    const data = new FormData()
    Object.keys(dialogModel.value).forEach(key => {
      const schema = dialogSchema.value.properties[key]
      if (schema?.type === 'array') {
        dialogModel.value[key].forEach(value => {
          data.append(key, schema.input === 'file' ? value.raw : value)
        })
      } else {
        const value = dialogModel.value[key]
        data.append(key, schema.input === 'file' ? value.raw : value)
      }
    })
    return data
  } else if (action === 'export') {
    const data = buildQuery()
    Object.assign(data, dialogModel.value)
    return data
  }
}

const dialogConfirm = async action => {
  console.log(dialogFormRef.value)
  if (action === 'details') {
    dialogVisible.value = false
  } else {
    await dialogFormRef.value.submit()
  }
}

const success = async result => {
  dialogVisible.value = false
  if (result.action === 'export') {
    downloadFile(result.data.file, result.data.name)
  } else {
    await reload()
  }
}

const print = () => {
  html2canvas(document.getElementById('table')).then(canvas => {
    const img = canvas.toDataURL('image/png', 1)
    printJS(img, 'image')
  })
}

const initColumns = () => {
  const result = [
    {
      key: 'checked',
      dataKey: 'checked',
      width: 44,
      fixed: 'left',
      cellRenderer: prop => {
        const onChange = value => {
          prop.rowData.checked = value
        }
        return (
          <el-checkbox modelValue={prop.rowData.checked} onChange={onChange} />
        )
      },
      headerCellRenderer: () => {
        const rawData = unref(listModel)
        const onChange = value => {
          listModel.value = rawData.map(row => {
            row.checked = value
            return row
          })
        }
        const allSelected = rawData.every(row => row.checked)
        const containsChecked = rawData.some(row => row.checked)
        return (
          <el-checkbox
            modelValue={allSelected}
            indeterminate={containsChecked && !allSelected}
            onChange={onChange}
          />
        )
      },
    },
    {
      key: 'rowNumber',
      dataKey: 'rowNumber',
      title: t('行号'),
      fixed: 'left',
      width: 44,
      hidden: false,
      cellRenderer: prop => {
        return (
          <>
            {(queryModel.value.pageIndex - 1) * queryModel.value.pageSize +
              prop.rowIndex +
              1}
          </>
        )
      },
    },
  ]
  Object.keys(listSchema.value.properties).forEach(key => {
    const property = listSchema.value.properties[key]
    if (property.hidden || property.hideInList) {
      return
    }
    const column = {
      key,
      dataKey: key,
      title: t(property?.title ?? key),
      width: property?.width ?? 120,
      hidden: property.hidden,
      sortable: property?.sortable ?? true,
      draggable: true,
    }
    // if (property.input === 'datetime') {
    //   column.cellRenderer = ({ cellData }) => {
    //     return <span>{dayjs(cellData).format(property.format ?? 'YYYY-MM-DD HH:mm:ss')}</span>;
    //   };
    // }
    // if (property.type === 'boolean') {
    //   column.cellRenderer = ({ cellData }) => {
    //     return (
    //       <span>
    //         <el-checkbox disabled vModel={cellData} />
    //       </span>
    //     );
    //   };
    // }
    column.cellRenderer = data => {
      return (
        <AppFormInput
          key={data.rowData}
          v-model={data.rowData}
          mode="details"
          schema={listSchema.value.properties[key]}
          prop={key}
        />
      )
    }
    result.push(column)
  })
  const rowButtons = buttons.value?.filter(
    o => !o.meta?.hidden && o.meta?.buttonType === 'row',
  )
  const width =
    rowButtons
      .map(o => o.meta?.width)
      .filter(o => o)
      .reduce((a, b) => a + b, null) ?? 130
  if (rowButtons.length) {
    result.push({
      key: 'operations',
      width,
      fixed: 'right',
      title: t('操作'),
      cellRenderer: prop => {
        return (
          <>
            {rowButtons.map(o => {
              return (
                <el-button
                  type="primary"
                  text={true}
                  onClick={() => click(o, [prop.rowData])}
                >
                  {t(o.meta.title)}
                </el-button>
              )
            })}
          </>
        )
      },
    })
  }
  result.forEach(o => {
    o.show = !o.hidden
  })
  return result
}

const getButtonType = action => {
  switch (action) {
    case 'search':
      return 'primary'
    case 'create':
      return 'primary'
    case 'delete':
      return 'danger'
    case 'edit':
      return 'primary'
    case 'import':
      return 'success'
    case 'export':
      return 'success'
    default:
      return 'primary'
  }
}

onMounted(async () => {
  await load()
})

//
routeData.clear()
columns.value = initColumns()
originalColumns.value = [...columns.value]
</script>
