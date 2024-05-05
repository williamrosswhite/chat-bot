<template>
  <div class="generation-option">
    <label for="hd" class="radio-button">HD:</label>
    <input v-model="hdChecked" type="checkbox">
  </div>
</template>

<script>
export default {
  props: {
    hd: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    hdChecked: {
      get() {
        return this.hd;
      },
      set(value) {
        this.$emit('hd-change', value);
      }
    }
  }
};
</script>