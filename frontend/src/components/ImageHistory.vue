<template>
  <ImageHistoryTiles />
</template>

<script>
import $ from 'jquery';
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
      axios.get(`${process.env.VUE_APP_API_URL}/api/images`) // replace with your actual API endpoint
        .then(response => {
          console.log('images gotten');
          this.images = response.data
        })
    },
    slide(direction) {
      $('#carouselExampleIndicators').carousel(direction);
    }
  },
  created() {
    this.getImages()
  }
}
</script>