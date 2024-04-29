<template>
  <div>
    <div class="option-container seed">
      <label for="seed" class="label">Seed:</label>
      <b-form-input v-model="seed" type="text" pattern="[0-9]*"></b-form-input>
      <b-button pill variant="outline-success" @click="handleClearSeed" class="process-image-button slider-value">Clear</b-button>
    </div>
    <p class="small denoising-info">(The same seed with the same parameters creates<br>the same image.  Leave blank to randomize)</p>
  </div>
</template>

<script>
import { BFormInput, BButton } from 'bootstrap-vue-3'

export default {
  name: 'SeedInput',
  components: {
    BFormInput,
    BButton
  },
  data() {
    return {
      seed: ''
    };
  },
  methods: {
    handleClearSeed() {
      this.seed = '';
      this.$emit('seed-cleared');
    },
    updateSeed(newSeed) {
      this.seed = newSeed;
      this.$emit('input', this.seed);
    }
  },
  emits: ['seed-cleared']
}
</script>