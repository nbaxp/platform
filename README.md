# 说明

## 目录结构

1. README.md: 描述文件
1. .gitignore: git 提交过滤
1. .gitattributes: git 配置文件
1. .editorconfig: 通用格式化配置
1. docs: 文档目录
1. src: 源码目录
   1. fe: 前端
      1. desktop: 桌面端
      1. mobile: 移动端
1. be: 后端
   1. dotnet: .net web 服务
   1. java: java web 服务器

## 开发环境

1. 源码管理: gitea
1. 自动构建: gitea actions
1. 文档格式: markdow（flowchart）
1. 操作系统: windows 11
1. 基础软件: wsl 2 + docker desktop + git windows + tortoisegit
1. 前端: Node.js(nvm) + visual studio code
1. 后端：
   1. .NET: Visual Studio 2022
   1. Java: JDK(jenv) + Intellij IDEA

## 初始化

第一次使用时，为确保 husky + lint-staged 起作用，需要在根目录执行:

```bash
npm install
npx husky init
```
