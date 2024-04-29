<template>
  <ImageDeck v-if="images.length > 0" :images="images" />  
  <p class="processing" v-show="isLoading">Processing...</p>
  <b-spinner v-if="isLoading" type="grow" label="Loading..."></b-spinner>
  <div class="overlay" v-if="isLoading"></div>
  <br>
  <p class="describe">Describe what you'd like to see...</p>
  <ImageTextArea @prompt-change="handlePromptChange" @enter-clicked="processImagePrompt"/>
  <div class="selectors">
    <ModelSelector :model="model" @model-change="handleModelChange" />
    <SizeSelector :model="model" :size="size" @size-change="handleSizeChange" />
    <StyleSelector v-show="model === 'dall-e-3'" :model="model" :style="style" @style-change="handleStyleChange" />
    <HdCheckbox  v-show="model === 'dall-e-3' || model === 'stable-diffusion'" v-model="hd" :model="model" />
  </div>
  <div id="warning-banner" v-show="model === 'stable-diffusion'" class="alert alert-warning" role="alert">Warning: Stable Diffusion can produce NSFW Images</div>
  <GuidanceScale  v-show="model === 'stable-diffusion'" :guidanceScale="guidanceScale" :model="model" @update:modelValue="handleGuidanceScaleChange" />
  <InterferenceDenoisingSteps v-show="model === 'stable-diffusion'" :model="model" :interferenceDenoisingSteps="interferenceDenoisingSteps" />
  <SamplesSelector v-show="model === 'stable-diffusion' || model === 'dall-e-2'" :model="model" :samples="samples" @update:samples="samples = $event" />
  <ProcessImageButton :isDisabled="isButtonDisabled" @click="processImagePrompt" />    
  <SeedInput v-show="model === 'stable-diffusion'"/>
</template>
  
<script>
  import { processImagePrompt } from '@/services/ImageProcessingService';
  import { mapImageData } from '@/services/ImageDataMapper';
  import { ImageDataDTO } from '@/models/ImageDataDTO';
  import { BSpinner } from 'bootstrap-vue-3'
  import ImageDeck from '@/components/ImageComponents/ImageDeck.vue';
  import GuidanceScale from '@/components/Generator/ImageGenerationComponents/GuidanceScale.vue';
  import ImageTextArea from '@/components/Generator/ImageGenerationComponents/ImageTextArea.vue';
  import HdCheckbox from '@/components/Generator/ImageGenerationComponents/HdCheckbox.vue';
  import ModelSelector from '@/components/Generator/ImageGenerationComponents/ModelSelector.vue';
  import SizeSelector from '@/components/Generator/ImageGenerationComponents/SizeSelector.vue';
  import StyleSelector from '@/components/Generator/ImageGenerationComponents/StyleSelector.vue';
  import InterferenceDenoisingSteps from '@/components/Generator/ImageGenerationComponents/InterferenceDenoisingSteps.vue';
  import SamplesSelector from '@/components/Generator/ImageGenerationComponents/SamplesSelector.vue';
  import ProcessImageButton from '@/components/Generator/ImageGenerationComponents/ProcessImageButton.vue';
  import SeedInput from '@/components/Generator/ImageGenerationComponents/SeedInput.vue';


  export default {
    data() {
      return {
        userImagePromptText: '',
        images: [],
        isLoading: false,
        model: 'stable-diffusion',
        size: '256x256',
        hd: false,
        style: 'natural',
        guidanceScale: 1, // initial value for the slider
        interferenceDenoisingSteps: 0, // default value for the denoising steps
        samples: 1, // default value for the number of images to generate
        seed: "",
        isButtonDisabled: false
      };
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
        if (newModel === 'stable-diffusion') {
          this.size = '256x256';
        }
      }
    },
    methods: {
      async processImagePrompt() {
        this.isLoading = true;
        try {
          const imageDataDTO = new ImageDataDTO({
            imagePromptText: this.userImagePromptText,
            model: this.model,
            size: this.size,
            style: this.style,
            hd: this.hd,
            samples: this.samples,
            guidanceScale: this.guidanceScale,
            inferenceDenoisingSteps: this.interferenceDenoisingSteps,
            seed: this.seed
          });

          const responseData = await processImagePrompt(imageDataDTO);
          console.log('Image request response:', responseData);
          const mappedData = mapImageData(responseData);
          this.images.push(...mappedData);

          if (responseData.length > 0 && this.model === 'stable-diffusion') {
            this.seed = responseData[0].seed;
          }
        } catch (error) {
          alert(error.message);
        } finally {
          this.isLoading = false;
        }
      },
      handlePromptChange(newText) {
        this.userImagePromptText = newText;
        this.isButtonDisabled = this.isLoading || this.userImagePromptText.length < 2;
      },
      handleModelChange(newModel) {
        this.model = newModel;
      },
      handleStyleChange(newStyle) {
        this.style = newStyle;
      },
      handleDenoisingChange(newSteps) {
        this.interferenceDenoisingSteps = newSteps;
      },
      handleSizeChange(newSize) {
        this.size = newSize;
      },
      handleGuidanceScaleChange(newScale) {
        this.guidanceScale = newScale;
      }
    },
    components: {
      ImageDeck,
      GuidanceScale,
      ImageTextArea,
      HdCheckbox,
      ModelSelector,
      SizeSelector,
      StyleSelector,
      InterferenceDenoisingSteps,
      SamplesSelector,
      ProcessImageButton,
      SeedInput,
      BSpinner
    }    
  };
</script>