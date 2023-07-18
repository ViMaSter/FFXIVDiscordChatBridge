using Discord.WebSocket;
using FFXIVDiscordChatBridge.Producer;
using NLog;

namespace FFXIVDiscordChatBridge.Consumer;

public class Discord
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly Producer.FFXIV _ffxivProducer;
    private readonly DiscordClientWrapper _discordWrapper;

    public Discord(DiscordClientWrapper discordWrapper, Producer.FFXIV ffxivProducer)
    {
        _discordWrapper = discordWrapper;
        _ffxivProducer = ffxivProducer;
    }

    public Task Start()
    {
        _discordWrapper.Client.MessageReceived += ClientOnMessageReceived;
        return Task.Delay(-1);
    }

    private Task ClientOnMessageReceived(SocketMessage socketMessage)
    {
        var message = socketMessage.Content;
        _logger.Info($"Received message from Discord: {message}");
        _ffxivProducer.Send(message).Wait();
        return Task.CompletedTask;
    }
}