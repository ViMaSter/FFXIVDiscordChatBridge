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
        if (socketMessage.Author.Id == _discordWrapper.Client.CurrentUser.Id)
        {
            return Task.CompletedTask;
        }
        
        if (socketMessage.Channel.Id != _discordWrapper.Channel!.Id)
        {
            return Task.CompletedTask;
        }

        if (socketMessage.Author is not SocketGuildUser guildUser)
        {
            return Task.CompletedTask;
        }

        var userDisplayName = guildUser.DisplayName;

        var formattedMessage = $"[{userDisplayName}]: {socketMessage.Content}";

        _logger.Info($"Received message from Discord: {formattedMessage}");
        _ffxivProducer.Send(formattedMessage).Wait();
        return Task.CompletedTask;

    }
}