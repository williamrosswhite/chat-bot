<template>
  <b-button pill variant="success" :disabled="isDisabled" class="process-image-button">
    Process Image Prompt
  </b-button>
</template>

<script>
import { BButton } from 'bootstrap-vue-3'

export default {
  name: 'ProcessImageButton',
  components: {
    BButton
  },
  props: {
    isDisabled: {
      type: Boolean,
      default: false
    }
  }
}
</script>

<style scoped>
/* .process-image-button {
  background-color: #28a745;
  border: none;
  color: white;
  padding: 15px 32px;
  text-align: center;
  text-decoration: none;
  display: inline-block;
  font-size: 16px;
  margin: 4px 2px;
  cursor: pointer;
  border-radius: 50px;
  transition-duration: 0.4s;
}

.process-image-button:hover {
  background-color: white;
  color: #28a745;
}
.process-image-button:disabled {
  background-color: #cccccc;
  color: #666666;
  cursor: not-allowed;
} */
</style>