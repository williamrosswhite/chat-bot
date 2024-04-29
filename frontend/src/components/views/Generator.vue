<template>
  <div id="app">
    <ImageGenerator />
    <hr>
    <TextChat />
    <br>
  </div>
</template>

<script>
import TextChat from '@/components/Generator/TextChat.vue'
import ImageGenerator from '@/components/Generator/ImageGenerator.vue'

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
 @import '@/assets/global.css';
</style>