<template>
  <ImageDeck v-if="images.length > 0" :images="images" />  
  <p class="processing" v-show="isLoading">Processing...</p>
  <b-spinner v-if="isLoading" class="spinner" type="grow" label="Loading..."></b-spinner>
  <div class="overlay" v-if="isLoading"></div>
  <br>
  <p class="describe">Describe what you'd like to see...</p>
  <ImageTextArea @prompt-change="handlePromptChange" @enter-clicked="processImagePrompt"/>
  <div class="selectors">
    <ModelSelector :model="model" @model-change="handleModelChange" />
    <SizeSelector :model="model" :size="size" @size-change="handleSizeChange" />
    <StyleSelector v-show="model === 'dall-e-3'" :model="model" :style="style" @style-change="handleStyleChange" />
    <HdCheckbox :hd="hd" @hd-change="handleHdChange" />
  </div>
  <div v-show="model === 'stable-diffusion'" class="alert alert-warning warning-banner" role="alert">Warning: Stable Diffusion can produce NSFW Images</div>
  <div v-show="model === 'dall-e-3'" class="alert alert-warning warning-banner dalle3-warning" role="alert">Note: dal-e-3 costs me per use.  Not a lot so don't worry about it, just plese be mindful.  Stable Diffusion is subscription and you can use freely.</div>
  <GuidanceScale  v-show="model === 'stable-diffusion'" :guidanceScale="guidanceScale" :model="model" @guidance-scale-change="handleGuidanceScaleChange" />
  <InferenceDenoisingSteps :inferenceDenoisingSteps="inferenceDenoisingSteps" @inference-denoising-steps-change="handleInferenceDenoisingStepsChange" v-show="model === 'stable-diffusion'"/>
  <SamplesSelector v-show="model === 'stable-diffusion' || model === 'dall-e-2'" :model="model" :samples="samples" @update:samples="samples = $event" />
  <ProcessImageButton variant="success" :isDisabled="isButtonDisabled" @click="processImagePrompt" />   
  <b-spinner v-if="isLoading" class="spinner" type="grow" label="Loading..."></b-spinner> 
  <SeedInput v-show="model === 'stable-diffusion'" :seedValue="seed.toString()" @update:seedValue="handleUpdateSeedValue" @seed-cleared="seed = ''"/>
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
  import InferenceDenoisingSteps from '@/components/Generator/ImageGenerationComponents/InferenceDenoisingSteps.vue';
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
        size: '1024x1024',
        hd: false,
        style: 'natural',
        guidanceScale: 1, // initial value for the slider
        inferenceDenoisingSteps: 0, // default value for the denoising steps
        samples: 1, // default value for the number of images to generate
        seed: '',
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
          this.size = '1024x1024';
        }
      },
    },
    methods: {
      async processImagePrompt() {
        this.isLoading = true;
        console.log(this.inferenceDenoisingSteps)
        try {
          const imageDataDTO = new ImageDataDTO({
            imagePromptText: this.userImagePromptText,
            model: this.model,
            size: this.size,
            style: this.style,
            hd: this.hd,
            samples: this.samples,
            guidanceScale: this.guidanceScale,
            inferenceDenoisingSteps: this.inferenceDenoisingSteps,
            seed: this.seed
          });

          const responseData = await processImagePrompt(imageDataDTO);
          console.log('Image request response:', responseData);
          const mappedData = mapImageData(responseData);
          this.images.push(...mappedData);

          if (responseData.length > 0 && this.model === 'stable-diffusion') {
            console.log(responseData[0].seed);
            this.seed = responseData[0].seed;
          }
        } catch (error) {
          if (error.response && error.response.data && error.response.data.message) {
            alert(error.response.data);
          } else {
            alert(error.message);
          }
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
      handleInferenceDenoisingStepsChange(newSteps) {
        this.inferenceDenoisingSteps = newSteps;
      },
      handleSizeChange(newSize) {
        this.size = newSize;
      },
      handleGuidanceScaleChange(newScale) {
        this.guidanceScale = newScale;
      },
      handleUpdateSeedValue(newSeedValue) {
        this.seed = newSeedValue;
      },
      handleHdChange(newHdValue) {
        this.hd = newHdValue;
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
      InferenceDenoisingSteps,
      SamplesSelector,
      ProcessImageButton,
      SeedInput,
      BSpinner
    }    
  };
</script>


<style scoped>
.selectors {
  padding-top: 20px;
  padding-bottom: 20px; 
}

.processing {
  margin-top: 40px;
  margin-bottom: 0px;
}

.describe {
  margin-top: 10px;
  margin-bottom: 40px;
}

.overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 9999;
}

.warning-banner {
  display: block;
  width: 100%;
  padding: 10px 50px;
  margin: 0 auto; /* centers the element horizontally */
  max-width: 85%;;
}

.spinner {
  display: flex;
  justify-content: center;
  align-items: center;
  margin: auto;
  margin-top: 30px;
  color: green
}

.dalle3-warning {
  margin-bottom: 30px;;
}

@media (max-width: 720px) {
  #warning-banner {
    display: inline-block;
    padding: 10px 15px;
    margin-top: 0px;
  }
}
</style>