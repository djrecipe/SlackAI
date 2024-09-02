using SlackAI;
using SlackNet;
using SlackNet.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Extensions.DependencyInjection;

var settings = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build()
    .Get<AppSettings>();

ChatSlashCommandHandler.ModelPath = settings.ModelPath;
GenSlashCommandHandler.OpenAIKey = settings.OpenAIKey;
GenSlashCommandHandler.OpenAIEndpoint = settings.OpenAIEndpoint;

var serviceCollection = new ServiceCollection();
serviceCollection.AddSlackNet(c => c
    // Configure the tokens used to authenticate with Slack
    .UseApiToken(settings.ApiKey) // This gets used by the API client
    .UseAppLevelToken(settings.ApiKeySockets) // This gets used by the socket mode client

    .RegisterSlashCommandHandler<ChatSlashCommandHandler>("/ai")
    .RegisterSlashCommandHandler<GenSlashCommandHandler>("/gen")
);
var services = serviceCollection.BuildServiceProvider();

var client = services.SlackServices().GetSocketModeClient();
await client.Connect();

Console.WriteLine("Connected. Press any key to exit...");
await Task.Run(Console.ReadKey);