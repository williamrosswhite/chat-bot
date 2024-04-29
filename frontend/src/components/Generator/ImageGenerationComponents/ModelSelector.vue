<template>
  <div class="option-container">
    <label for="model" class="label">Model:</label>
    <b-form-select v-model="selectedModel" :options="modelOptions" class="select"></b-form-select>
  </div>
</template>

<script>
import { BFormSelect } from 'bootstrap-vue-3'

export default {
  components: {
    BFormSelect
  },
  props: {
    model: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      modelOptions: [
        { value: 'stable-diffusion', text: 'Stable Diffusion' },
        { value: 'dall-e-2', text: 'dall-e-2' },
        { value: 'dall-e-3', text: 'dall-e-3' }
      ]
    };
  },
  computed: {
    selectedModel: {
      get() {
        return this.model;
      },
      set(value) {
        this.$emit('model-change', value);
      }
    }
  }
};
</script>