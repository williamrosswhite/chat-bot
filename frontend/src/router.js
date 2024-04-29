// router.js
import { createRouter, createWebHistory } from 'vue-router'
import Generator from './components/views/Generator.vue'
import ImageHistory from './components/views/ImageHistory.vue'


const routes = [
  { path: '/', component: Generator },
  { path: '/imageHistory', component: ImageHistory },
  { path: '/:catchAll(.*)', redirect: '/' },  // catch-all route
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router