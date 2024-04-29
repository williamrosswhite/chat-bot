<template>
  <div class="generation-option">
    <label for="style" class="label">Style:</label>
    <input type="radio" id="natural" value="natural" v-model="localStyle" class="more-margin-right">
    <label for="natural" class="label">Natural</label>
    <input type="radio" id="vivid" value="vivid" v-model="localStyle" class="more-margin-right">
    <label for="vivid">Vivid</label>
  </div>
</template>

<script>
export default {
  props: ['model', 'style'],
  data() {
    return {
      localStyle: this.style
    };
  },
  watch: {
    localStyle(newStyle) {
      this.$emit('style-change', newStyle);
    },
    style(newStyle) {
      this.localStyle = newStyle;
    }
  }
};
</script>