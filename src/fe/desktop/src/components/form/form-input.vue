<template>
  <template v-if="getDisabled()">
    <div v-if="model[prop] !== null" class="el-input">
      <el-switch
        v-if="schema.type === 'boolean'"
        v-model="model[prop]"
        disabled
        type="checked"
      />
      <template v-else-if="schema.input === 'year'">
        {{ dayjs(model[prop]).format('YYYY') }}
      </template>
      <template v-else-if="schema.input === 'date'">
        {{ dayjs(model[prop]).format('YYYY-MM-DD') }}
      </template>
      <template v-else-if="schema.input === 'datetime'">
        {{ dayjs(model[prop]).format('YYYY-MM-DD HH:mm:ss') }}
      </template>
      <template v-else-if="schema.input === 'password'"> ****** </template>
      <template v-else-if="schema.input === 'select'">
        <template v-if="schema.multiple">
          {{
            options.filter(o => model[prop].includes(o.value)).map(o => o.label)
          }}
        </template>
        <template v-else>
          {{ options.find(o => o.value == model[prop])?.label }}
        </template>
      </template>
      <template v-else-if="schema.input === 'cascader'">
        <div>
          <template v-if="schema.multiple">
            <div v-for="item in getCascaderDisplay" :key="item">
              {{ item }}
            </div>
          </template>
          <template v-else>
            {{ getCascaderDisplay }}
          </template>
        </div>
      </template>
      <template v-else-if="schema.input === 'icon'">
        <svg-icon :name="model[prop]" />
      </template>
      <template v-else-if="schema.input === 'upload'">
        <template v-if="schema.multiple">
          <template v-if="schema.isImage">
            <el-image
              v-for="item in model[prop]"
              :key="item"
              :src="item"
              :preview-src-list="[item]"
              :preview-teleported="true"
              style="height: 1em"
              class="mr-2"
            />
          </template>
          <template v-else>
            <el-link v-for="item in model[prop]" :key="item" class="mr-2">
              {{ item }}
            </el-link>
          </template>
        </template>
        <template v-else>
          <el-link v-if="!schema.isImage">
            {{ model[prop] }}
          </el-link>
          <el-image
            v-else
            :src="model[prop]"
            :preview-src-list="[model[prop]]"
            :preview-teleported="true"
            style="height: 1em"
          />
        </template>
      </template>
      <template v-else>
        <pre>{{ model[prop] }}</pre>
      </template>
    </div>
  </template>
  <template v-else>
    <template v-if="getInput(schema) === 'tabs' && mode === 'query'">
      <el-tabs
        v-model="model[prop]"
        type="card"
        style="height: 24px; margin: 0"
        class="form"
      >
        <el-tab-pane key="all" label="全部" :name="''" />
        <el-tab-pane
          v-for="item in options"
          :key="item.value"
          :label="item.label"
          :name="item.value"
        />
      </el-tabs>
    </template>
    <template v-if="getInput(schema) === 'color'">
      <el-color-picker v-model="model[prop]" />
    </template>
    <template v-else-if="getInput(schema) === 'select'">
      <el-select
        v-model="model[prop]"
        :placeholder="placeholder"
        :multiple="!!schema.multiple"
        :clearable="true"
      >
        <template #prefix>
          <svg-icon
            v-if="options?.find(o => o.value == model[prop])?.icon"
            :name="options.find(o => o.value == model[prop])?.icon"
          />
        </template>
        <el-option
          v-for="item in options"
          :key="item.key"
          :label="item.label"
          :value="item.value"
        >
          <span style="display: flex; align-items: center">
            <svg-icon
              v-if="item.icon"
              :name="item.icon"
              class="el-icon--left"
            />
            <span>{{ item.label }}</span>
          </span>
        </el-option>
      </el-select>
    </template>
    <template v-else-if="getInput(schema) === 'cascader'">
      <el-cascader
        v-model="cascaderValues"
        :options="options"
        clearable
        :props="cascaderProps"
        :placeholder="placeholder"
        @change="onCascaderChange"
      />
    </template>
    <template
      v-else-if="
        getInput(schema) === 'month' ||
        getInput(schema) === 'datetime' ||
        getInput(schema) === 'datetimerange'
      "
    >
      <el-date-picker
        v-model="model[prop]"
        :type="schema.input"
        :value-format="schema.format ?? 'YYYY-MM-DD HH:mm:ss'"
        :clearable="!!schema.clearable"
      />
    </template>
    <template v-else-if="getInput(schema) === 'number'">
      <el-input
        v-model="model[prop]"
        :disabled="getDisabled()"
        :placeholder="placeholder"
        type="number"
      />
    </template>
    <template v-else-if="getInput(schema) === 'integer'">
      <el-input-number
        v-model="model[prop]"
        :disabled="getDisabled()"
        :placeholder="placeholder"
        :precision="0"
      />
    </template>
    <template v-else-if="getInput(schema) === 'boolean'">
      <el-select
        v-if="schema.nullable"
        v-model="model[prop]"
        :disabled="getDisabled()"
        :placeholder="placeholder"
      >
        <el-option prop="select" value="" :label="$t('select')" />
        <el-option prop="true" :value="true" :label="$t('true')" />
        <el-option prop="false" :value="false" :label="$t('false')" />
      </el-select>
      <el-checkbox
        v-else
        v-model="model[prop]"
        :label="schema.showLabel ? placeholder : ''"
      />
    </template>
    <template v-else-if="getInput(schema) === 'file'">
      <el-upload
        ref="uploadRef"
        v-model:file-list="model[prop]"
        class="upload"
        drag
        :accept="schema.accept"
        :multiple="schema.multiple"
        :limit="limit"
        :auto-upload="false"
        :on-change="handleChange"
      >
        <template #trigger>
          <el-icon style="font-size: 4em">
            <i class="i-ep-upload-filled" />
          </el-icon>
        </template>
        <template #tip>
          <div class="el-upload__tip">
            <div>
              单个文件大小限制：{{ bytesFormat(size) }}，上传数量限制：{{
                limit
              }}
              <template v-if="schema.accept">
                ，上传文件类型：{{ schema.accept }}
              </template>
            </div>
          </div>
        </template>
      </el-upload>
    </template>
    <template v-else-if="schema.input === 'icon'">
      <icon-select v-model="model[prop]" />
    </template>
    <template v-else-if="schema.input === 'upload'">
      <el-upload
        ref="upload"
        v-model:file-list="fileList"
        :accept="schema.accept"
        :limit="limit"
        :action="getAction()"
        :multiple="!!schema.multiple"
        :before-upload="beforeUpload"
        :on-exceed="onExceed"
        :on-success="onUploadSuccess"
        :list-type="!schema.isImage ? 'text' : 'picture-card'"
        :http-request="uploadFile"
        class="w-full"
      >
        <template #trigger>
          <el-icon>
            <i class="i-ep-plus" />
          </el-icon>
        </template>
        <template #tip>
          <div class="el-upload__tip">
            <div>
              单个文件大小限制：{{ bytesFormat(size) }}，上传数量限制：{{ limit
              }}<template v-if="schema.accept">
                ，上传文件类型：{{ schema.accept }}
              </template>
            </div>
          </div>
        </template>
        <template #file="{ file }">
          <template v-if="!schema.isImage">
            <div class="el-upload-list__item-info">
              <a class="el-upload-list__item-name">
                <el-icon class="el-icon--document"
                  ><i class="i-ep-document"
                /></el-icon>
                <span class="el-upload-list__item-file-name">{{
                  file.url
                }}</span>
              </a>
              <el-icon class="el-icon--close" @click="onRemove(file)">
                <i class="i-ep-close" />
              </el-icon>
            </div>
          </template>
          <div v-else>
            <img
              class="el-upload-list__item-thumbnail"
              :src="file.url"
              alt=""
            />
            <span class="el-upload-list__item-actions">
              <span
                class="el-upload-list__item-preview"
                @click="onPreview(file)"
              >
                <el-icon><i class="i-ep-zoom-in" /></el-icon>
              </span>
              <span class="el-upload-list__item-delete" @click="onRemove(file)">
                <el-icon><i class="i-ep-delete" /></el-icon>
              </span>
            </span>
          </div>
        </template>
      </el-upload>
      <el-dialog v-model="preivewImageVisable" :close-on-click-modal="false">
        <img w-full :src="previewImageUrl" alt="schema.title" />
      </el-dialog>
    </template>
    <template v-else-if="schema.input === 'image-captcha'">
      <image-captcha
        v-model="model[prop]"
        :icon="schema.icon"
        :url="schema.url"
        :code-hash="schema.codeHash"
        @callback="updateCodeHash"
      />
    </template>
    <template v-else-if="schema.input === 'code-captcha'">
      <code-captcha
        v-model="model[prop]"
        :icon="schema.icon"
        :url="schema.url"
        :code-hash="schema.codeHash"
        :query="model[schema.query]"
        :regexp="schema.regexp"
        @callback="updateCodeHash"
      />
    </template>
    <template v-else>
      <el-input
        v-model="model[prop]"
        clearable
        :disabled="getDisabled()"
        :placeholder="placeholder"
        :type="schema.input ?? 'text'"
        :show-password="schema.input === 'password'"
      >
        <template #prefix>
          <el-icon v-if="schema.icon" class="el-input__icon">
            <svg-icon :name="schema.icon" />
          </el-icon>
        </template>
      </el-input>
    </template>
  </template>
</template>

<script setup>
import { dayjs, ElMessageBox, useFormItem } from 'element-plus'
import {
  computed,
  onMounted,
  reactive,
  ref,
  watch,
  watchEffect,
  inject,
} from 'vue'
import { useI18n } from 'vue-i18n'
import { listToTree, findPath } from '@/utils/index.js'
import SvgIcon from '@/components/icon/index.vue'
import IconSelect from '@/components/icon/icon-select.vue'
import { bytesFormat, importFunction } from '@/utils/index.js'
import request, { getUrl } from '@/utils/request.js'
import { useTokenStore } from '@/store/index.js'
import ImageCaptcha from '@/components/form/input/image-captcha.vue'
import CodeCaptcha from '@/components/form/input/code-captcha.vue'
import { orderBy } from 'lodash-es'

const props = defineProps([
  'modelValue',
  'schema',
  'prop',
  'isReadOnly',
  'mode',
])
const emit = defineEmits(['update:modelValue'])

const model = reactive(props.modelValue)
watch(model, value => {
  emit('update:modelValue', value)
})
/* start */
const routeData = inject('routeData')
const tokenStore = useTokenStore()
const { t } = useI18n()
const placeholder = computed(() => {
  return t(props.schema.placeholder ?? props.schema.title ?? props.prop)
})
const getDisabled = () => {
  if (props.mode === 'details') {
    return true
  }
  if (props.mode === 'update' && props.schema.readOnly) {
    return true
  }
  if (props.mode === 'update' && props.schema.readonly) {
    return true
  }
  return false
}
const getInput = schema => {
  return schema.input ?? schema.type
}
/* end */

// options
const options = ref([])
const cascaderProps = ref({
  multiple: !!props.schema.multiple,
  checkStrictly: !!props.schema.checkStrictly,
  emitPath: false,
})
const cascaderValues = ref([])
watchEffect(() => {
  if (props.mode !== 'details') {
    if (props.schema.input === 'select') {
    } else if (props.schema.input === 'cascader') {
      if (model[props.prop]) {
        if (props.schema.multiple) {
          cascaderValues.value = props.modelValue[props.prop]
        } else {
          cascaderValues.value = props.modelValue[props.prop]
            ? findPath(options.value, n => n.value === model[props.prop]).map(
                o => o.value,
              )
            : []
        }
      } else {
        cascaderValues.value = []
      }
    }
  }
})

const onCascaderChange = values => {
  model[props.prop] = values
  console.log(cascaderValues.value)
}

const getCascaderDisplay = computed(() => {
  if (props.schema.multiple) {
    return model[props.prop]
      .map(o =>
        findPath(options.value, n => n.value === o)
          .map(i => i.label)
          .join(' / '),
      )
      .sort()
  }
  return findPath(options.value, n => n.value === model[props.prop])
    .map(o => o.label)
    .join(' / ')
})

// import files
const { formItem } = useFormItem()
const handleChange = async (uploadFile, uploadFiles) => {
  const ext = uploadFile.name.substr(uploadFile.name.lastIndexOf('.'))
  const index = uploadFiles.findIndex(o => o.uid !== uploadFile.uid)
  if (props.schema.accept && !fileTypes.some(o => o === ext)) {
    ElMessageBox.alert(
      `当前文件 ${uploadFile.name} 不是可选文件类型 ${props.schema.accept}`,
      '提示',
    )
    uploadFiles.splice(index, 1)
    return false
  }
  if (uploadFile.size > size.value) {
    ElMessageBox.alert(
      `当前文件大小 ${bytesFormat(uploadFile.size)} 已超过 ${bytesFormat(size)}`,
      '提示',
    )
    uploadFiles.splice(index, 1)
    return false
  }
  if (uploadFiles.length) {
    model[props.prop] = props.schema.multiple ? uploadFiles : uploadFiles[0]
  } else {
    model[props.prop] = props.schema.multiple ? [] : null
  }
  try {
    await formItem.validate()
  } catch (error) {
    console.log(error)
  }
}

//upload
const upload = ref(null)
const preivewImageVisable = ref(false)
const previewImageUrl = ref(null)
const limit = computed(() =>
  props.schema.multiple ? props.schema.limit ?? 5 : 1,
)
const size = computed(() => props.schema.size ?? 1024 * 1024)
const fileTypes = props.schema.accept?.split(',') ?? []
const fileList = ref([])
watchEffect(() => {
  if (props.schema.input === 'upload') {
    if (model[props.prop]) {
      if (props.schema.multiple) {
        fileList.value = model[props.prop].map((o, i) => ({
          name: `${props.prop}_${i}`,
          url: o,
        }))
      } else {
        fileList.value = [{ name: props.prop, url: model[props.prop] }]
      }
    } else {
      fileList.value = []
    }
  }
})

const onUploadSuccess = (uploadFile, uploadFiles) => {
  fileList.value.find(o => o.name === uploadFiles.name).url = uploadFile.data
  if (props.schema.multiple) {
    model[props.prop] = fileList.value.map(o => o.url)
  } else {
    model[props.prop] = uploadFile.data
  }
}

const onRemove = file => {
  upload.value.handleRemove(file)
  if (props.schema.multiple) {
    model[props.prop] = fileList.value
      .filter(o => o.name !== file.name)
      .map(o => o.url)
  } else {
    model[props.prop] = null
  }
}

const beforeUpload = async file => {
  const ext = file.name.substr(file.name.lastIndexOf('.'))
  if (props.schema.accept && !fileTypes.some(o => o === ext)) {
    ElMessageBox.alert(
      `当前文件 ${file.name} 不是可选文件类型 ${props.schema.accept}`,
      '提示',
    )
    return false
  }
  if (file.size > size.value) {
    ElMessageBox.alert(
      `当前文件大小 ${bytesFormat(file.size)}，已超过 ${bytesFormat(size.value)}`,
      '提示',
    )
    return false
  }
  return true
}

const uploadFile = async option => {
  const formData = new FormData()
  if (option.data) {
    for (const [key, value] of Object.entries(option.data)) {
      if (isArray(value) && value.length) formData.append(key, ...value)
      else formData.append(key, value)
    }
  }
  formData.append(option.filename, option.file, option.file.name)
  const result = await request(option.method, option.action, formData)
  if (result.ok) {
    option.onSuccess(result)
  }
}

const onPreview = file => {
  previewImageUrl.value = file.url
  preivewImageVisable.value = true
}

const onExceed = (files, uploadFiles) => {
  ElMessage.warning(
    `上传最大数量为 ${limit.value}, 本次选择了 ${files.length} 个文件, 总计 ${
      files.length + uploadFiles.length
    } 个文件`,
  )
}

const getAction = () => {
  return getUrl(props.schema.url)
}
const updateCodeHash = data => {
  model[props.schema.codeHash ?? 'codeHash'] = data
}
//mounted
onMounted(async () => {
  if (props.schema.input === 'select' || props.schema.input === 'cascader') {
    //选择
    if (props.schema.url && !props.schema.options) {
      let list = routeData.get(props.prop)
      if (!list) {
        try {
          const method = props.schema.method ?? 'POST'
          const url = `${props.schema.url}`
          const data = { includeAll: true }
          const result = await request(method, url, data)
          if (props.schema.input === 'cascader') {
            list = listToTree(
              result.data.items.map(o => ({
                id: o[props.schema.id ?? 'id'],
                parentId: o[props.schema.parentId ?? 'parentId'],
                value: o[props.schema.value ?? 'id'],
                label: t(o[props.schema.label ?? 'name']),
                disabled: !!o[props.schema.disabled ?? 'disabled'],
              })),
            )
          } else if (props.schema.input === 'select') {
            list = result.data.items.map(o => ({
              value: o[props.schema.value ?? 'id'],
              label: t(o[props.schema.label ?? 'name']),
              disabled: !!o[props.schema.disabled ?? 'disabled'],
            }))
          }
          routeData.set(props.prop, list)
        } catch (error) {
          console.log(error)
        }
      }
      options.value = list
    }
    if (props.schema.options) {
      options.value = props.schema.options
    }
  }
})
</script>
