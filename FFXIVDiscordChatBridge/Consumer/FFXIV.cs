using System.Collections.Concurrent;
using System.Diagnostics;
using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Enums;
using Sharlayan.Models;

namespace FFXIVDiscordChatBridge.Consumer;

public class FFXIV : WithLogger, IDisposable
{
    private MemoryHandler? _memoryHandler;
    private int _previousArrayIndex;
    private int _previousOffset;
    private int _currentChatLine = -1;
    private Task? _scan;

    public FFXIV(Action<string> onNewChatMessage)
    {
        OnNewChatMessage = onNewChatMessage;
    }

    private Action<string> OnNewChatMessage { get; }

    public void Start()
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

        _logger.Debug(@"Scanning memory..");
        Thread.Sleep(10000);
        _logger.Debug($"Locations: {_memoryHandler.Scanner.Locations.Count}");
        foreach (var location in _memoryHandler.Scanner.Locations)
        {
            _logger.Debug($"Found {location.Key}. Location: {location.Value.GetAddress().ToInt64():X}");
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

        _logger.Info($"Player name: {getCurrentPlayer.Entity.Name}");

        _logger.Info($"Chat in game and it should appear here:");

        _scan = Task.Run(ScanForNewMessages);
    }

    private Task ScanForNewMessages()
    {
        var currentChatLine = 0;
        while (true)
        {
            if (_currentChatLine == currentChatLine)
            {
                continue;
            }

            var chatLog = ReadChatLog();

            if (chatLog == null || chatLog.IsEmpty)
            {
                continue;
            }

            var line = chatLog.LastOrDefault()?.Line;
            if (line != null)
            {
                _logger.Info("New chat line: {line}", line);
                OnNewChatMessage(line);
            }

            currentChatLine++;
            _currentChatLine = currentChatLine;
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
        _scan?.Dispose();
    }
}