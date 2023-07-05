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

namespace SlackAI;

/// <summary>
/// A slash command handler that just echos back whatever you sent it 
/// </summary>
class SlashCommandHandler : ISlashCommandHandler
{
    private ChatSession session;
    private readonly ISlackApiClient _slack;
    public SlashCommandHandler(ISlackApiClient slack)
    {
        _slack = slack;
        var ex = new InteractiveExecutor(
            new LLama.LLamaModel(new ModelParams("C:\\Downloads\\wizardLM-7B.ggmlv3.q4_0.bin")));
        session = new ChatSession(ex);
    }
    public async Task<SlashCommandResponse> Handle(SlashCommand command)
    {
        Console.WriteLine($"{command.UserName} used the {"/ai"} slash command in the {command.ChannelName} channel");
        Task.Run(() => AskAI(command.Text, command.ChannelName));
        return new SlashCommandResponse
        {
            Message = new Message
            {
                Text = "Please wait a moment..."
            }
        };
    }

    public void AskAI(string prompt, string channel)
    {
        Console.WriteLine($"> {prompt}");
        string result = "";
        var response = session.Chat(prompt,
            new InferenceParams() {Temperature = 0.6f, MaxTokens = 500, FrequencyPenalty = 1.0f, PenalizeNL = false, AntiPrompts = new List<string> { "User:" } });
        foreach (var item in response)
        {
            Console.Write(item);
            result += $"{item}";
        }
        result = result.Remove(result.Length - 5, 5);
        _slack.Chat.PostMessage(new Message()
            {
                Channel = channel, 
                Text = $">{prompt}\n{result}"

            }
        );
    }
}