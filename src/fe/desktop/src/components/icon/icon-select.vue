<template>
  <el-select v-model="model" filterable clearable @change="onChange">
    <template v-if="model" #prefix>
      <svg-icon :name="model" />
    </template>
    <el-option
      v-for="(item, index) in icons"
      :key="item"
      :label="item"
      :value="item"
    >
      <div class="inline-flex items-center">
        <svg-icon :name="icons[index]" />
        <span class="el-input__inner" style="padding-left: 10px">
          {{ item }}
        </span>
      </div>
    </el-option>
  </el-select>
</template>

<script setup>
import { ref } from 'vue'
import SvgIcon from '@/components/icon/index.vue'

const svgs = import.meta.glob('../../assets/icons/**/*.svg')
const icons = Object.keys(svgs).map(o =>
  o.substring(o.lastIndexOf('/') + 1, o.lastIndexOf('.')),
)
const props = defineProps({
  modelValue: {
    type: String,
    default: null,
  },
})
const model = ref(props.modelValue)
const emit = defineEmits(['update:modelValue'])
const onChange = value => {
  model.value = value
  emit('update:modelValue', value)
}
</script>
