<template>
  <div v-loading="tableLoading" class="c-list" style="height: 100%">
    <div style="height: calc(100% - 52px)">
      <el-scrollbar ref="listScrollbarRef" :always="true" style="height: 100%">
        <el-row style="padding-bottom: 20px">
          <el-col>
            <app-form
              v-model="queryModel"
              inline
              mode="query"
              label-position="left"
              :schema="config.query.schema"
              :hide-button="true"
              :is-query-form="true"
              @submit="load"
            >
              <template
                v-for="item in filterList.filter(o => !o.hidden && o.readOnly)"
              >
                <template
                  v-if="config.edit?.schema?.properties[item.column]?.title"
                >
                  <el-form-item
                    :label="
                      item.title ??
                      config.edit?.schema?.properties[item.column].title
                    "
                  >
                    <app-form-input
                      v-model="item"
                      :schema="config.edit?.schema?.properties[item.column]"
                      prop="value"
                      mode="query"
                    />
                  </el-form-item>
                </template>
                <div v-else>
                  {{ item.column }}
                </div>
              </template>
            </app-form>
            <template v-for="item in buttons">
              <el-button
                v-if="item.meta.isTop"
                v-show="
                  !item.meta.show || item.meta.show(selectedRows, queryModel)
                "
                :class="item.meta.htmlClass ?? 'el-button--primary'"
                :disabled="
                  item.meta.disabled &&
                  item.meta.disabled.constructor === Function &&
                  item.meta.disabled(selectedRows, queryModel)
                "
                @click="click(item, selectedRows)"
              >
                <el-icon v-if="item.meta.icon">
                  <svg-icon :name="item.meta.icon" />
                </el-icon>
                <span>{{ item.meta.title }}</span>
              </el-button>
            </template>
            <el-button
              v-if="config.query.hasFilter"
              @click="click('过滤', selectedRows)"
            >
              <el-icon><ep-filter /></el-icon>
              <span>{{ $t('筛选') }}</span>
            </el-button>
            <slot name="tableButtons" :rows="selectedRows" />
          </el-col>
        </el-row>
        <el-table
          :key="tableKey"
          ref="tableRef"
          :tree-props="treeProps"
          :data="tableData"
          :header-cell-class-name="getClass"
          row-key="id"
          table-layout="auto"
          border
          fit
          style="width: calc(100% - 26px)"
          @selection-change="handleSelectionChange"
          @sort-change="sortChange"
        >
          <el-table-column
            v-if="!config.table.schema.disableSelection"
            fixed="left"
            type="selection"
            :selectable="config.table.selectable"
          />
          <el-table-column type="index" :label="$t('rowIndex')">
            <template #default="scope">
              {{
                (pageModel.pageIndex - 1) * pageModel.pageSize +
                scope.$index +
                1
              }}
            </template>
          </el-table-column>
          <template v-for="(item, key) in config.table.schema.properties">
            <template v-if="item.navigation">
              <el-table-column :prop="key" :label="item.title">
                <template #default="scope">
                  {{ getProp(scope.row, item.navigation) }}
                </template>
              </el-table-column>
            </template>
            <template v-else-if="item.oneToMany">
              <el-table-column :prop="key" :label="item.title">
                <template #default="scope">
                  <el-link
                    type="primary"
                    @click="
                      showList(
                        { [key]: scope.row[key] },
                        item.oneToMany,
                        item.config,
                      )
                    "
                  >
                    <app-form-input
                      v-model="scope.row"
                      mode="details"
                      :schema="item"
                      :prop="key"
                    />
                  </el-link>
                </template>
              </el-table-column>
            </template>
            <template v-else-if="item.link">
              <el-table-column :prop="key" :label="item.title">
                <template #default="scope">
                  <el-link
                    type="primary"
                    @click="click({ path: key }, [scope.row])"
                  >
                    {{ scope.row[key] }}
                  </el-link>
                </template>
              </el-table-column>
            </template>
            <template v-else-if="item.type !== 'object'">
              <template v-if="!item.hideForList && showColumn(item, key)">
                <el-table-column
                  :prop="key"
                  sortable="custom"
                  :sort-orders="['descending', 'ascending', null]"
                >
                  <template #header="scope">
                    {{ item.title }}
                  </template>
                  <template #default="scope">
                    <app-form-input
                      v-model="scope.row"
                      mode="details"
                      :schema="item"
                      :prop="key"
                    />
                  </template>
                </el-table-column>
              </template>
            </template>
            <template v-if="item.type === 'object'">
              <template v-for="(item2, key2) in item['properties']">
                <el-table-column :prop="key + '.' + key2">
                  <template #header="scope">
                    {{ item2.title }}
                  </template>
                  <template #default="scope">
                    <template v-if="scope.row[key]">
                      <app-form-input
                        v-model="scope.row[key]"
                        mode="details"
                        :schema="item2"
                        :prop="key2"
                      />
                    </template>
                  </template>
                </el-table-column>
              </template>
            </template>
          </template>
          <slot name="columns" />
          <el-table-column fixed="right">
            <template #header>
              <el-button @click="filterDrawer = true">
                {{ $t('operations') }}
                <el-icon class="el-icon--right">
                  <ep-filter />
                </el-icon>
              </el-button>
            </template>
            <template #default="scope">
              <div class="flex">
                <template v-for="item in buttons">
                  <el-button
                    v-if="!item.meta.isTop"
                    v-show="
                      !item.meta.show || item.meta.show(scope.row, queryModel)
                    "
                    :class="item.meta.htmlClass ?? 'el-button--primary'"
                    :disabled="
                      item.meta.disabled && item.meta.disabled(scope.row)
                    "
                    @click="click(item, [scope.row])"
                  >
                    <el-icon v-if="item.meta.icon">
                      <svg-icon :name="item.meta.icon" />
                    </el-icon>
                    <span>{{ item.meta.title }}</span>
                  </el-button>
                </template>
                <slot name="rowButtons" :rows="[scope.row]" />
              </div>
            </template>
          </el-table-column>
        </el-table>
      </el-scrollbar>
    </div>
    <div style="height: 52px; padding-top: 20px">
      <el-scrollbar>
        <el-pagination
          v-model:currentPage="pageModel.pageIndex"
          v-model:page-size="pageModel.pageSize"
          small
          :total="pageModel.total"
          :page-sizes="pageModel.sizeList"
          :background="true"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="onPageSizeChange"
          @current-change="onPageIndexChange"
        />
      </el-scrollbar>
    </div>
  </div>
  <el-drawer
    v-model="filterDrawer"
    :close-on-click-modal="false"
    destroy-on-close
    @close="tableRef.doLayout()"
  >
    <template #header>
      <span class="el-dialog__title"> {{ $t('过滤') }} </span>
    </template>
    <el-scrollbar>
      <el-row>
        <el-col style="max-height: calc(100% - 180px)">
          <el-form inline>
            <el-form-item>
              <el-button
                type="primary"
                @click="columns.forEach(o => (o.checked = true))"
              >
                {{ $t('全选') }}
              </el-button>
            </el-form-item>
            <el-form-item>
              <el-button
                type="primary"
                @click="columns.forEach(o => (o.checked = !o.checked))"
              >
                {{ $t('反选') }}
              </el-button>
            </el-form-item>
            <el-form-item v-for="item in columns">
              <el-checkbox
                v-model="item.checked"
                :label="item.title"
                size="large"
              />
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
    </el-scrollbar>
    <template #footer>
      <span class="dialog-footer">
        <el-button type="primary" @click="filterDrawer = false">
          {{ $t('confirm') }}
        </el-button>
      </span>
    </template>
  </el-drawer>
  <el-drawer
    v-model="subDrawer"
    :close-on-click-modal="false"
    destroy-on-close
    size="50%"
  >
    <el-scrollbar>
      <app-list
        v-if="subDrawer"
        :query="subListQuery"
        :buttons="subListQuery.buttons"
        :config="subListQuery.config"
      />
    </el-scrollbar>
    <template #footer>
      <span class="dialog-footer">
        <el-button type="primary" @click="subDrawer = false">
          {{ $t('confirm') }}
        </el-button>
      </span>
    </template>
  </el-drawer>
  <el-dialog
    v-model="dialogVisible"
    align-center
    destroy-on-close
    :close-on-click-modal="false"
    style="width: auto; width: 700px"
  >
    <template #header>
      <span class="el-dialog__title"> {{ editFormTitle }} </span>
    </template>
    <el-row v-loading="editFormloading">
      <el-col style="max-height: calc(100% - 180px); min-height: 100%">
        <el-scrollbar>
          <template
            v-if="
              editFormMode === 'create' ||
              editFormMode === 'update' ||
              editFormMode === 'details'
            "
          >
            <app-form
              ref="editFormRef"
              v-model="editFormModel"
              :disabled="editFormMode === 'details'"
              :mode="editFormMode"
              inline
              label-position="left"
              :hide-button="true"
              :schema="config.edit?.schema"
              style="height: 100%"
            />
          </template>
          <template v-else-if="editFormMode === 'import'">
            <app-form
              ref="importFormRef"
              v-model="importModel"
              mode="update"
              label-position="left"
              :schema="config.import?.schema"
              :hide-button="true"
              :is-query-form="true"
              style="height: 100%"
            />
          </template>
          <template v-else-if="editFormMode === 'filter'">
            <el-form :model="filterList" inline class="filter">
              <el-row
                v-for="(item, index) in filterList.filter(o => !o.hidden)"
              >
                <el-col :span="6">
                  <el-select
                    v-model="item.column"
                    clearable
                    :disabled="item.readOnly"
                    :placeholder="$t('字段')"
                  >
                    <template
                      v-for="(value, prop) in config.edit?.schema?.properties"
                    >
                      <el-option
                        v-if="value.type !== 'object' && value.type !== 'array'"
                        :value="prop"
                        :label="value.title"
                      />
                    </template>
                  </el-select>
                </el-col>
                <el-col v-if="item.column" :span="6">
                  <el-select
                    v-model="item.action"
                    clearable
                    :disabled="item.readOnly"
                    :placeholder="$t('操作符')"
                  >
                    <el-option
                      v-for="item in getOperators(
                        config.edit?.schema?.properties[item.column],
                      )"
                      :value="item.value"
                      :label="item.label"
                    />
                  </el-select>
                </el-col>
                <el-col v-if="item.column" :span="8">
                  <app-form-input
                    v-model="item"
                    :schema="config.edit?.schema?.properties[item.column]"
                    prop="value"
                  />
                </el-col>
                <el-col v-if="!item.readOnly && item.action" :span="2">
                  <el-button circle @click="filterList.splice(index, 1)">
                    <template #icon>
                      <ep-close />
                    </template>
                  </el-button>
                </el-col>
              </el-row>
              <el-row>
                <el-col>
                  <el-button circle @click="pushfilterList">
                    <template #icon>
                      <ep-plus />
                    </template>
                  </el-button>
                </el-col>
              </el-row>
            </el-form>
          </template>
          <template v-else>
            <slot :name="editFormMode" />
          </template>
        </el-scrollbar>
      </el-col>
    </el-row>
    <template #footer>
      <span class="dialog-footer">
        <el-button type="primary" @click="submit">
          {{ $t('confirm') }}
        </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { ElMessage, ElMessageBox } from 'element-plus'
import { camelCase, capitalize } from 'lodash'
import { nextTick, onMounted, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'

import AppFormInput from '@/components/form/form-input.vue'
import AppForm from '@/components/form/index.vue'
import SvgIcon from '@/components/icon/index.vue'
import {
  format,
  getProp,
  importFunction,
  listToTree,
  schemaToModel,
} from '@/utils/index.js'
import request, { getUrl } from '@/utils/request.js'

const props = defineProps([
  'modelValue',
  'config',
  'querySchema',
  'query',
  'buttons',
])
const emit = defineEmits(['command'])

const listScrollbarRef = ref(null)
/* 变量定义 */
// 配置
const config = reactive(props.config)
// 分页
const pageModel = reactive({
  sizeList: [10, 100, 1000, 10000],
  pageIndex: 1,
  pageSize: 10,
  total: 0,
})
const treeProps = reactive({
  children: 'children',
})
const tableKey = ref(false)
const tableRef = ref(null)
const uploadRef = ref(null)
const columns = ref([])
const filterDrawer = ref(false)
const subDrawer = ref(false)
const subListQuery = ref(props.query ?? {})
const tableLoading = ref(false)
const selectedRows = ref([])
const dialogVisible = ref(false)
const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const sortColumns = ref(new Map())
const filterList = ref([])
const tableSchema = ref({})
const tableData = ref([])
const editFormRef = ref(null)
const importFormRef = ref(null)
const editFormloading = ref(false)
const editFormMode = ref(null)
const editFormTitle = ref('')
const editFormModel = ref(null)
// 注释一下代码暂停权限验证
// const buttons = ref(props.buttons ?? route.meta.children.filter((o) => o.meta.hasPermission));
// 添加下行代码暂停权限验证
const buttons = ref(props.buttons ?? route.meta.children)
const queryModel = ref(schemaToModel(config.query.schema))
function buildQuery() {
  queryModel.value.maxResultCount = pageModel.pageSize
  queryModel.value.skipCount = (pageModel.pageIndex - 1) * pageModel.pageSize
  //
  const postData = JSON.parse(JSON.stringify(queryModel.value))
  // Object.assign(postData, subListQuery.value.query);//注释掉子表DTO查询
  postData.filters = filterList.value.filter(
    o => o.column && o.action && (o.value || o.value === false),
  )
  if (subListQuery.value.query) {
    Object.keys(subListQuery.value.query).forEach(o => {
      postData.filters.push({
        logic: 'and',
        column: o,
        action: 'equal',
        value: subListQuery.value.query[o],
      })
    })
  }
  // 添加子表filter查询
  if (
    route.meta.businessType &&
    route.meta.path !== '/jis-bbac/settlement/bbac_can_sa_service'
  ) {
    postData.filters.push({
      logic: 'and',
      column: 'businessType',
      action: 'equal',
      value: route.meta.businessType,
    })
  }
  if (postData.items) {
    delete postData.items
  }
  if (postData.query?.id) {
    delete postData.query.id
  }
  return postData
}
const load = async () => {
  tableLoading.value = true
  try {
    const { url } = config.query
    const { method } = config.query
    const postData = buildQuery()
    const listData = (await request(method, url, postData)).data
    let { items } = listData
    if (tableSchema.value.isTree) {
      items = listToTree(listData.items)
    }
    tableData.value = items
    pageModel.total = listData.totalCount
    // data.value = listData;
    tableKey.value = !tableKey.value
    nextTick(() => {
      tableRef.value.doLayout()
      nextTick(() => listScrollbarRef.value.update())
    })
  } catch (error) {
    console.log(error)
  } finally {
    tableLoading.value = false
  }
}
const reload = async () => {
  pageModel.pageIndex = 1
  await load()
}
const onPageIndexChange = async () => {
  await load()
}
const onPageSizeChange = async () => {
  await reload()
}
watch(queryModel.value, async (value, oldValue, a) => {
  if (config.query.autoSubmit) {
    await load()
  }
})
//
config.import ??= { schema: { type: 'object', properties: {} } }
config.import.schema.properties.files ??= {
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
}
const defaultImportModel = schemaToModel(config.import.schema)
const importModel = ref(null)
const onClick = async (
  method,
  confirMmessage = '确认操作吗？',
  reload = true,
) => {
  try {
    if (confirMmessage) {
      await ElMessageBox.confirm(confirMmessage, '提示', {
        type: 'warning',
      })
    }
    tableLoading.value = true
    let result = null
    if (method.constructor.name === 'AsyncFunction') {
      result = await method()
    } else {
      result = method()
    }
    if (!result.errors && reload) {
      pageModel.pageIndex = 1
      await load()
    }
  } catch (error) {
    if (error === 'cancel') {
      ElMessage({
        type: 'info',
        message: '操作取消',
      })
    }
  } finally {
    tableLoading.value = false
  }
}
const getSortModel = model => {
  ;(model.sorting ?? '')
    .split(',')
    .map(o => o.trim())
    .filter(o => o)
    .map(o => ({
      prop: camelCase(o.split(' ')[0]),
      order: `${o.split(' ').filter(p => p)[1] ?? 'asc'}ending`,
    }))
    .forEach(o => sortColumns.value.set(o.prop, o.order))
}
const getColumns = schema => {
  Object.keys(schema.properties).forEach(propertyName => {
    const property = schema.properties[propertyName]
    if (
      !property.hideForList ||
      (property.type !== 'object' &&
        property.type !== 'array' &&
        !property.hidden)
    ) {
      columns.value.push({
        name: propertyName,
        title: property.title,
        checked: true,
      })
    }
  })
}
const getClass = ({ row, column }) => {
  if (column.property) {
    column.order = sortColumns.value.get(column.property)
  }
}
const sortChange = async ({ column, prop, order }) => {
  if (order === null) {
    sortColumns.value.delete(prop)
  } else {
    sortColumns.value.set(prop, order)
  }
  queryModel.value.sorting = Array.from(sortColumns.value)
    .map(o => capitalize(o[0]) + (o[1] === 'ascending' ? '' : ` DESC`))
    .join(',')
  await load()
}
const showColumn = (item, prop) => {
  return columns.value.some(o => o.name === prop && o.checked)
}
const getFilters = (item, prop) => {
  if (item.input === 'select' && item.options) {
    return item.options.map(o => ({ text: o.label, value: o.value }))
  }
  return null
}
const filterHandler = (value, row, column) => {
  return row[column.property] === value
}
const handleSelectionChange = rows => {
  selectedRows.value = rows
}
const showList = async (value, nav, config) => {
  if (!subDrawer.value) {
    const targetRoute = router.getRoutes().find(o => o.path === nav)
    if (config.constructor === String) {
      const value = (await import(config)).default
      config =
        value.constructor === Function ? value(route.meta.businessType) : value
    }
    subListQuery.value = {
      query: value,
      buttons: targetRoute.meta.children,
      config,
    }
    subDrawer.value = true
  }
}
const click = async (item, rows) => {
  editFormloading.value = true
  editFormMode.value = item.path ?? item
  if (item.path === 'query') {
    // list
    await load()
  } else if (item.path === 'create' || item.path === 'update') {
    // create
    if (item.path === 'create') {
      editFormModel.value = schemaToModel(config.edit?.schema)
    } else {
      const url = format(config.edit?.detailsUrl, rows[0].id)
      editFormModel.value = (
        await request(config.edit?.detailsMethod ?? 'POST', url)
      ).data
      editFormModel.value.id = rows[0].id
    }
    editFormTitle.value = `${t(item.path)}${config.edit?.schema?.title}`
    dialogVisible.value = true
  } else if (item.path === 'delete') {
    if (!rows.length) {
      return
    }
    // delete
    const url = format(config.edit?.deleteUrl, rows[0].id)
    if (item.meta.isTop) {
      // 批量删除
      try {
        await ElMessageBox.confirm(
          format('确认删除选中的%s行数据吗？', rows.length),
          '提示',
          {
            type: 'warning',
          },
        )
        tableLoading.value = true
        const result = await request(
          config.edit?.deleteMethod ?? 'POST',
          url,
          rows.map(o => o.id),
        )
        if (!result.errors) {
          pageModel.pageIndex = 1
          await reload()
        }
      } catch (error) {
        if (error === 'cancel') {
          ElMessage({
            type: 'info',
            message: '操作取消',
          })
        }
      } finally {
        tableLoading.value = false
      }
    } else {
      // 单个删除
      try {
        await ElMessageBox.confirm(
          format('确认删除当前行数据吗？', rows[0]),
          '提示',
          {
            type: 'warning',
          },
        )
        await request(config.edit?.deleteMethod ?? 'POST', url)
        await reload()
      } catch (error) {
        if (error === 'cancel') {
          ElMessage({
            type: 'info',
            message: '操作取消',
          })
        }
      }
    }
    await load()
  } else if (item.path === 'export') {
    if (item.meta.pattern === 'paged') {
      const url = config.edit?.exportUrl
      const method = config.edit?.exportMethod
      const postData = buildQuery()
      await onClick(async () => {
        const response = await request(method, url, postData)
        if (!response.errors) {
          window.open(
            getUrl(`settleaccount/getblobfile/download/${response.data}`),
          )
        }
      }, '确认导出?')
    } else if (item.meta?.pattern === 'file') {
      window.open(
        getUrl(`settleaccount/getblobfile/download/${rows[0].downFileName}`),
      )
    } else if (item.meta?.pattern === 'row') {
      const url = config.edit?.exportUrl
      const method = config.edit?.exportMethod ?? 'POST'
      const postData = {
        [item.meta.key]: rows[0][item.meta.key],
      }
      const response = await request(method, url, postData)
      if (!response.errors) {
        window.open(
          getUrl(`settleaccount/getblobfile/download/${response.data}`),
        )
      }
    } else {
      console.log(item)
    }
  } else if (item.path === 'import') {
    // import
    try {
      importModel.value = { ...defaultImportModel }
      editFormloading.value = true
      editFormTitle.value = `${t(item.path)}${config.query.schema.title}`
      dialogVisible.value = true
    } catch (e) {
      console.log(e)
    } finally {
      editFormloading.value = false
    }
  } else if (item === 'filter') {
    editFormTitle.value = t('自定义查询')
    dialogVisible.value = true
  } else {
    emit('command', item, rows, load, showList)
  }
  editFormloading.value = false
}
const submit = async () => {
  if (editFormMode.value === 'create' || editFormMode.value === 'update') {
    try {
      const valid = await editFormRef.value.validate()
      if (valid) {
        await onClick(
          async () => {
            let url =
              (editFormMode.value === 'create'
                ? config.edit?.createUrl
                : config.edit?.updateUrl) ?? config.query.url
            if (editFormMode.value === 'update') {
              url = format(url, editFormModel.value.id)
            }
            const method =
              editFormMode.value === 'create'
                ? config.edit?.createMethod
                : config.edit?.updateMethod
            const result = await request(method, url, editFormModel.value)
            if (!result.errors) {
              dialogVisible.value = false
              editFormMode.value = null
              await reload()
            }
          },
          null,
          true,
        )
      }
    } catch (error) {
      console.log(error)
    } finally {
      editFormloading.value = false
    }
  } else if (editFormMode.value === 'details') {
    dialogVisible.value = false
    editFormMode.value = null
  } else if (editFormMode.value === 'import') {
    try {
      const valid = await importFormRef.value.validate()
      if (valid) {
        editFormloading.value = true
        const url = config.edit?.importUrl
        const formData = new FormData()
        //
        if (route.meta.businessType) {
          formData.append('businessType', route.meta.businessType)
        }
        Object.keys(importModel.value).forEach(propertyName => {
          if (importModel.value[propertyName]) {
            const schema = config.import.schema.properties[propertyName]
            const value = importModel.value[propertyName]
            if (schema?.type === 'array') {
              importModel.value[propertyName].forEach(item => {
                formData.append(
                  propertyName,
                  schema.input === 'file' ? item.raw : item,
                )
              })
            } else {
              formData.append(
                propertyName,
                schema.input === 'file' ? value.raw : value,
              )
            }
          }
        })
        const result = await request('POST', url, formData)
        if (!result.errors) {
          editFormloading.value = false
          dialogVisible.value = false
          await load()
        } else if (result.data?.code === 400 && result.data.fileName) {
          window.open(
            getUrl(
              `settleaccount/getblobfile/download/${result.data.fileName}`,
            ),
          )
        }
      }
    } catch (error) {
      console.log(error)
    } finally {
      editFormloading.value = false
    }
  } else if (editFormMode.value === 'filter') {
    await load()
    dialogVisible.value = false
  }
}

const download = (url, filename) => {
  const downloadUrl = window.URL.createObjectURL(url)
  const link = document.createElement('a')
  link.href = downloadUrl
  link.download = filename
  link.click()
  window.URL.revokeObjectURL(downloadUrl)
}
const getButtonDisabled = async (src, row) => {
  if (src) {
    const method = await importFunction(src)
    return src.startsWith('async') ? await method(row) : method(row)
  }
  return false
}
const pushfilterList = () => {
  filterList.value.push({
    logic: 'and',
    column: '',
    action: 'equal',
    value: null,
  })
}
const operators = [
  {
    value: 'equal',
    label: '等于',
  },
  {
    value: 'notEqual',
    label: '不等于',
  },
  {
    value: 'biggerThan',
    label: '大于',
  },
  {
    value: 'smallThan',
    label: '小于',
  },
  {
    value: 'biggerThanOrEqual',
    label: '大于等于',
  },
  {
    value: 'smallThanOrEqual',
    label: '小于等于',
  },
  {
    value: 'like',
    label: '类似于',
  },
  {
    value: 'notLike',
    label: '不类似于',
  },
  {
    value: 'in',
    label: '包含于',
  },
  {
    value: 'notIn',
    label: '不包含于',
  },
]
const getOperators = schema => {
  const values = ['equal', 'notEqual']
  if (schema.type === 'string') {
    values.push('like', 'notLike')
    if (
      schema.input &&
      ['year', 'month', 'date', 'datetime'].includes(schema.input)
    ) {
      values.push(
        'biggerThan',
        'smallThan',
        'biggerThanOrEqual',
        'smallThanOrEqual',
      )
    }
  } else if (schema.type === 'boolean') {
  } else {
    values.push(
      'biggerThan',
      'smallThan',
      'biggerThanOrEqual',
      'smallThanOrEqual',
    )
  }
  return operators.filter(o => values.includes(o.value))
}
onMounted(async () => {
  if (route.meta.children?.length) {
    for (const item of route.meta.children) {
      if (item.meta.disabled?.constructor === String) {
        item.meta.disabled = await importFunction(item.meta.disabled)
      }
    }
  }
  //
  getSortModel(queryModel.value)
  filterList.value = queryModel.value?.filters ?? []
  filterList.value.forEach(o => {
    if (o.default) {
      o.value = o.default.constructor === Function ? o.default() : o.default
    }
  })
  getColumns(config.table.schema)
  // if (props.query) {
  //   Object.assign(queryModel.value.query, props.query);
  // }
  if (!config.query.disableQueryOnLoad) {
    await load()
  }
})
</script>

<style>
.el-form.filter .el-col {
  padding: 5px;
}

dl.upload {
  min-height: 100%;
}

dl.upload dt {
  font-weight: bold;
  line-height: 3em;
}

dl.upload dd {
  line-height: 2em;
}

dl.upload .el-form-item {
  width: 300px;
}

div.upload {
  width: 100%;
}
</style>
