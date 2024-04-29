export class ImageDataDTO {
  constructor(data) {
    this.imageUrl = data.imageUrl;
    this.imagePromptText = data.imagePromptText;
    this.model = data.model;
    this.guidanceScale = data.guidanceScale;
    this.hd = data.hd;
    this.inferenceDenoisingSteps = data.inferenceDenoisingSteps;
    this.samples = data.samples;
    this.seed = data.seed;
    this.size = data.size;
    this.style = data.style;
  }

  toApiFormat() {
    return {
      imagePromptText: this.imagePromptText,
      model: this.model,
      size: String(this.size),
      style: this.style === 'natural' ? true : false,
      hd: this.hd,
      guidanceScale: this.guidanceScale,
      samples: this.samples,
      inferenceDenoisingSteps: this.inferenceDenoisingSteps,
      seed: this.seed ? this.seed.toString() : '0'
    };
  }
}

