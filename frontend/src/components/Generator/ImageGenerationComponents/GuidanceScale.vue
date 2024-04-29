<template>
  <div class="generation-option slider-component">
    <label for="slider" class="label">Guidance Scale:</label>
    <input type="range" id="slider" v-model.number="localGuidanceScale" min="1" max="20" @input="emitValue">
    <span class="more-margin-left">{{ localGuidanceScale }}</span>
    <p class="small">(Controls how strictly image keeps to the prompt)</p>
  </div>
</template>

<script>
export default {
  props: {
    model: {
      type: String,
      required: true
    },
    guidanceScale: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      localGuidanceScale: this.guidanceScale
    };
  },
  watch: {
    guidanceScale(newVal) {
      this.localGuidanceScale = newVal;
    }
  },
  methods: {
    emitValue() {
      this.$emit('update:guidanceScale', this.localGuidanceScale);
    }
  }
};
</script>

<style scoped>
.slider-component {
  margin-bottom: 15px;
}
</style>