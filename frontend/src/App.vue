<template>
  <img alt="Vue logo" src="./assets/logo.png">
  <ChatBot msg="Welcome to Your Vue.js App"/>
</template>

<script>
import ChatBot from './components/ChatBot.vue'

export default {
  name: 'App',
  components: {
    ChatBot
  }
}
</script>

<style>
 @import './assets/global.css';
</style>