<template>
  <div class="text-area option-container text-box">
    <b-form-textarea
      v-model="text" 
      @keydown.enter="onEnter" 
      @keydown.enter.prevent 
      placeholder="2 character minimum"
    ></b-form-textarea>
  </div>
</template>

<script>
import { BFormTextarea } from 'bootstrap-vue-3'

export default {
  props: {
    text: {
      type: String,
      required: true
    }
  },
  methods: {
    onEnter() {
      this.$emit('enter-pressed');
    }
  },
  components: {
    BFormTextarea
  }
};
</script>