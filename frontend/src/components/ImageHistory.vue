<template>
  <div v-for="(image, index) in images" :key="index">
    <img :src="image" :alt="'Image ' + index">
  </div>
  <ImageHistoryTiles />
</template>

<script>
import axios from 'axios'
import ImageHistoryTiles from './HistoryModules/ImageHistoryTiles'

export default {
  name: 'ImageHistory',
  components: {
    ImageHistoryTiles
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
          console.log('images gotten', response.data);
          this.images = response.data
        })
    },
  },
  created() {
    this.getImages()
  }
}
</script>