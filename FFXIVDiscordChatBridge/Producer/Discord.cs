using System.Net.Http.Json;
using Discord;
using Discord.WebSocket;
using FFXIVDiscordChatBridge.Extensions;
using FFXIVDiscordChatBridge.Producer.DiscordWebhookRequest;
using FFXIVHelpers.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FFXIVDiscordChatBridge.Producer;

public interface IDiscordClientWrapper
{
    public DiscordSocketClient Client { get; }
    public IMessageChannel? Channel { get; }
}

public class DiscordClientWrapper : IDiscordClientWrapper
{
    private readonly ILogger<DiscordClientWrapper> _logger;
    public DiscordSocketClient Client { get; }
    public IMessageChannel? Channel { get; private set; }

    private readonly string _discordToken;
    private readonly string _discordChannelId;

    public DiscordClientWrapper(ILogger<DiscordClientWrapper> logger, IConfiguration configuration)
    {
        _logger = logger;
        _discordToken = configuration["discordToken"] ?? throw new InvalidOperationException();
        _discordChannelId = configuration["discordChannelID"] ?? throw new InvalidOperationException();

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers | GatewayIntents.GuildPresences,
            LogLevel = LogSeverity.Verbose
        });
        Client.Log += (message) =>
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem - Required to redirect logs to NLog
            _logger.Log(message.Severity.ToNLogSeverity(), message.Message);
            return Task.CompletedTask;
        };
        Initialize().Wait();
    }

    private async Task Initialize()
    {
        _logger.LogInformation("Starting Discord client..");

        await Client.LoginAsync(TokenType.Bot, _discordToken);
        _logger.LogInformation("Logged in to Discord");
        await Client.StartAsync();
        _logger.LogInformation("Started Discord client; listening for messages");
        await Task.Run(async () =>
        {
            var ready = false;
            Client.Ready += () =>
            {
                _logger.LogInformation("Discord client is ready");
                ready = true;
                return Task.CompletedTask;
            };
            while (!ready)
            {
                await Task.Delay(100);
            }
        });
        
        Channel = Client.GetChannel(ulong.Parse(_discordChannelId)) as IMessageChannel ?? throw new InvalidOperationException();
    }
}

public class Discord : IDiscord
{
    public const string XIVAPIClientString = "XIVAPI";

    private readonly ILogger<Discord> _logger;
    private readonly HttpClient _xivapiClient;
    private readonly Dictionary<Character, string> _avatarCache = new();
    private readonly string _discordWebhookURL;

    public Discord(ILogger<Discord> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _xivapiClient = httpClientFactory.CreateClient(XIVAPIClientString);
        _discordWebhookURL = configuration["discordWebhookURL"] ?? throw new InvalidOperationException();
    }
    
    public async Task Send(Character sender, string? discordMappedName, string message)
    {
        if (!_avatarCache.ContainsKey(sender))
        {
            var httpResponse = await _xivapiClient.GetAsync($"/character/search?name={sender.CharacterName}&server={sender.WorldName}");
            var responseContent = await httpResponse.Content.ReadFromJsonAsync<XIVAPIResponse.RootObject>();
            if (responseContent != null && responseContent.Results.Any())
            {
                _avatarCache.Add(sender, responseContent.Results[0].Avatar);
            }
        }
        _logger.LogInformation("Sending message to Discord: {Content}", message);
        var displayName = sender.CharacterName;
        if (!string.IsNullOrEmpty(discordMappedName))
        {
            displayName = discordMappedName;
        }
        
        var webhookMessage = new RootObject(message, displayName, _avatarCache[sender]);
        var responseMessage = await _xivapiClient.PostAsJsonAsync(_discordWebhookURL, webhookMessage);
        _logger.LogInformation("Discord returned {StatusCode}: {Content}", responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
    }
}