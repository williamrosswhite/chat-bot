<template>
  <div>
    <img :src="imageUrl" alt="Generated image" v-if="imageUrl">
  </div>
  <p v-show="isLoading">Processing...</p>
  <br>
  <div>
    <textarea 
      v-model="userImagePromptText" 
      @keydown.enter="processImagePrompt" 
      @keydown.enter.prevent 
      placeholder="Describe and image you'd like to see...">
    </textarea>
  </div>
  <div>
    <button :disabled="isLoading" @click="processImagePrompt">Process Image Prompt</button>
  </div>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        userImagePromptText: '',
        imageUrl: '',
        isLoading: false
      };
    },
methods: {
  processImagePrompt() {
    this.isLoading = true;
    console.log('sending...', this.userImagePromptText);
    axios.post(`${process.env.VUE_APP_API_URL}/api/ImageRequest`, { 
      imagePromptText: this.userImagePromptText      
    })
    .then(response => {
      if(response.data.error?.code === "rate_limit_exceeded") {
        alert('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.')
      } else {
        let base64Image = response.data.data[0]?.b64_json;
        if(base64Image != undefined) {
          console.log(`Displaying returned image for: ${this.userImagePromptText}`);          
          this.imageUrl = `data:image/jpeg;base64,${base64Image}`;
        } else {
          alert('Image generation failed, try again!');
        }
      }
      this.isLoading = false;
    })
    .catch(error => {
      console.error('Error processing image prompt:', error);
      alert('Error processing image prompt, try again!');
      this.isLoading = false;
    })
  }
}
  };
  </script>