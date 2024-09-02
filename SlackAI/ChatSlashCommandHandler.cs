using LLama;
using LLama.Common;
using LLama.OldVersion;
using Newtonsoft.Json.Linq;
using SlackNet;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.WebApi;
using LLamaModel = LLama.OldVersion.LLamaModel;
using MessageUpdateResponse = SlackNet.WebApi.MessageUpdateResponse;

namespace SlackAI;

/// <summary>
/// A slash command handler that just echos back whatever you sent it 
/// </summary>
class ChatSlashCommandHandler : ISlashCommandHandler
{
    private ChatSession session;
    private readonly ISlackApiClient _slack;
    public static string ModelPath { get; set; }
    public ChatSlashCommandHandler(ISlackApiClient slack)
    {
        _slack = slack;
        var ex = new InteractiveExecutor(
            new LLama.LLamaModel(new ModelParams(ModelPath)));
        session = new ChatSession(ex);
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
            /*
             * Must include AntiPrompt to prevent the AI from repeating the prompt
             */
            var response = session.Chat(prompt,
                new InferenceParams() { Temperature = 0.6f, MaxTokens = 5000, FrequencyPenalty = 1.0f, PenalizeNL = false, AntiPrompts = new List<string> { "User: " } });
            var message_response = _slack.Chat.PostMessage(new Message()
                {
                    Channel = channel_name,
                    Text = result
                }
            );
            message_response.Wait();
            Task<MessageUpdateResponse> current_task = null;
            foreach (var item in response)
            {
                Console.Write(item);
                result += $"{item}";
                if (current_task == null || current_task.IsCompleted)
                {
                    current_task = _slack.Chat.Update(new MessageUpdate()
                    {
                        ChannelId = channel_id,
                        Ts = message_response.Result.Ts,
                        Text = result
                    });
                }
            }
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