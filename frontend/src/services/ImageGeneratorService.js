// ImageGeneratorService.js
import axios from 'axios';

export default {

  async processImagePromptOpenAi(userImagePromptText, model, size, style, hd) {
    try {
      const response = await axios.post(`${process.env.VUE_APP_API_URL}/openapi/ImageRequest`, { 
        imagePromptText: userImagePromptText,
        model: model,
        size: size,
        style: style === 'natural' ? true : false,      
        hd: hd
      });

      if(response.data.error?.code === "rate_limit_exceeded") {
        throw new Error('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.');
      }

      let base64Image = response.data.data[0]?.b64_json;
      if(base64Image != undefined) {
        console.log(`Displaying returned image for: ${userImagePromptText}`);          
        return `data:image/jpeg;base64,${base64Image}`;
      } else {
        throw new Error('Image generation failed, try again!');
      }
    } catch (error) {
      console.error('Error processing image prompt:', error);
      throw error;
    }
  },

  async processImagePromptStableDiffusion(userImagePromptText, size, hd, guidanceScale, panorama) {
    axios.post(`${process.env.VUE_APP_API_URL}/stablediffusion/ImageRequest`, { 
      imagePromptText: userImagePromptText,
      model: 'stable-diffusion',
      size: size,
      style: false,      
      hd: hd,
      guidanceScale: guidanceScale,
      panorama: panorama
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