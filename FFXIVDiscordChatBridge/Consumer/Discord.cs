using Discord;
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
        
        var fullMessage = socketMessage.Content;
        
        foreach (var socketMessageTag in socketMessage.Tags)
        {
            switch (socketMessageTag.Type)
            {
                case TagType.Emoji:
                    if (socketMessageTag.Value is not Emote e)
                    {
                        continue;
                    }

                    fullMessage = fullMessage.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $":{e.Name}:");
                    break;
                case TagType.UserMention:
                    if (socketMessageTag.Value is not SocketGuildUser u)
                    {
                        continue;
                    }

                    fullMessage = fullMessage.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $"@{u.Nickname}");
                    break;
                case TagType.ChannelMention:
                    break;
                case TagType.RoleMention:
                    if (socketMessageTag.Value is not SocketRole r)
                    {
                        continue;
                    }
                    fullMessage = fullMessage.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $"@{r.Name}");
                    break;
                case TagType.EveryoneMention:
                    break;
                
                case TagType.HereMention:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        fullMessage = fullMessage.ReplaceLineEndings();

        var userDisplayName = guildUser.DisplayName;

        _logger.LogInformation("Received message from Discord: {FullMessage}", fullMessage);
        foreach (var line in fullMessage.Split(Environment.NewLine))
        {
            if (string.IsNullOrEmpty(line.Trim()))
            {
                continue;
            }
            
            var formattedMessage = $"[{userDisplayName}]: {line}";

            _ffxivProducer.Send(formattedMessage).Wait();
        }

        foreach (var attachment in socketMessage.Attachments)
        {
            var formattedMessage = $"{userDisplayName} sent an attachment: '{attachment.Filename}'     ";

            _ffxivProducer.Send(formattedMessage).Wait();
        }
        
        foreach (var socketMessageSticker in socketMessage.Stickers)
        {
            var formattedMessage = $"{userDisplayName} sent a '{socketMessageSticker.Name}' sticker: https://media.discordapp.net/stickers/{socketMessageSticker.Id}.webp     ";

            _ffxivProducer.Send(formattedMessage).Wait();
        }

        return Task.CompletedTask;

    }
}