# rwhite83's Chat Bot

This is just a demo project for me to learn things while I'm on the bench at work.  In this project, I'm learning about:
- How to set up and use github copilot:
    - I've found copilot immensely helpful.  Not just in troubleshooting, but in helping me learn by being able to answer questions I have about what I'm working on.  I struggle with trying to read documentation, but being able to just ask about something and then ask follow ups is much more condusive to my learning style.  It's also incredibly good at being just that, a copilot, and I can have a running dialogue with it about what I'm working on, the problems I run into, and how to go about setting up a new thing.  This is going to really improve my development process.
- AI chat functionality in general:
    - I am interested in AI chat generation both in how it can help me as a developer, but also as a writer.  Experimenting with various chat bots, I've found that they can be very fun as choose your own adventure kinds of things, and have come to understand that they can funcion a little like a Star Trek holodeck, if limited to text.
- Vue.js:
    - I haven't worked a lot on the UI for this project yet, just enough to fascilitate showing a persistent chat functionality.
- OpenAI:
    - I wanted to learn about Azure's AI platform, but it is not open to the public yet so had to settle for regular OpenAI.  I've gotten a persistent chat to function through this little app, and it's cool, but once I lock down environment and deployment to Azure, I can really start playing with different ways to configure prompts and responses.
- Setting up and understanding differnt .NET environment configurations:
    - For example, yesterday I set out with the goal of hosting this app on Azure, but have instead spent yesterday and today learning how to configure two different modes for the app to run, one local with back and front ends run independently and talking to each other for local development, and another for hosting somewhere like Azure, where the front end has to be built and the whole thing has to be rolled into one packaged application
- Debugging backend in Visual Studio:
    - I also learned today about the proper way to set up debugging in Visual Studio Code instead of relying on doing it in Visual Studio or just using comment outputs

## Local Development
```
- open two terminals in Visual Studio or however else you like
- navigate to ./chatbot/backend in one of them
- run `dotnet watch run` for hot reload 
    - or to debug, open Visual Studio debugger and select 'Development (http)' configuration and then launch debugger
    - backendw will launch a swagger tab at http://localhost/5111
- in the other terminal/window, navigate to ./chatbot/frontend
- run `npm run serve`
- browser will launch to application at http://localhost:5112/
    - note this is uncertified here, deliberately different from deployed version
- you can also navigate to swagger on the server's port at http://localhost:5111
```

## Deploy to Azure
```
- in ./chatbot/frontend, run npm run build
- this will generate build files and copy them to backend's wwwroot folder
- open Visual Studio Code to backend project, then go to Azure tab
- open OpenAi Chatbot Pay-As-You-Go -> App Services, and right click on rwhite83-openAI-test
- select Deploy to Web App
```

### Next Steps

```
- create an image generation service on my page
- create a login
- implement a database to save user's activity
- after that, I will focus on either learning more Vue in making a more interesting UI interface, or learning more about OpenAI prompt engineering
- whichever I focus on first, the other will come after
```
