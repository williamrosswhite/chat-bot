<template>
  <div>
    <div v-for="(message, index) in history" :key="index">
      <p :class="message.role">{{ message.content }}</p>
    </div>
  </div>    
  <p>Enter a text prompt:</p>
  <div class="text-area option-container">
    <b-form-textarea
      v-model="userPromptText" 
      @keydown.enter="processPrompt" 
      @keydown.enter.prevent 
      placeholder="2 character minimum"
      no-auto-shrink
    ></b-form-textarea>
  </div>
  <br/>
  <div>
    <b-button pill variant="success" :disabled="isButtonDisabled" @click="processPrompt" class="process-image-button">Process Prompt</b-button>
  </div>
  <br><br>
  <input type="text" v-model="secretInput" placeholder="">
  <br>
</template>
  
  <script>
  import axios from 'axios';
  import { toRaw } from 'vue';
  import { BButton, BFormTextarea } from 'bootstrap-vue-3'
  
  export default {
    data() {
      return {
        isLoading: false,
        userPromptText: '',
        promptResponse: '',
        history: [],
        secretInput: '',
        secret: process.env.VUE_APP_SECRET,  // Add this line
        isButtonDisabled: false
      }
    },
    methods: {
      processPrompt() {
        this.isLoading = true;
        this.history.push({ role: 'user', content: this.userPromptText });
        let historyArray = toRaw(this.history);
        axios.post(`${process.env.VUE_APP_API_URL}/openapi/ChatRequest`, { 
          messages: historyArray,
        })
        .then(response => {
          let extractedResponse = response.data.choices[0].message;
          this.history.push({ role: extractedResponse.role, content: extractedResponse.content });
          this.promptResponse = extractedResponse.content;
          this.userPromptText = '';
          this.isLoading = false;
        })
        .catch(error => console.error(error));
      },
      goToImageHistory() {
        this.$router.push('/imageHistory');
      }
    },
    watch: {
      secretInput(newVal) {
        if (newVal === this.secret) {
          this.goToImageHistory();
        }
      }
    },
    components: {
      BButton,
      BFormTextarea
    }    
  }
  </script>