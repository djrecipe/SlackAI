using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace IronAI
{
    public class TextToImage
    {
        private readonly ITextToImageService service;
        private readonly KernelFunction function;
        public TextToImage(string apiKey, string azureEndpoint)
        {
            // Create a kernel builder
            var builder = Kernel.CreateBuilder();

            // Add OpenAI services to the kernel
            builder.AddAzureOpenAITextToImage("Dalle3", azureEndpoint, apiKey);
            builder.AddAzureOpenAIChatCompletion("oaichat", azureEndpoint, apiKey);

            // Build the kernel
            var kernel = builder.Build();

            // Get AI service instance used to generate images
            service = kernel.GetRequiredService<ITextToImageService>();

            var prompt = @"
You are an AI which generates images for casual chat channels at a company called 'Iron Software'.
This company resides in Thailand but most employees are English speakers. Please keep responses considerate and accurate to the user's request, yet also
try to make them funny as well.

Think about an object, image, or illustration which represents the following: {{$input}}.";

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 256,
                Temperature = 1
            };

            function = kernel.CreateFunctionFromPrompt(prompt, executionSettings);
        }

        public async Task<string> Generate(string Prompt)
        {
            return await service.GenerateImageAsync(Prompt, 1024, 1024);
            //using (WebClient myWebClient = new WebClient())
            //{
            //    byte[] data = myWebClient.DownloadData(imageUrl);
            //    File.WriteAllBytes("output.jpg", data);
            //    return data;
            //}
        }
    }
}
