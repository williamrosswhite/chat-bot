<template>
  <div v-for="(url, index) in imageUrls" :key="index">
    <img :src="url" alt="Generated image">
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
  <div class="selectors">
    <div class="generationOption">
      <label for="model">Model:</label>
      <select v-model="model">
        <option value="dall-e-2">dall-e-2</option>
        <option value="dall-e-3">dall-e-3</option>
        <option value="stable-diffusion">Stable Diffusion</option>
      </select>
    </div>
    <div class="generationOption" v-if="model === 'dall-e-3' || model === 'dall-e-2'">      
      <label for="size">Image size:</label>
      <select v-model="size">
        <option v-for="option in sizeOptions" :value="option.value" :key="option.value">
          {{ option.text }}
        </option>
      </select>
    </div>
    <div class="generationOption" v-if="model === 'dall-e-3'">
      <label for="style">Style:</label>
      <input type="radio" id="natural" value="natural" v-model="style">
      <label for="natural">Natural</label>
      <input type="radio" id="vivid" value="vivid" v-model="style">
      <label for="vivid">Vivid</label>
    </div>
    <div class="generationOption" v-if="model === 'dall-e-3'">
      <label for="hd">HD:</label>
      <input type="checkbox" id="hd" v-model="hd">
    </div>
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
        imageUrls: [],
        isLoading: false,
        model: 'dall-e-2',
        size: '256x256',
        hd: false,
        style: 'natural'
      };
    },
    computed: {
      sizeOptions() {
        let commonOption = { value: '1024x1024', text: '1024x1024' };
        if (this.model === 'dall-e-2') {
          return [
            { value: '256x256', text: '256x256' },
            { value: '512x512', text: '512x512' },
            commonOption
          ];
        } else if (this.model === 'dall-e-3') {
          return [
            commonOption,
            { value: '1792x1024', text: '1792x1024' },
            { value: '1024x1792', text: '1024x1792' }
          ];
        }
        return [];
      }
    },
    watch: {
      model(newModel) {
        if (newModel === 'dall-e-2') {
          this.size = '256x256';
          this.hd = false;
          this.style = 'natural';
        }
        if (newModel === 'dall-e-3') {
          this.size = '1024x1024';
        }
      }
    },
    methods: {
      processImagePrompt() {
        if(this.model === 'dall-e-3' || this.model === 'dall-e-2') {
          this.isLoading = true;
          console.log('sending...', this.userImagePromptText);
          axios.post(`${process.env.VUE_APP_API_URL}/openapi/ImageRequest`, { 
            imagePromptText: this.userImagePromptText,
            model: this.model,
            size: this.size,
            style: this.style === 'natural' ? true : false,      
            hd: this.hd
          })
          .then(response => {
            if(response.data.error?.code === "rate_limit_exceeded") {
              alert('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.')
            } else {
              let base64Image = response.data.data[0]?.b64_json;
              if(base64Image != undefined) {
                console.log(`Displaying returned image for: ${this.userImagePromptText}`);          
                this.imageUrls.push(`data:image/jpeg;base64,${base64Image}`);
              } else {
                alert('Image generation failed, try again!');
              }
            }
            this.isLoading = false;
          })
          .catch(error => {
            console.error('Error processing image prompt:', error);
            alert('Error processing image prompt, try again!  (You may have tripped the content moderation)');
            this.isLoading = false;
          })
        } else if (this.model === 'stable-diffusion') {
          this.isLoading = true;
          console.log('sending...', this.userImagePromptText);
          axios.post(`${process.env.VUE_APP_API_URL}/stablediffusion/ImageRequest`, { 
            imagePromptText: this.userImagePromptText,
            model: this.model,
            size: this.size,
            style: this.style === 'natural' ? true : false,      
            hd: this.hd
          })
          .then(response => {
            if(response.data.error?.code === "rate_limit_exceeded") {
              alert('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.')
            } else {
              console.log(response)
              this.imageUrls.push(response.data?.output[0]);
              // let base64Image = response.data.data[0]?.b64_json;
              // if(base64Image != undefined) {
              //   console.log(`Displaying returned image for: ${this.userImagePromptText}`);          
              //   this.imageUrls.push(`data:image/jpeg;base64,${base64Image}`);
              // } else {
              //   alert('Image generation failed, try again!');
              // }
            }
            this.isLoading = false;
          })
          .catch(error => {
            console.error('Error processing image prompt:', error);
            alert('Error processing image prompt, try again!  (You may have tripped the content moderation)');
            this.isLoading = false;
          })
        }
      }
    }
  };
</script>