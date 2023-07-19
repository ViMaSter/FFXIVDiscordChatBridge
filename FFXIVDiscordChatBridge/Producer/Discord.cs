using Discord;
using Discord.WebSocket;
using FFXIVDiscordChatBridge.Extensions;
using Microsoft.Extensions.Configuration;
using NLog;

namespace FFXIVDiscordChatBridge.Producer;

public class DiscordClientWrapper
{
    public readonly DiscordSocketClient Client;

    private readonly string _discordToken;
    private readonly string _discordChannelId;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public IMessageChannel? Channel;

    public DiscordClientWrapper(IConfiguration configuration)
    {
        _discordToken = configuration["discordToken"] ?? throw new InvalidOperationException();
        _discordChannelId = configuration["discordChannelID"] ?? throw new InvalidOperationException();

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages,
            LogLevel = LogSeverity.Verbose
        });
        Client.Log += (message) =>
        {
            _logger.Log(message.Severity.ToNLogSeverity(), message.Message);
            return Task.CompletedTask;
        };
        Initialize().Wait();
    }

    private async Task Initialize()
    {
        _logger.Info("Starting Discord client..");

        await Client.LoginAsync(TokenType.Bot, _discordToken);
        _logger.Info("Logged in to Discord");
        await Client.StartAsync();
        _logger.Info("Started Discord client; listening for messages");
        await Task.Run(async () =>
        {
            var ready = false;
            Client.Ready += () =>
            {
                _logger.Info("Discord client is ready");
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

public class Discord
{
    private readonly IMessageChannel _channel;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Discord(DiscordClientWrapper wrapper)
    {
        _channel = wrapper.Channel!;
    }
    
    public async Task Send(string content)
    {
        _logger.Info($"Sending message to Discord: {content}");
        var a = await _channel.SendMessageAsync(content);
        _logger.Info($"Send message to Discord: {content}");
    }
}