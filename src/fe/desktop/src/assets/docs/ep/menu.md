# 菜单 Menu

```mermaid
flowchart LR
从路由获取菜单项 --> 遍历菜单项 --> 有子元素 --> 设为分组
遍历菜单项 --> 没有子元素 --> 设为菜单 --> 点击菜单 --> 站内路由跳转
点击菜单 --> 站外打开链接
```