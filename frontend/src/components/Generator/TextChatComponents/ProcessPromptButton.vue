<template>
  <div>
    <b-button pill variant="success" :disabled="isButtonDisabled" @click="emitClick" class="process-image-button">Process Prompt</b-button>
  </div>
</template>

<script>
export default {
  props: ['isButtonDisabled'],
  methods: {
    emitClick() {
      this.$emit('click');
    }
  }
}
</script>