<template>
  <div>
    <div class="option-container">
      <label for="denoisingSteps" class="label">Inference / Denoising Steps:</label>
      <b-form-select v-model="inferenceDenoisingStepsSelected" :options="inferenceDenoisingStepsOptions" class="select"></b-form-select>
    </div>
    <p class="small less-margin-top">(Higher values produce cleaner images)</p>
  </div>
</template>

<script>
import { BFormSelect } from 'bootstrap-vue-3'

export default {
  name: 'DenoisingSelector',
  components: {
    BFormSelect
  },
  props: {
    inferenceDenoisingSteps: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      inferenceDenoisingStepsOptions: [
        { value: 0, text: '0' },
        { value: 21, text: '21' },
        { value: 31, text: '31' },
        { value: 41, text: '41' },
        { value: 51, text: '51' }
      ]
    }
  },
  computed: {
    inferenceDenoisingStepsSelected: {
      get() {
        return this.inferenceDenoisingSteps;
      },
      set(value) {
        this.$emit('inference-denoising-steps-change', value);
      }
    }
  }
}
</script>

<style scoped>
.small {
  font-size: 0.8em;
}

.less-margin-top {
  margin-bottom: 1em;
}

.option-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1em;
}

.label {
  margin-right: 1em;
}

.select {
  flex-grow: 1;
}
</style>