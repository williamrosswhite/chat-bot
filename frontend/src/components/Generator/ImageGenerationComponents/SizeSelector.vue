<template>
  <div class="option-container">
    <label for="size" class="label">Image size:</label>
    <b-form-select v-model="selectedSize" :options="sizeOptions"></b-form-select>
  </div>
</template>

<script>
import { BFormSelect } from 'bootstrap-vue-3'

export default {
  props: ['model', 'size'],
  data() {
    return {
      selectedSize: '1024x1024',
    };
  },
  computed: {
    sizeOptions() {
      let commonOption = { value: '1024x1024', text: '1024x1024' };
      if (this.model === 'dall-e-2') {
        return [
          { value: '256x256', text: '256x256' },
          { value: '512x512', text: '512x512' },
          commonOption
        ];
      } else if (this.model === 'dall-e-3') {
        return [
          commonOption,
          { value: '1792x1024', text: '1792x1024' },
          { value: '1024x1792', text: '1024x1792' }
        ];
      } else if (this.model === 'stable-diffusion') {
        return [
          { value: '256x256', text: '256x256' },
          { value: '512x512', text: '512x512' },
          commonOption,
          { value: '1024x464', text: '1024x512 (landscape)' },
          { value: '464x1024', text: '464x1024 (portrait)' }
        ];
      }
      return [];
    },
  },
  watch: {
    selectedSize(newSize) {
      this.$emit('size-change', newSize);
    },
    size(newSize) {
      this.selectedSize = newSize;
    }
  },
  components: {
    BFormSelect,
  },
};
</script>