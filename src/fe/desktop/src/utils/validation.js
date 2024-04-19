import Schema from 'async-validator'

import i18n from '@/locale/index.js'

import { format } from './index.js'
import request from './request.js'

const messages = {
  default: '%s验证失败',
  required: '%s是必填项',
  enum: '%s必须是%s之一',
  whitespace: '%s不能为空',
  // date: {
  //   format: '%s date %s is invalid for format %s',
  //   parse: '%s date could not be parsed, %s is invalid ',
  //   invalid: '%s date %s is invalid',
  // },
  types: {
    string: '%s不是有效的字符串',
    method: '%s不是有效的函数',
    array: '%s不是有效的数组',
    object: '%s不是有效的对象',
    number: '%s不是有效的数字',
    date: '%s不是有效的日期',
    boolean: '%s不是有效的布尔值',
    integer: '%s不是有效的整数',
    float: '%s不是有效的浮点数',
    regexp: '%s不是有效的正则表达式',
    email: '%s不是有效的邮箱',
    url: '%s不是有效的 url',
    hex: '%s不是有效的十六进制',
  },
  string: {
    len: '%s长度必须是%s',
    min: '%s最小长度为%s',
    max: '%s最大长度为%s',
    range: '%s长度必须在%s和%s之间',
  },
  number: {
    len: '%s必须等于%s',
    min: '%s不小于%s',
    max: '%s不大于%s',
    range: '%s必须在%s和%s之间',
  },
  array: {
    len: '%s的数量必须是%s',
    min: '%s的数量不小于%s',
    max: '%s的数量不大于%s',
    range: '%s的数量必须在%s和%s之间',
  },
  pattern: {
    mismatch: '%s的值必须是正确的格式',
  },
  clone: function clone() {
    const cloned = JSON.parse(JSON.stringify(this))
    cloned.clone = this.clone
    return cloned
  },
  //
  compare: '%s 和 %s 输入必须一致',
  true: '%s必须选中',
  remote: '%s远程验证失败',
}

const validators = {
  compare(rule, value, callback, source, options) {
    const errors = []
    if (value !== rule.data[rule.compare]) {
      const message = i18n.global.t('compareAttribute', [
        i18n.global.t(rule.title ?? rule.field),
        i18n.global.t(
          rule.schema.properties[rule.compare].title ?? rule.compare,
        ),
      ])
      errors.push(new Error(message))
    }
    callback(errors)
  },
  true(rule, value, callback, source, options) {
    const errors = []
    if (!value) {
      const message = format(options.messages.true, rule.title)
      errors.push(new Error(message))
    }
    callback(errors)
  },
  remote(rule, value, callback, source, options) {
    const errors = []
    if (!value) {
      callback(errors)
    } else {
      const { url } = rule
      const method = rule.method ?? 'POST'
      const data = { [rule.field]: value }
      request(method, url, data, null, true)
        .then(result => {
          if (!result.ok || !result.data) {
            const message = i18n.global.t(
              result.message ?? rule.message,
              i18n.global.t(rule.title ?? rule.field),
            )
            errors.push(new Error(message))
          }
          callback(errors)
        })
        .catch(o => {
          errors.push(o)
          callback(errors)
        })
    }
  },
}

//
const getMessage = (key, ...args) => {
  //const current = i18n.global.locale.value;
  const template = i18n.global.t(key)
  const args2 = args.map(o => i18n.global.t(o))
  return format(template, ...args2)
}
//
const getRules = (parentSchema, prop, data) => {
  const property = parentSchema.properties[prop]
  if (!property.rules) {
    return null
  }
  const rules = [
    ...(Array.isArray(property.rules) ? property.rules : [property.rules]),
  ].map(o => ({ ...o }))
  Object.values(rules).forEach(rule => {
    rule.data = data
    rule.schema = parentSchema
    rule.title ??= property.title
    if (!rule.type && property.type !== 'object') {
      rule.type = property.type
    }
    if (rule.validator) {
      if (rule.validator.constructor === String) {
        rule.validator = validators[rule.validator]
      }
    }
    if (!rule.message) {
      if (rule.required) {
        rule.message = getMessage('required', prop)
      } else if (rule.type === 'email') {
        rule.message = getMessage('emailAddress', prop)
      } else if (rule.pattern) {
        rule.message = format(messages.pattern.mismatch, property.title)
      } else if (
        property.type === 'string' ||
        property.type === 'number' ||
        property.type === 'array'
      ) {
        if (rule.len) {
          rule.message = format(
            messages[property.type].len,
            property.title,
            rule.len,
          )
        } else if (rule.min) {
          rule.message = format(
            messages[property.type].min,
            property.title,
            rule.min,
          )
        } else if (rule.max) {
          rule.message = format(
            messages[property.type].max,
            property.title,
            rule.max,
          )
        } else if (rule.range) {
          rule.message = format(
            messages[property.type].range,
            property.title,
            rule.range,
          )
        }
      }
    } else {
      rule.message = getMessage(rule.message, prop)
    }
  })
  return rules
}

Object.assign(Schema.messages, messages)
Object.assign(Schema.validators, validators)

function required() {
  return { required: true }
}

function trim(message) {
  return {
    pattern: '^(?!\\s).*(?<!\\s)$',
    message: message ?? '无效的空格字符',
    trigger: 'blur',
  }
}

export { getRules, required, trim }
