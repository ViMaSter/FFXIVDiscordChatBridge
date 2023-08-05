using Discord;
using FFXIVHelpers.Extensions;
using Microsoft.Extensions.Logging;

namespace FFXIVHelpers.Test.Extensions;

public class DiscordLogSeverityExtensionsTests
{
    [Test]
    public void ToNLogSeverity_Critical_ReturnsCritical()
    {
        const LogSeverity discordLogSeverity = LogSeverity.Critical;
        var result = discordLogSeverity.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Critical, result);
    }
    
    [Test]
    public void ToNLogSeverity_Error_ReturnsError()
    {
        const LogSeverity discordLogSeverity = LogSeverity.Error;
        var result = discordLogSeverity.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Error, result);
    }
    
    [Test]
    public void ToNLogSeverity_Warning_ReturnsWarning()
    {
        const LogSeverity discordLogSeverity = LogSeverity.Warning;
        var result = discordLogSeverity.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Warning, result);
    }
    
    [Test]
    public void ToNLogSeverity_Info_ReturnsInformation()
    {
        const LogSeverity discordLogSeverity = LogSeverity.Info;
        var result = discordLogSeverity.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Information, result);
    }
    
    [Test]
    public void ToNLogSeverity_Verbose_ReturnsTrace()
    {
        const LogSeverity discordLogSeverity = LogSeverity.Verbose;
        var result = discordLogSeverity.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Trace, result);
    }
    
    [Test]
    public void ToNLogSeverity_Debug_ReturnsDebug()
    {
        const LogSeverity discordLogSeverity = LogSeverity.Debug;
        var result = discordLogSeverity.ToNLogSeverity();
        Assert.AreEqual(LogLevel.Debug, result);
    }
    
    [Test]
    public void ToNLogSeverity_Unknown_ThrowsArgumentOutOfRangeException()
    {
        const LogSeverity discordLogSeverity = (LogSeverity) 99;
        
        var result = Assert.Throws<ArgumentOutOfRangeException>(() => discordLogSeverity.ToNLogSeverity())!;
        Assert.Multiple(() =>
        {
            Assert.That(result.ParamName, Is.EqualTo("discordLogSeverity"));
            Assert.That(result.ActualValue, Is.EqualTo(discordLogSeverity));
        });
    }
}