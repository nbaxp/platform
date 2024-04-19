# Vue 基础

```html
<script type="module">
  import { createApp, ref, reactive, watch, onMounted } from 'vue'

  const simpleComponent = {
    components: {}, //组件注册
    template: `<label>子组件:<input type="text" v-model="model.value"></label>
      <button @click="onClick">click</button>`,
    props: ['modelValue'],
    emit: ['update:modelValue'],
    setup(props, context) {
      const model = reactive(props.modelValue)
      watch(model, value => context.emit('update:modelValue', value))

      const childMethod = () => {
        alert('child method')
      }

      const callback = result => {
        alert(`paretn method callback: ${result}`)
      }
      const onClick = () => {
        context.emit('click', 'call parent method from child', callback)
      }

      context.expose({ childMethod })
      return {
        model,
        childMethod,
        onClick,
      }
    },
  }

  const appComponent = {
    components: { simpleComponent },
    template: `<label>父组件:<input type="text" v-model="model.value" /></label>
      <button @click="onClick">click</button>
      <simple-component ref="childRef" v-model="model" @click="parentMethod" />`,
    props: ['modelValue'],
    setup(props, context) {
      const childRef = ref(null)
      const model = reactive({
        value: 'test',
      })
      const onClick = () => {
        console.log(props, context)
        childRef.value.childMethod()
      }
      const parentMethod = (o, callback) => {
        alert(o)
        callback('from parent')
      }

      onMounted(() => {
        console.log(childRef.value)
      })

      return {
        model,
        childRef,
        onClick,
        parentMethod,
      }
    },
  }

  const app = createApp(appComponent)
  app.mount('#app')
</script>
```
