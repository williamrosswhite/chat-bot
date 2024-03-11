<template>
  <div>
    <div v-for="(message, index) in history" :key="index">
      <p :class="message.role">{{ message.content }}</p>
    </div>
  </div>
  <div>
    <textarea 
      v-model="userPromptText" 
      @keydown.enter="processPrompt" 
      @keydown.enter.prevent 
      placeholder="Enter something...">
    </textarea>
  </div>
  <div>
    <button @click="processPrompt">Process Prompt</button>
  </div>
</template>
  
  <script>
  import axios from 'axios';
  import { toRaw } from 'vue';
  
  export default {
    data() {
      return {
        promptResponse: '',
        history: []
      }
    },
    methods: {
      processPrompt() {
        this.history.push({ role: 'user', content: this.userPromptText });
        let historyArray = toRaw(this.history);
        axios.post(`${process.env.VUE_APP_API_URL}/api/ChatRequest`, { 
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

        })
        .catch(error => console.error(error));
      }
    }
  }
  </script>