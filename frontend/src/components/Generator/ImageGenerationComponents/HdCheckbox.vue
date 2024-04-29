<template>
  <div class="generation-option">
    <label for="hd" class="more-margin-right">HD:</label>
    <input type="checkbox" id="hd" v-model="checked" @change="onChange">
  </div>
</template>

<script>
export default {
  props: {
    model: {
      type: String,
      required: true
    },
    value: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    checked: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit('input', val);
      }
    }
  },
  methods: {
    onChange(event) {
      this.$emit('input', event.target.checked);
    }
  }
};
</script>