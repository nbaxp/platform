import { defineStore } from 'pinia'

export default defineStore('tabs', {
  state: () => ({
    routes: [],
    isRefreshing: false,
  }),
  actions: {
    addRoute(route) {
      if (!this.routes.find(o => o.fullPath === route.fullPath)) {
        this.routes.push(route)
      } else {
        const index = this.routes.findIndex(o => o.fullPath === route.fullPath)
        this.routes[index] = route
      }
    },
  },
})
