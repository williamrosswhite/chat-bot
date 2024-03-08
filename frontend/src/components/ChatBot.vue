<template>
  <div id="app">
    <TextChat />
  </div>
</template>

<script>
import TextChat from './ChatModules/TextChat.vue'

export default {
  name: 'App',
  components: {
    TextChat
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only (<style scoped>) -->
<style>
 @import '../assets/global.css';
</style>