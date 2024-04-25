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
        history: []
      }
    },
    methods: {
      processPrompt() {
        this.isLoading = true;
        this.history.push({ role: 'user', content: this.userPromptText });
        let historyArray = toRaw(this.history);
        console.log('historyArray: ', historyArray);
        axios.post(`${process.env.VUE_APP_API_URL}/openapi/ChatRequest`, { 
          messages: historyArray,
        })
        .then(response => {
          console.log('raw response: ', response)
          if(response.data.error?.code === "rate_limit_exceeded") {
            alert('Whoops!  Too eager dude!\nRequests per minute limit exceeded, give it a second to cool down.')
          } else {
            let extractedResponse = response.data.choices[0].message;
            console.log('extracted response: ', extractedResponse);
            this.history.push({ role: extractedResponse.role, content: extractedResponse.content });
            this.promptResponse = extractedResponse.content;
            this.userPromptText = '';
          }
          this.isLoading = false;
        })
        .catch(error => console.error(error));
      }
    },
    computed: {
      isButtonDisabled() {
        return this.isLoading || this.userPromptText.length < 2;
      }
    },
    components: {
      BButton,
      BFormTextarea
    }    
  }
  </script>