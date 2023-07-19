namespace FFXIVDiscordChatBridge.Extensions;

static class DiscordLogSeverityExtensions
{
    public static NLog.LogLevel ToNLogSeverity(this Discord.LogSeverity discordLogSeverity)
    {
        return discordLogSeverity switch {
            Discord.LogSeverity.Critical => NLog.LogLevel.Fatal,
            Discord.LogSeverity.Error => NLog.LogLevel.Error,
            Discord.LogSeverity.Warning => NLog.LogLevel.Warn,
            Discord.LogSeverity.Info => NLog.LogLevel.Info,
            Discord.LogSeverity.Verbose => NLog.LogLevel.Trace,
            Discord.LogSeverity.Debug => NLog.LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(discordLogSeverity), discordLogSeverity, null)
        };
    }
}