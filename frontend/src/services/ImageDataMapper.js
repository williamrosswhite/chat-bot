import { ImageDataDTO } from '@/models/ImageDataDTO';

export function mapImageData(responseData) {
  return responseData.map(item => new ImageDataDTO(item));
}