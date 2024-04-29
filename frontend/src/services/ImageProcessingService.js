import { processImagePromptOpenAi, processImagePromptStableDiffusion } from '@/services/ImageGeneratorService';

export async function processImagePrompt(imageDataDTO) {
  if(imageDataDTO.model === 'dall-e-3' || imageDataDTO.model === 'dall-e-2') {
    return await processImagePromptOpenAi(imageDataDTO);
  } else if (imageDataDTO.model === 'stable-diffusion') {
    return await processImagePromptStableDiffusion(imageDataDTO);
  }
}