using Discord;
using Discord.WebSocket;
using FFXIVDiscordChatBridge.Extensions;
using NLog;

namespace FFXIVDiscordChatBridge.Producer;

class DiscordClientWrapper
{
    public readonly DiscordSocketClient Client;

    private readonly string _discordToken;
    private readonly string _discordChannelId;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public IMessageChannel Channel;

    public DiscordClientWrapper(string discordToken, string discordChannelId)
    {
        
        _discordToken = discordToken;
        _discordChannelId = discordChannelId;

        Client = new DiscordSocketClient();
        Client.Log += (message) =>
        {
            _logger.Log(message.Severity.ToNLogSeverity(), message.Message);
            return Task.CompletedTask;
        };
    }
        
    public async Task Initialize()
    {
        _logger.Info("Starting Discord client..");

        await Client.LoginAsync(TokenType.Bot, _discordToken);
        _logger.Info("Logged in to Discord");
        await Client.StartAsync();
        _logger.Info("Started Discord client; listening for messages");
        // wait for client to be ready
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

internal class Discord
{
    private readonly IMessageChannel _channel;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Discord(DiscordClientWrapper wrapper)
    {
        _channel = wrapper.Channel;
    }
    
    public async Task Send(string content)
    {
        _logger.Info($"Sending message to Discord: {content}");
        var a = await _channel.SendMessageAsync(content);
        _logger.Info($"Send message to Discord: {content}");
    }
}