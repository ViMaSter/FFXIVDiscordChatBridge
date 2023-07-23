﻿using System.Collections.Concurrent;
using System.Diagnostics;
using FFXIVByteParser;
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
    private FFXIVByteHandler? _handler;
    
    public event IFFXIVConsumer.OnNewChatMessageDelegate OnNewChatMessage;

    public FFXIV(ILogger<FFXIV> logger, ILogger<FFXIVByteHandler> byteHandlerLogger, IConfiguration configuration, Producer.IDiscordProducer discordProducer)
    {
        _logger = logger;
        _byteHandlerLogger = byteHandlerLogger;
        _channelCode = configuration["ffxivChannelCode"] ?? throw new Exception("ffxivChannelCode not found");
        OnNewChatMessage += async (message) =>
        {
            await discordProducer.Send(message);
        };
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
            Thread.Sleep(250);
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
        
        if (getCurrentPlayer.Entity == null)
        {
            throw new Exception("Can't read current player");
        }

        _handler = new FFXIVByteHandler(_byteHandlerLogger, _channelCode,getCurrentPlayer.Entity.Name, "Zalera", FFXIVByteHandler.CharacterNameDisplay.WITHOUT_WORLD);

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

            _logger.LogInformation("New chat line: {DiscordMessage}", discordMessage);
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