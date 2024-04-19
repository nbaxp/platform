# 思维导图

- [x] Master
  - [x] Layout
  - [x] Header
    - [x] Logo
    - [x] Nav
  - [x] Footer
  - [x] Main
    - [x] Menu
    - [x] Main
      - [x] Tabs
      - [x] Breadcrumb
      - [x] Content
- [x] Views
  - [ ] Login
  - [ ] Register
  - [ ] Redirect
  - [ ] 403
  - [ ] 404
- [x] Components
  - [x] Markdown
  - [x] Chart
  - [x] Editor
  - [x] Icon
    - [x] Svg Icon
    - [x] Icon Select
  - [x] Form
    - [ ] Input
      - [x] Select
      - [x] Cascader
      - [ ] File
      - [ ] Filter
    - [ ] Validation
      - [ ] Remote
  - [x] List
    - [x] Query From
    - [x] Table List
      - [x] SubList Drawer
    - [x] Pagination
    - [x] Options
      - [x] Delete
      - [x] Dialog
      - [x] Reset
      - [x] Print
      - [x] Dialog
        - [x] Details
        - [x] Create
        - [x] Update
        - [x] Export
        - [x] Import
        - [x] Filter

## 应用启动

```mermaid
flowchart LR

createPinia --> useMock
createI18n --> useMock
createRouter --> useMock
useMock --> createApp
createApp --> store["app.use(store)"]  --> app.mount
createApp --> i18n["app.use(i18n)"] --> app.mount
createApp --> router["app.use(router)"] --> app.mount
```

## 路由守卫

```mermaid
flowchart LR
createRouter --> beforeEach --> locale[fetch locale] --> menu[fetch menu] --> refresh[refresh token] --> login[valid permission]
```

```mermaid
flowchart LR
起步 -->Git
起步 -->Markdown --> Web基础 --> HTML
起步 -->开发环境 --> 开发工具 --> vscode(VS Code)
开发环境 --> 运行时 --> 浏览器
运行时 --> Node --> Vite
Web基础 --> CSS
Web基础 --> JavaScript --> 语言
JavaScript --> 库 --> Vue --> 基础
基础 --> 模块化
Vue --> 国际化 --> i18n(Vue I18n)
Vue --> 路由 --> router(Vue Router)
Vue --> 状态管理 --> Pinia
Vue --> 图表 --> echarts(Vue ECharts)
Vue --> ui(UI 框架)-->PC(桌面浏览器) --> desktop(Element Plus\Arco Design\TDesign\Ant Design Vue)
ui --> 移动端 --> Vant
ui --> 小程序 --> UniApp
```

```mermaid
flowchart LR
useMock --> createApp --> mixin --> useStore --> useLocale --> useRouter --> useElementPlus --> mount
App--> ElConfigProvider --> r1(RouterView) --> Layout --> r2(RouterView) --> PageView
```
