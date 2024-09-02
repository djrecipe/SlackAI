# Thank you
- .NET Slack API: https://github.com/soxtoby/SlackNet
- .NET LLM Wrappings: https://github.com/SciSharp/LLamaSharp
- LLM instructions: https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html

# SlackAI
Slack LLM app integration
- Socket connection so you don't need a legitimate URL (you can host it on-premises)
- Uses `/ai` slash command for asking questions
- Uses `/gen` slash command for generating images

# Set-up
- Set-up a Slack App and get API tokens: https://api.slack.com/authentication/basics
- For DALL-E/Open AI support, you'll need to [set-up a resource in Azure](https://learn.microsoft.com/en-us/azure/ai-services/openai/dall-e-quickstart?tabs=dalle3%2Ccommand-line&pivots=programming-language-studio)
- Enable socket connection and slash commands for your slack app
- Download some LLM files
- Put your LLM path and API keys into your program and have fun
