<template>
  <div>
    <ImageDeck v-if="images.length > 0" :images="images" />
    <div ref="observer"></div>
    <p v-if="isLoading" class="loading-text">Retrieving more pictures...</p>
  </div>
</template>

<script>
import axios from 'axios'
import ImageDeck from '@/components/ImageComponents/ImageDeck.vue'
import { ImageDataDTO } from '@/models/ImageDataDTO.js'

export default {
  name: 'ImageHistory',
  components: {
    ImageDeck
  },
  data() {
    return {
      images: [],
      imageCount: 25,
      isLoading: false
    }
  },
  methods: {
    getImages() {
      this.isLoading = true;
      axios.get(`${process.env.VUE_APP_API_URL}/api/images`, {
        params: {
          limit: this.imageCount,
          offset: this.images.length
        }
      })
        .then(response => {
          this.images.push(...response.data.value.map(item => new ImageDataDTO(item)));
          this.isLoading = false;
        });
    }
  },
  mounted() {
    const options = {
      root: null,
      rootMargin: '0px',
      threshold: 1.0
    }

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          this.getImages();
        }
      });
    }, options);

    observer.observe(this.$refs.observer);
  }
}
</script>

<style scoped>
.loading-text {
  padding-top: 20px;
}
</style>