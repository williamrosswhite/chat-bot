<template>
  <div class="text-area option-container text-box">    
    <b-form-textarea
      v-model="text"
      @keydown.enter="emitText"
      @keydown.enter.prevent
      placeholder="Enter text here..."
    ></b-form-textarea>
  </div>
</template>

<script>
import { BFormTextarea } from 'bootstrap-vue-3'

export default {
  data() {
    return {
      text: ''
    }
  },
  watch: {
    text(newText) {
      this.$emit('prompt-change', newText);
    }
  },
  methods: {
    emitText() {
      this.$emit('enter-clicked', this.text);
      this.text = '';
    }
  },
  components: {
    BFormTextarea
  }
}
</script>