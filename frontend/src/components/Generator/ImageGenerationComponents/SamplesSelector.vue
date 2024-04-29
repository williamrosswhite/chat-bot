<template>
  <div class="bottom-element">
    <div class="option-container">
        <label for="samples" class="label">Samples:</label>
        <b-form-select v-model="selectedSample" :options="samplesOptions" class="select"></b-form-select>
    </div>
    <p class="small less-margin-top">(Number of images to generate)</p>
  </div>
</template>

<script>
import { BFormSelect } from 'bootstrap-vue-3'

export default {
  name: 'SamplesSelector',
  components: {
    BFormSelect
  },
  props: {
    model: {
      type: String,
      required: true
    },
    samples: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      samplesOptions: [
        { value: 1, text: '1' },
        { value: 2, text: '2' },
        { value: 3, text: '3' },
        { value: 4, text: '4' }
      ]
    }
  },
  computed: {
    selectedSample: {
      get() {
        return this.samples;
      },
      set(value) {
        this.$emit('update:samples', value);
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

.bottom-element {
  margin-bottom: 25px;
}
</style>