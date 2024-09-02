using IronAI;
using LLama;
using LLama.Common;
using LLama.OldVersion;
using Newtonsoft.Json.Linq;
using SlackNet;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.WebApi;
using System;
using SlackNet.Blocks;
using static System.Collections.Specialized.BitVector32;
using LLamaModel = LLama.OldVersion.LLamaModel;
using MessageUpdateResponse = SlackNet.WebApi.MessageUpdateResponse;

namespace SlackAI;

/// <summary>
/// A slash command handler that just echos back whatever you sent it 
/// </summary>
class GenSlashCommandHandler : ISlashCommandHandler
{
    public static string OpenAIKey { get; set; }
    public static string OpenAIEndpoint { get; set; }
    private readonly ISlackApiClient _slack;
    private readonly TextToImage converter = new TextToImage(OpenAIKey, OpenAIEndpoint);
    public GenSlashCommandHandler(ISlackApiClient slack)
    {
        _slack = slack;
    }
    public async Task<SlashCommandResponse> Handle(SlashCommand command)
    {
        Console.WriteLine($"{command.UserName} used the {"/ai"} slash command in the {command.ChannelName} channel");
        Task.Run(() => AskAI(command.Text, command.ChannelName, command.ChannelId));
        return new SlashCommandResponse
        {
            Message = new Message
            {
                Text = "Please wait a moment..."
            }
        };
    }

    public void AskAI(string prompt, string channel_name, string channel_id)
    {
        try
        {
            Console.WriteLine($"> {prompt}");
            string result = $">{prompt}\n";
            var message_response = _slack.Chat.PostMessage(new Message()
                {
                    Channel = channel_name,
                    Text = result
                }
            );
            message_response.Wait();
            /*
             * Must include AntiPrompt to prevent the AI from repeating the prompt
             */
            var img = converter.Generate(prompt).Result;
            ImageBlock block = new ImageBlock()
            {
                ImageUrl = img.ToString(),
                AltText = $"Generating '{prompt}', please wait..."
            };
            Attachment attachment = new Attachment()
            {
            };
            attachment.Blocks.Add(block);
            var update_response = _slack.Chat.Update(new MessageUpdate()
            {
                ChannelId = channel_id,
                Ts = message_response.Result.Ts,
                Text = $"Finished generating '{prompt}'.",
                Attachments = new List<Attachment>() { attachment }
            });
            update_response.Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while using AI: '{e.Message}'");
            //_slack.Chat.PostMessage(new Message()
            //{
            //    Channel = channel_name,
            //    Text = $"Error while using AI: '{e.Message}'"
            //});
        }
    }
}