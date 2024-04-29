<template>
    <div class="deck-container">
      <div class="card-container">
        <ImageItem v-for="(image, index) in images" :key="index" :image="image" />
      </div>
    </div>
</template>


<script>
import ImageItem from './ImageItem.vue';

export default {
  components: {
    ImageItem
  },
  props: {
    images: {
      type: Array,
      required: true
    }
  }
}
</script>

<style scoped>
.deck-container {
  display: flex;
  justify-content: center;
  width: 100%;
  max-width: 97%;
  margin: 0 auto; /* Centers the container horizontally */
  box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1); /* Adds a shadow to the container */
  padding: 10px; /* Adds some space around the images */
}

.card-container {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 10px;
  width: 100%;
}
</style>