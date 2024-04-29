<template>
  <input type="text" v-model="secretInput" placeholder="">
</template>

<script>
export default {
  data() {
    return {
      secretInput: ''
    }
  },
  watch: {
    secretInput(newVal) {
      this.$emit('input-change', newVal);
    }
  }
}
</script>