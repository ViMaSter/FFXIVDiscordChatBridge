using NLog;

namespace FFXIVDiscordChatBridge;

public class WithLogger
{
    protected Logger _logger;

    protected WithLogger()
    {
        _logger = LogManager.GetCurrentClassLogger();
    }
}