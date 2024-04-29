<template>
  <div>
    <div class="option-container seed">
      <label for="seed" class="label">Seed:</label>
      <b-form-input v-model="localSeed" @keypress="enforceNumericInput" type="text"></b-form-input>
      <b-button pill variant="outline-success" @click="handleClearSeed" class="process-image-button more-margin-left">Clear</b-button>
    </div>
    <p class="small less-margin-top">(The same seed with the same parameters creates<br>the same image.  Leave blank to randomize)</p>
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
  props: {
    seedValue: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      localSeed: this.seedValue
    };
  },
  methods: {
    handleClearSeed() {
      this.localSeed = '';
      this.$emit('update:seedValue', this.localSeed);
    },
    enforceNumericInput(event) {
      if (!/\d/.test(String.fromCharCode(event.keyCode))) {
        event.preventDefault();
      }
    }
  },
  watch: {
    localSeed(newSeed) {
      this.$emit('update:seedValue', newSeed);
    },
    seedValue(newSeedValue) {
      this.localSeed = newSeedValue;
    }
  }
}
</script>

<style scoped>
.seed {
  padding-top: 30px;
}
</style>