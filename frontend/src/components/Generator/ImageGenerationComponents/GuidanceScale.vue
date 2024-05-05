<template>
  <div class="generation-option slider-component">
    <label for="slider" class="label">Guidance Scale:</label>
    <input type="range" id="slider" v-model.number="guidanceScaleSelected" min="1" max="20" @input="emitValue">
    <span class="more-margin-left">{{ guidanceScaleSelected }}</span>
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
  computed: {
    guidanceScaleSelected: {
      get() {
        return this.guidanceScale;
      },
      set(value) {
        this.$emit('guidance-scale-change', value);
      }
    }
  }
};
</script>

<style scoped>
.slider-component {
  margin-bottom: 15px;
}
</style>