using Discord;
using Discord.WebSocket;
using FFXIVHelpers.Models;
using FFXIVDiscordChatBridge.Producer;
using FFXIVHelpers;
using FFXIVHelpers.Extensions;
using Microsoft.Extensions.Logging;
using Timer = System.Threading.Timer;

namespace FFXIVDiscordChatBridge.Consumer;

public class Discord : IDiscordConsumer
{
    private readonly ILogger<Discord> _logger;
    private readonly IFFXIV _ffxivProducer;
    private readonly UsernameMapping _usernameMapping;
    private readonly DiscordMessageConverter _discordMessageConverter;
    private readonly IDiscordClientWrapper _discordWrapper;
    // ReSharper disable once NotAccessedField.Local - Required to manage lifetime
    private Timer? _displayNameRefreshTimer;

    public Discord(ILogger<Discord> logger, IDiscordClientWrapper discordWrapper, IFFXIV ffxivProducer, UsernameMapping usernameMapping, DiscordMessageConverter discordMessageConverter)
    {
        _logger = logger;
        _discordWrapper = discordWrapper;
        _ffxivProducer = ffxivProducer;
        _usernameMapping = usernameMapping;
        _discordMessageConverter = discordMessageConverter;
    }

    public Task Start()
    {
        _discordWrapper.Client.MessageReceived += (message) => ClientOnMessageReceived(message, DiscordMessageConverter.EventType.MessageSent);
        _discordWrapper.Client.MessageUpdated += (_, socketMessage, _) => ClientOnMessageReceived(socketMessage, DiscordMessageConverter.EventType.MessageEdited);
        
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
            var users = (await _discordWrapper.Channel!.GetUsersAsync().FlattenAsync())
                .Select(user=>user as SocketGuildUser)
                .Where(users=>!string.IsNullOrEmpty(users?.Username))
                .GroupBy(users=>users!.Username)
                .Select(users=>users.First())
                .ToDictionary(user => user!.Username, user => user!.DisplayName);
            _usernameMapping.UpdateDisplayNameMapping(users);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private Task ClientOnMessageReceived(SocketMessage socketMessage, DiscordMessageConverter.EventType eventType)
    {
        if (socketMessage is not SocketUserMessage socketUserMessage)
        {
            _logger.LogInformation("Received non-UserMessage from Discord: {Message}", socketMessage);
            return Task.CompletedTask;
        }
        
        return socketMessage.Channel switch
        {
            SocketDMChannel => HandleUsernameMapping(socketUserMessage),
            SocketTextChannel => HandleChannelMessage(socketUserMessage, eventType),
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
    
    private async Task HandleChannelMessage(SocketUserMessage socketMessage, DiscordMessageConverter.EventType eventType)
    {
        if (socketMessage.Author.Id == _discordWrapper.Client.CurrentUser.Id)
        {
            return;
        }

        if (socketMessage.Channel.Id != _discordWrapper.Channel!.Id)
        {
            return;
        }
        
        var fullMessage = _discordMessageConverter.ToFFXIVCompatible(socketMessage, eventType);

        // send each line to FFXIV
        foreach (var line in fullMessage.Trim().Split(Environment.NewLine))
        {
            if (string.IsNullOrEmpty(line.Trim()))
            {
                continue;
            }

            _ffxivProducer.Send(line).Wait();
        }
    }
}