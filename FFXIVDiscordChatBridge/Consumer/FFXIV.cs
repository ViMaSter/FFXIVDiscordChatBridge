using System.Collections.Concurrent;
using System.Diagnostics;
using FFXIVByteParser;
using FFXIVByteParser.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Enums;
using Sharlayan.Models;

namespace FFXIVDiscordChatBridge.Consumer;

public class FFXIV : IDisposable, IFFXIVConsumer
{
    private readonly string _channelCode;
    private MemoryHandler? _memoryHandler;
    private int _previousArrayIndex;
    private int _previousOffset;
    private readonly ILogger<FFXIV> _logger;
    private readonly ILogger<FFXIVByteHandler> _byteHandlerLogger;
    private readonly UsernameMapping _usernameMapping;
    private readonly Producer.IFFXIV _ffxivProducer;
    private FFXIVByteHandler? _handler;
    private readonly string _worldName;
    public Character CurrentCharacter => _handler?.CurrentCharacter ?? throw new Exception("FFXIVByteHandler not yet initialized");
    
    private delegate Task OnNewChatMessageDelegate(FromFFXIV message);

    private OnNewChatMessageDelegate OnNewChatMessage { get; }

    public FFXIV(ILogger<FFXIV> logger, ILogger<FFXIVByteHandler> byteHandlerLogger, IConfiguration configuration, Producer.IDiscord discordProducer, UsernameMapping usernameMapping, Producer.IFFXIV ffxivProducer)
    {
        _logger = logger;
        _byteHandlerLogger = byteHandlerLogger;
        _usernameMapping = usernameMapping;
        _ffxivProducer = ffxivProducer;
        _channelCode = configuration["ffxivChannelCode"] ?? throw new Exception("ffxivChannelCode not found");
        _worldName = configuration["ffxivWorldName"] ?? throw new Exception("ffxivWorldName not found");
        OnNewChatMessage = async (message) =>
        {
            switch (message)
            {
                case FromMonitoredChannel:
                    await discordProducer.Send($"<{_usernameMapping.GetMappingFromFFXIVUsername(message.Character)}> {message.Message}");
                    break;
                case FromTellMessage:
                    HandleAuthorizationMessage(message);
                    break;
            }
        };
    }

    private void HandleAuthorizationMessage(FromFFXIV message)
    {
        var discordUsername = message.Message.Split(" ")[0];
        if (_usernameMapping.ReceiveFromFFXIV(message.Character, discordUsername, out var response))
        {
            _ffxivProducer.Send($"{message.Character.Format(CharacterNameDisplay.WITHOUT_WORLD)} has linked their account to @{discordUsername}");
        }
    }


    public Task Start()
    {
        var processes = Process.GetProcessesByName("ffxiv_dx11");
        if (processes.Length <= 0)
        {
            processes = Process.GetProcessesByName("ffxiv");
            if (processes.Length <= 0)
            {
                throw new Exception("FFXIV not found");
            }
        }

        _memoryHandler = SharlayanMemoryManager.Instance.AddHandler(new SharlayanConfiguration
        {
            ProcessModel = new ProcessModel
            {
                Process = processes[0]
            },
            GameLanguage = GameLanguage.English,
            GameRegion = GameRegion.Global,
            PatchVersion = "latest",
            UseLocalCache = false
        });

        _logger.LogDebug(@"Scanning memory..");
        var startedAt = DateTime.Now;
        while (!(_memoryHandler.Scanner.Locations.ContainsKey("CHATLOG") && _memoryHandler.Scanner.Locations.ContainsKey("PLAYERINFO")))
        {
            Thread.Sleep(500);
            if (DateTime.Now - startedAt > TimeSpan.FromSeconds(30))
            {
                throw new Exception("Memory scanning failed");
            }
        }

        _logger.LogDebug("Locations: {LocationsCount}", _memoryHandler.Scanner.Locations.Count);
        foreach (var location in _memoryHandler.Scanner.Locations)
        {
            _logger.LogDebug("Found {LocationKey}. Location: {Int64}", location.Key, location.Value.GetAddress().ToInt64());
        }

        if (_memoryHandler.Reader == null)
        {
            throw new Exception("Can't read memory");
        }
        if (!_memoryHandler.Reader.CanGetChatLog())
        {
            throw new Exception("Can't read chatlog");
        }

        var getCurrentPlayer = _memoryHandler.Reader.GetCurrentPlayer();
        
        if (string.IsNullOrEmpty(getCurrentPlayer.Entity.Name))
        {
            throw new Exception("Can't read current player");
        }

        _handler = new FFXIVByteHandler(_byteHandlerLogger, _channelCode,getCurrentPlayer.Entity.Name, _worldName);
        _usernameMapping.SetHostingCharacter(new Character(getCurrentPlayer.Entity.Name, _worldName));

        _logger.LogInformation("Player name: {PlayerName}", getCurrentPlayer.Entity.Name);

        _logger.LogInformation($"Chat in game and it should appear here:");

        return ScanForNewMessages();
    }

    // ReSharper disable once FunctionNeverReturns - this is intentional as it loops over messages
    private async Task ScanForNewMessages()
    {
        while (true)
        {
            var chatLog = ReadChatLog();

            if (chatLog == null || chatLog.IsEmpty)
            {
                continue;
            }
            
            var chatLogItem = chatLog.LastOrDefault();
            if (chatLogItem == null)
            {
                continue;
            }

            Debug.Assert(_handler != null, nameof(_handler) + " != null");
            var couldParseMessage = _handler.TryFFXIVToDiscordFriendly(chatLogItem, out var discordMessage);
            if (!couldParseMessage)
            {
                continue;
            }

            if (discordMessage == null)
            {
                continue;
            }

            _logger.LogInformation("New chat line {Type}: {DiscordMessage}", discordMessage.GetType().Name, discordMessage.Message);
            await OnNewChatMessage(discordMessage);
        }
    }
 
    private ConcurrentQueue<ChatLogItem>? ReadChatLog()
    {
        var readResult = _memoryHandler?.Reader.GetChatLog(_previousArrayIndex, _previousOffset);
        var chatLogEntries = readResult?.ChatLogItems;

        if (readResult == null)
        {
            return chatLogEntries;
        }

        _previousArrayIndex = readResult.PreviousArrayIndex;
        _previousOffset = readResult.PreviousOffset;

        return chatLogEntries;
    }

    public void Dispose()
    {
        _memoryHandler?.Dispose();
    }
}