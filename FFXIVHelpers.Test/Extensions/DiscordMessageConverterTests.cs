using Discord;
using FFXIVHelpers.Extensions;
using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FFXIVHelpers.Test.Extensions;

public class DiscordMessageConverterTests
{
    private DiscordMessageConverter _discordMessageConverter = null!;
    private const string USER_DISPLAY_NAME = "displayName";
    private const string MESSAGE_CONTENT = "message content";
    private const string ATTACHMENT_FILENAME = "originalFilename.extension";
    private const string STICKER_NAME = nameof(STICKER_NAME);
    private const ulong STICKER_ID = 123456789;
    

    [SetUp]
    public void Setup()
    {
        var discordEmojiConverter = new DiscordEmojiConverter();
        var usernameMapping = new FFXIVHelpers.UsernameMapping(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        var logger = NullLogger<DiscordMessageConverter>.Instance;
        var discordWrapper = Mock.Of<IDiscordClientWrapper>();
        _discordMessageConverter = new DiscordMessageConverter(discordEmojiConverter, usernameMapping, logger, discordWrapper);
    }
    
    [Test]
    public void CanHandleSimpleMessage()
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(USER_DISPLAY_NAME);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MESSAGE_CONTENT);
        
        var result = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        Assert.AreEqual($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}", result);
    }
    
    [Test]
    public void CanHandleAttachments()
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(USER_DISPLAY_NAME);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        var discordAttachmentMock = new Mock<IAttachment>();
        discordAttachmentMock.SetupGet(attachment => attachment.Filename).Returns(ATTACHMENT_FILENAME);
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>()
        {
            discordAttachmentMock.Object
        });
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MESSAGE_CONTENT);
        
        var result = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        Assert.AreEqual($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}{Environment.NewLine}{USER_DISPLAY_NAME} sent an attachment: '{ATTACHMENT_FILENAME}'", result);
    }
    
    [Test]
    public void CanHandleStickers()
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(USER_DISPLAY_NAME);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        var discordStickerMock = new Mock<IStickerItem>();
        discordStickerMock.SetupGet(sticker => sticker.Name).Returns(STICKER_NAME);
        discordStickerMock.SetupGet(sticker => sticker.Id).Returns(STICKER_ID);
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<IStickerItem>()
        {
            discordStickerMock.Object
        });
        discordMessageMock.SetupGet(message => message.Content).Returns(MESSAGE_CONTENT);
        
        var result = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        Assert.AreEqual($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}{Environment.NewLine}{USER_DISPLAY_NAME} sent a '{STICKER_NAME}' sticker: https://media.discordapp.net/stickers/{STICKER_ID}.webp", result);
    }
}