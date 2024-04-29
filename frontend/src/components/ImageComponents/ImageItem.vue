<template>
  <div class="card">
    <img :src="image.imageUrl" class="image" />
    <div>
      <p class="bubble">{{ image.imagePromptText }}</p>
      <div class="model-info-container">
        <div class="small model-info small-bubble" v-if="image.model != null && image.model != ''">Model: {{ image.model }}</div>
        <div class="small model-info small-bubble" v-if="image.seed != null && image.seed != 0">Seed: {{ image.seed }}</div>
        <div class="small model-info small-bubble" v-if="image.size != null && image.size != ''">Size: {{ image.size }}</div>
        <div class="small model-info small-bubble" v-if="image.hd != false">Hd</div>
        <div class="small model-info small-bubble" v-if="image.style != null && image.style != ''">Style: {{ image.style }}</div>
        <div class="small model-info small-bubble" v-if="image.inferenceDenoisingSteps != null">Denoising Steps: {{ image.inferenceDenoisingSteps }}</div>
        <div class="small model-info small-bubble" v-if="image.guidanceScale != null">Guidance Scale: {{ image.guidanceScale }}</div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    image: {
      type: Object,
      required: true
    }
  }
}
</script>

<style scoped>
.card {
  box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.75);
  margin: 10px;
  max-height: 75vh;
  max-width: 75vh;
  width: auto;
  display: flex;
  flex-direction: column;
  padding: 3px;
}

.image {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.info {
  padding: 0px;
  margin: 0px;
}

.small {
  margin: 0px;
  font-size: 12px;
  padding: 0px;
  padding-bottom: 5px;
}

.bubble {
  padding: 3px;
  margin: 5px;
  background-color: rgb(165, 211, 165);
  border-radius: 15px;
  box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1);
  border: 1px solid #b9b9b9;
}

.small-bubble {
  display: inline-block;
  padding: 3px;
  margin: 5px;
  background-color: rgb(206, 206, 206);
  border-radius: 15px;
  box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1);
  border: 1px solid #b9b9b9;
}

.model-info-container {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-start;
  justify-content: center;
}

.model-info {
  white-space: nowrap;
}
</style>