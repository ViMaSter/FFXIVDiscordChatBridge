using Microsoft.Extensions.Logging;

namespace FFXIVHelpers.Extensions;

public static class DiscordLogSeverityExtensions
{
    public static LogLevel ToNLogSeverity(this Discord.LogSeverity discordLogSeverity)
    {
        return discordLogSeverity switch {
            Discord.LogSeverity.Critical => LogLevel.Critical,
            Discord.LogSeverity.Error => LogLevel.Error,
            Discord.LogSeverity.Warning => LogLevel.Warning,
            Discord.LogSeverity.Info => LogLevel.Information,
            Discord.LogSeverity.Verbose => LogLevel.Trace,
            Discord.LogSeverity.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(discordLogSeverity), discordLogSeverity, null)
        };
    }
}