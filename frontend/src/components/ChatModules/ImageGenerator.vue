<template>
  <div v-for="(url, index) in imageUrls" :key="index">
    <img :src="url" alt="Generated image">
  </div>
  <p v-show="isLoading">Processing...</p>
  <br>
  <p>Describe what you'd like to see...</p>
  <div class="text-area option-container">
    <b-form-textarea
      v-model="userImagePromptText" 
      @keydown.enter="processImagePrompt" 
      @keydown.enter.prevent 
      placeholder="2 character minimum"
      no-auto-shrink
    ></b-form-textarea>
  </div>
  <div class="selectors">
    <div class="option-container">
      <label for="model" class="label">Model:</label>
      <b-form-select v-model="model" :options="modelOptions" class="select"></b-form-select>
    </div>
    <div class="option-container">
      <label for="size" class="label">Image size:</label>
      <b-form-select v-model="size" :options="sizeOptions"></b-form-select>
    </div>
    <div class="generation-option" v-if="model === 'dall-e-3'">
      <label for="style" class="label">Style:</label>
      <input type="radio" id="natural" value="natural" v-model="style" class="radio-button">
      <label for="natural" class="label">Natural</label>
      <input type="radio" id="vivid" value="vivid" v-model="style" class="radio-button">
      <label for="vivid">Vivid</label>
    </div>
    <div class="checkbox-container">
      <div class="generation-option" v-if="model === 'dall-e-3' || model === 'stable-diffusion'">
        <label for="hd" class="radio-button">HD:</label>
        <input type="checkbox" id="hd" v-model="hd">
      </div>
    </div>
  </div>
  <div id="warning-banner" v-show="model === 'stable-diffusion'" class="alert alert-warning" role="alert">
    Warning: Stable Diffusion can produce NSFW Images
  </div>
  <div class="generation-option slider-component" v-if="model === 'stable-diffusion'">
    <label for="slider" class="label">Guidance Scale:</label>
    <input type="range" id="slider" v-model.number="guidanceScale" min="1" max="20">
    <span class="slider-value">{{ guidanceScale }}</span>
    <p class="small">(Controls how strictly image keeps to the prompt)</p>
  </div>
  <div v-if="model === 'stable-diffusion'">
    <div class="option-container">
        <label for="denoisingSteps" class="label">Interference / Denoising Steps:</label>
        <b-form-select v-model="interferenceDenoisingSteps" :options="interferenceDenoisingStepsOptions" class="select"></b-form-select>
    </div>
      <p class="small denoising-info">(Higher values produce cleaner images)</p>
  </div>
  <div class="bottom-element" v-if="model === 'stable-diffusion' || model === 'dall-e-2'">
    <div class="option-container">
        <label for="samples" class="label">Samples:</label>
        <b-form-select v-model="samples" :options="samplesOptions" class="select"></b-form-select>
    </div>
      <p class="small denoising-info">(Number of images to generate)</p>
  </div>
  <div>
    <b-button pill variant="success" :disabled="isButtonDisabled" @click="processImagePrompt" class="process-image-button">Process Image Prompt</b-button>
  </div>
  <div v-if="model === 'stable-diffusion'">
    <div class="option-container seed">
          <label for="seed" class="label">Seed:</label>
          <b-form-input v-model="seed" type="text" pattern="[0-9]*"></b-form-input>
          <b-button pill variant="outline-success" @click="clearSeed" class="process-image-button slider-value">Clear</b-button>
    </div>
    <p class="small denoising-info">(The same seed with the same parameters creates<br>the same image.  Leave blank to randomize)</p>
  </div>  
</template>
  
<script>
  import ImageGeneratorService from '@/services/ImageGeneratorService';
  import { BFormSelect, BButton, BFormTextarea, BFormInput } from 'bootstrap-vue-3'

  export default {
    data() {
      return {
        userImagePromptText: '',
        imageUrls: [],
        isLoading: false,
        model: 'dall-e-2',
        size: '256x256',
        hd: false,
        style: 'natural',
        guidanceScale: 1, // initial value for the slider
        interferenceDenoisingSteps: 0, // default value for the denoising steps
        samples: 1, // default value for the number of images to generate
        seed: ""
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
        } else if (this.model === 'stable-diffusion') {
          return [
            { value: '256x256', text: '256x256' },
            { value: '512x512', text: '512x512' },
            commonOption,
            { value: '512x1024', text: '512x1024 (portrait)' },
            { value: '1024x512', text: '1024x512 (landscape)' },
            { value: '464x1024', text: '464x1024 (phone)' }
          ];
        }
        return [];
      },
      interferenceDenoisingStepsOptions() {
        return [
            { value: 0, text: '0' },
            { value: 21, text: '21' },
            { value: 31, text: '31' },
            { value: 41, text: '41' },
            { value: 51, text: '51' }
        ];
      },
      samplesOptions() {
        return [
            { value: 1, text: '1' },
            { value: 2, text: '2' },
            { value: 3, text: '3' },
            { value: 4, text: '4' }
        ];
      },
      modelOptions() {
        return [
          { value: 'dall-e-2', text: 'dall-e-2' },
          { value: 'dall-e-3', text: 'dall-e-3' },
          { value: 'stable-diffusion', text: 'Stable Diffusion' }
        ];
      },
      isButtonDisabled() {
        return this.isLoading || this.userImagePromptText.length < 2;
      },
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
          this.samples = 1; // reset samples to 1
        }
      }
    },
    methods: {
      async processImagePrompt() {
        this.isLoading = true;
        try {
          if(this.model === 'dall-e-3' || this.model === 'dall-e-2') {
            console.log('sending...', this.userImagePromptText, "to OpenAI");
            const generatedImageUrls = await ImageGeneratorService.processImagePromptOpenAi(this.userImagePromptText, this.model, this.size, this.style, this.hd, this.samples);
            console.log('received response: ', generatedImageUrls);
            this.imageUrls.push(...generatedImageUrls.map(item => item.url));
          } else if (this.model === 'stable-diffusion') {
            console.log("seed: ", this.seed);
            console.log('sending...', this.userImagePromptText, "to Stable Diffusion");
            const responseData = await ImageGeneratorService.processImagePromptStableDiffusion(
              this.userImagePromptText, 
              this.size, 
              this.hd, 
              this.guidanceScale, 
              this.samples, 
              this.interferenceDenoisingSteps, 
              this.seed
            );
            console.log('received response: ', responseData);
            this.imageUrls.push(...responseData.output);
            this.seed = responseData.seed;
          }
        } catch (error) {
          alert(error.message);
        } finally {
          this.isLoading = false;
        }
      },
      clearSeed() {
        this.seed = "";
      }
    },
    components: {
      BFormSelect,
      BButton,
      BFormTextarea,
      BFormInput
    }    
  };
</script>