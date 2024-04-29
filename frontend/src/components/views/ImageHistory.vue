<template>
  <ImageDeck v-if="images.length > 0" :images="images" />  
</template>

<script>
import axios from 'axios'
import ImageDeck from '@/components/ImageComponents/ImageDeck.vue'

export default {
  name: 'ImageHistory',
  components: {
    ImageDeck
  },
  data() {
    return {
      images: []
    }
  },
  methods: {
    getImages() {
      console.log('getting images, this could take a while...');
      axios.get(`${process.env.VUE_APP_API_URL}/api/images`)
        .then(response => {
          console.log('images received:', response.data);
          this.images.push(...response.data.value.map(item => ({
              imageUrl: item.imageUrl,
              imagePromptText: item.imagePromptText,
              model: item.model,
              guidanceScale: item.guidanceScale,
              hd: item.hd,
              inferenceDenoisingSteps: item.inferenceDenoisingSteps,
              samples: item.samples,
              seed: item.seed,
              size: item.size,
              style: item.style
            })));    
        })
    },
  },
  created() {
    this.getImages()
  }
}
</script>