using Discord.WebSocket;
using FFXIVDiscordChatBridge.Producer;
using Microsoft.Extensions.Logging;

namespace FFXIVDiscordChatBridge.Consumer;

public class Discord : IDiscordConsumer
{
    private readonly ILogger<Discord> _logger;
    private readonly IFFXIVProducer _ffxivProducer;
    private readonly IDiscordClientWrapper _discordWrapper;

    public Discord(ILogger<Discord> logger, IDiscordClientWrapper discordWrapper, IFFXIVProducer ffxivProducer)
    {
        _logger = logger;
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

        _logger.LogInformation($"Received message from Discord: {formattedMessage}");
        _ffxivProducer.Send(formattedMessage).Wait();
        return Task.CompletedTask;

    }
}