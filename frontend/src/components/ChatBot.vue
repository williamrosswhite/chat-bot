<template>
  <div id="app">
    <ImageGenerator />
    <br><br>
    <TextChat />
    <br>
  </div>
</template>

<script>
import TextChat from './ChatModules/TextChat.vue'
import ImageGenerator from './ChatModules/ImageGenerator.vue'

export default {
  name: 'App',
  components: {
    TextChat,
    ImageGenerator
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only (<style scoped>) -->
<style>
 @import '../assets/global.css';
</style>