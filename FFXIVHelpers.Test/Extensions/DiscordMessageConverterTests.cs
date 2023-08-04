using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using FFXIVHelpers.Extensions;
using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FFXIVHelpers.Test.Extensions;

public class DiscordMessageConverterTests
{
    private DiscordMessageConverter _discordMessageConverter;

    [SetUp]
    public void Setup()
    {
        var discordEmojiConverter = new DiscordEmojiConverter();
        var usernameMapping = new FFXIVHelpers.UsernameMapping(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        var logger = NullLogger<DiscordMessageConverter>.Instance;
        var discordWrapper = Moq.Mock.Of<IDiscordClientWrapper>();
        _discordMessageConverter = new DiscordMessageConverter(discordEmojiConverter, usernameMapping, logger, discordWrapper);
    }
    
    [Test]
    public void CanHandleSimpleMessage()
    {
        var MESSAGE = new Mock<IMessage>();
        var USER = new Mock<IGuildUser>();
        USER.SetupGet(user => user.DisplayName).Returns("username");
        // make author SocketGuildUser
        MESSAGE.SetupGet(message => message.Author).Returns(USER.Object);
        MESSAGE.SetupGet(message => message.Tags).Returns(new List<ITag>());
        MESSAGE.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        MESSAGE.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        MESSAGE.SetupGet(message => message.Content).Returns("message content");
        
        var result = _discordMessageConverter.ToFFXIVCompatible(MESSAGE.Object, DiscordMessageConverter.EventType.MessageSent);
        Assert.AreEqual("[username]: message content", result);
    }
}