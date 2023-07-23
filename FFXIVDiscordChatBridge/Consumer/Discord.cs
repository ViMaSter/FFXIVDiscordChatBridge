using Discord;
using Discord.WebSocket;
using FFXIVHelpers.Models;
using FFXIVDiscordChatBridge.Producer;
using FFXIVHelpers;
using Microsoft.Extensions.Logging;
using Timer = System.Threading.Timer;

namespace FFXIVDiscordChatBridge.Consumer;

public class Discord : IDiscordConsumer
{
    private readonly ILogger<Discord> _logger;
    private readonly IFFXIV _ffxivProducer;
    private readonly UsernameMapping _usernameMapping;
    private readonly IDiscordClientWrapper _discordWrapper;
    // ReSharper disable once NotAccessedField.Local - Required to manage lifetime
    private Timer? _displayNameRefreshTimer;

    public Discord(ILogger<Discord> logger, IDiscordClientWrapper discordWrapper, IFFXIV ffxivProducer, UsernameMapping usernameMapping)
    {
        _logger = logger;
        _discordWrapper = discordWrapper;
        _ffxivProducer = ffxivProducer;
        _usernameMapping = usernameMapping;
    }

    public Task Start()
    {
        _discordWrapper.Client.MessageReceived += ClientOnMessageReceived;
        if (_discordWrapper.Client.ConnectionState != ConnectionState.Connected)
        {
            _discordWrapper.Client.Ready += SetupDisplayNameLoop;
        }
        else
        {
            SetupDisplayNameLoop().Wait();
        }

        return Task.Delay(-1);
    }

    private Task SetupDisplayNameLoop()
    {
        _displayNameRefreshTimer = new Timer(async _ =>
        {
            var users = (await _discordWrapper.Channel!.GetUsersAsync().FlattenAsync()).Select(user=>user as SocketGuildUser);
            _usernameMapping.UpdateDisplayNameMapping(users.ToDictionary(user => user!.Username, user => user!.DisplayName));
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private Task ClientOnMessageReceived(SocketMessage socketMessage)
    {
        if (socketMessage is not SocketUserMessage socketUserMessage)
        {
            _logger.LogInformation("Received non-UserMessage from Discord: {Message}", socketMessage);
            return Task.CompletedTask;
        }
        
        return socketMessage.Channel switch
        {
            SocketDMChannel => HandleUsernameMapping(socketUserMessage),
            SocketTextChannel => HandleFFXIVMessage(socketUserMessage),
            _ => Task.CompletedTask
        };
    }

    private Task HandleUsernameMapping(SocketUserMessage socketMessage)
    {
        // skip messages from the bot itself
        if (socketMessage.Author.Id == _discordWrapper.Client.CurrentUser.Id)
        {
            return Task.CompletedTask;
        }
        
        var fromDiscordMessage = socketMessage.Content.Replace("`", "").Trim().Split("@").Select(x => x.Trim()).ToList();
        if (fromDiscordMessage.Count != 2)
        {
            _logger.LogInformation("Received invalid username mapping message via Discord: '{Username}' sent '{Message}'", socketMessage.Author.Username, socketMessage.Content);
            socketMessage.Author.SendMessageAsync($"To verify your character name, send me your character name and world.{Environment.NewLine}For example: If your character `Haurchefant Greystone` is on the `Zalera` server, enter `Haurchefant Greystone@Zalera` below.").Wait();
            return Task.CompletedTask;
        }
        
        _usernameMapping.ReceiveFromDiscord(new Character(fromDiscordMessage.First(), fromDiscordMessage.Last()), socketMessage.Author.Username, out var message);
        socketMessage.Author.SendMessageAsync(message).Wait();
        return Task.CompletedTask;
    }

    private Task HandleFFXIVMessage(SocketUserMessage socketMessage)
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

        var userDisplayName = _usernameMapping.GetMappingFromDiscordUsername(guildUser.Username) ?? guildUser.DisplayName;

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