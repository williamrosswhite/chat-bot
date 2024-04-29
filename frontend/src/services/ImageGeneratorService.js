import axios from 'axios';

export async function processImagePromptOpenAi(imageDataDTO) {
  try {
    return axios.post(`${process.env.VUE_APP_API_URL}/openapi/ImageRequest`, imageDataDTO.toApiFormat())
    .then(response => {
      return response.data;
    })
  } catch (error) {
    console.error('Error processing image prompt:', error);
    throw error;
  }
}

export async function processImagePromptStableDiffusion(imageDataDTO) {
  try {
    return axios.post(`${process.env.VUE_APP_API_URL}/stablediffusion/ImageRequest`, imageDataDTO.toApiFormat())
    .then(response => {
        return response.data;
    })
  } catch (error) {
    console.error('Error processing image prompt:', error);
    throw error;
  }
}