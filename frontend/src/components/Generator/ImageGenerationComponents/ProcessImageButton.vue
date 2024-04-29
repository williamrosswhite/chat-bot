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