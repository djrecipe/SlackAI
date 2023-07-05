using SlackAI;
using SlackNet;
using SlackNet.WebApi;
using SlackNet.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
serviceCollection.AddSlackNet(c => c
    // Configure the tokens used to authenticate with Slack
    .UseApiToken("") // This gets used by the API client
    .UseAppLevelToken(
        "") // This gets used by the socket mode client

    .RegisterSlashCommandHandler<SlashCommandHandler>("/ai")
);
var services = serviceCollection.BuildServiceProvider();

var client = services.SlackServices().GetSocketModeClient();
await client.Connect();

Console.WriteLine("Connected. Press any key to exit...");
await Task.Run(Console.ReadKey);