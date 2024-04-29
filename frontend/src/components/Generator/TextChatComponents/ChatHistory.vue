<template>
  <div>
    <div v-for="(message, index) in history" :key="index">
      <p :class="message.role">{{ message.content }}</p>
    </div>
  </div>
</template>

<script>
export default {
  props: ['history']
}
</script>