using Discord;
using FFXIVHelpers.Extensions;
using Microsoft.Extensions.Logging;

namespace FFXIVHelpers.Test.Extensions;

public class DiscordLogSeverityExtensionsTests
{
    [Test]
    public void ToNLogSeverity_Critical_ReturnsCritical()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = LogSeverity.Critical;
        var result = DISCORD_LOG_SEVERITY.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Critical, result);
    }
    
    [Test]
    public void ToNLogSeverity_Error_ReturnsError()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = LogSeverity.Error;
        var result = DISCORD_LOG_SEVERITY.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Error, result);
    }
    
    [Test]
    public void ToNLogSeverity_Warning_ReturnsWarning()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = LogSeverity.Warning;
        var result = DISCORD_LOG_SEVERITY.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Warning, result);
    }
    
    [Test]
    public void ToNLogSeverity_Info_ReturnsInformation()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = LogSeverity.Info;
        var result = DISCORD_LOG_SEVERITY.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Information, result);
    }
    
    [Test]
    public void ToNLogSeverity_Verbose_ReturnsTrace()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = LogSeverity.Verbose;
        var result = DISCORD_LOG_SEVERITY.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Trace, result);
    }
    
    [Test]
    public void ToNLogSeverity_Debug_ReturnsDebug()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = LogSeverity.Debug;
        var result = DISCORD_LOG_SEVERITY.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Debug, result);
    }
    
    [Test]
    public void ToNLogSeverity_Unknown_ThrowsArgumentOutOfRangeException()
    {
        const LogSeverity DISCORD_LOG_SEVERITY = (LogSeverity) 99;
        
        var result = Assert.Throws<ArgumentOutOfRangeException>(() => DISCORD_LOG_SEVERITY.ToNLogSeverity())!;
        Assert.Multiple(() =>
        {
            Assert.That(result.ParamName, Is.EqualTo("discordLogSeverity"));
            Assert.That(result.ActualValue, Is.EqualTo(DISCORD_LOG_SEVERITY));
        });
    }
}