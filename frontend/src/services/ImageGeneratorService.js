// ImageGeneratorService.js
import axios from 'axios';

export default {

  async processImagePromptOpenAi(userImagePromptText, model, size, style, hd, samples) {
    try {
      const response = await axios.post(`${process.env.VUE_APP_API_URL}/openapi/ImageRequest`, { 
        imagePromptText: userImagePromptText,
        model: model,
        size: size,
        style: style === 'natural' ? true : false,      
        hd: hd,
        samples: samples
      });

      if(response.data.error?.code === "rate_limit_exceeded") {
        throw new Error('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.');
      }

      let base64Images = response.data.data.map(item => item?.b64_json)
        .filter(item => item !== undefined);

      console.log(`Displaying returned images for: ${userImagePromptText}`);          
      return base64Images.map(base64Image => `data:image/jpeg;base64,${base64Image}`);

    } catch (error) {
      console.error('Error processing image prompt:', error);
      throw error;
    }
  },

  async processImagePromptStableDiffusion(userImagePromptText, size, hd, guidanceScale, samples, inferenceDenoisingSteps, seed) {
    console.log("inference steps", inferenceDenoisingSteps)
    try {
      console.log("sending seed: ", seed);
      return axios.post(`${process.env.VUE_APP_API_URL}/stablediffusion/ImageRequest`, { 
        imagePromptText: userImagePromptText,
        model: 'stable-diffusion',
        size: size,
        style: false,      
        hd: hd,
        guidanceScale: guidanceScale,
        samples: samples,
        inferenceDenoisingSteps: inferenceDenoisingSteps,
        seed: seed || 0
      })
      .then(response => {
        if(response.data.error?.code === "rate_limit_exceeded") {
          alert('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.')
        } else {
          console.log(response)
          return response.data;
        }
        this.isLoading = false;
      })
    } catch (error) {
      console.error('Error processing image prompt:', error);
      throw error;
    }
  }
}