// router.js
import { createRouter, createWebHistory } from 'vue-router'
import ChatBot from './components/ChatBot.vue'
import ImageHistory from './components/ImageHistory.vue'


const routes = [
  { path: '/', component: ChatBot },
  { path: '/imageHistory', component: ImageHistory },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router