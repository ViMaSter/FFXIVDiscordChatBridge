using Discord;
using Discord.WebSocket;
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
        Assert.That(result, Is.EqualTo($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}"));
    }
    
    [Test]
    public void SkipsEmptyLines()
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(USER_DISPLAY_NAME);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MESSAGE_CONTENT+Environment.NewLine+Environment.NewLine);
        
        var result = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        Assert.That(result, Is.EqualTo($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}"));
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
        Assert.That(result, Is.EqualTo($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}{Environment.NewLine}{USER_DISPLAY_NAME} sent an attachment: '{ATTACHMENT_FILENAME}'"));
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
        Assert.That(result, Is.EqualTo($"[{USER_DISPLAY_NAME}]: {MESSAGE_CONTENT}{Environment.NewLine}{USER_DISPLAY_NAME} sent a '{STICKER_NAME}' sticker: https://media.discordapp.net/stickers/{STICKER_ID}.webp"));
    }
    
    [Test]
    public void 
    CanHandleTags()
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(USER_DISPLAY_NAME);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        
        var eyesEmojiId = Emote.Parse("<:eyes_r:737363893571027115>");
        var discordMessageTagMock = new Mock<ITag>();
        discordMessageTagMock.SetupGet(tag => tag.Type).Returns(TagType.Emoji);
        discordMessageTagMock.SetupGet(tag => tag.Index).Returns(24);
        discordMessageTagMock.SetupGet(tag => tag.Length).Returns(28);
        discordMessageTagMock.SetupGet(tag => tag.Key).Returns(737363893571027115);
        discordMessageTagMock.SetupGet(tag => tag.Value).Returns(eyesEmojiId);
        
        var user = new Mock<IGuildUser>();
        user.SetupGet(r => r.Nickname).Returns("Snasen");
        var discordUserTackMock = new Mock<ITag>();
        discordUserTackMock.SetupGet(tag => tag.Type).Returns(TagType.UserMention);
        discordUserTackMock.SetupGet(tag => tag.Index).Returns(59);
        discordUserTackMock.SetupGet(tag => tag.Length).Returns(20);
        discordUserTackMock.SetupGet(tag => tag.Key).Returns(94583392179322880);
        discordUserTackMock.SetupGet(tag => tag.Value).Returns(user.Object);
        
        var channel = new Mock<IGuildChannel>();
        channel.SetupGet(r => r.Name).Returns("bloop");
        var discordChannelMock = new Mock<ITag>();
        discordChannelMock.SetupGet(tag => tag.Type).Returns(TagType.ChannelMention);
        discordChannelMock.SetupGet(tag => tag.Index).Returns(89);
        discordChannelMock.SetupGet(tag => tag.Length).Returns(22);
        discordChannelMock.SetupGet(tag => tag.Key).Returns(1032389530248032356);
        discordChannelMock.SetupGet(tag => tag.Value).Returns(channel.Object);

        var role = new Mock<IRole>();
        role.SetupGet(r => r.Name).Returns("himebot");
        
        var discordRoleMock = new Mock<ITag>();
        discordRoleMock.SetupGet(tag => tag.Type).Returns(TagType.RoleMention);
        discordRoleMock.SetupGet(tag => tag.Index).Returns(118);
        discordRoleMock.SetupGet(tag => tag.Length).Returns(22);
        discordRoleMock.SetupGet(tag => tag.Key).Returns(366743881838100480);
        discordRoleMock.SetupGet(tag => tag.Value).Returns(role.Object);

        var everyone = new Mock<IRole>();
        everyone.SetupGet(r => r.Name).Returns("@everyone");
        var discordEveryoneMock = new Mock<ITag>();
        discordEveryoneMock.SetupGet(tag => tag.Type).Returns(TagType.EveryoneMention);
        discordEveryoneMock.SetupGet(tag => tag.Index).Returns(151);
        discordEveryoneMock.SetupGet(tag => tag.Length).Returns(9);
        discordEveryoneMock.SetupGet(tag => tag.Key).Returns(0);
        discordEveryoneMock.SetupGet(tag => tag.Value).Returns(everyone.Object);

        var here = new Mock<IRole>();
        here.SetupGet(r => r.Name).Returns("@everyone");
        var discordHereMock = new Mock<ITag>();
        discordHereMock.SetupGet(tag => tag.Type).Returns(TagType.HereMention);
        discordHereMock.SetupGet(tag => tag.Index).Returns(167);
        discordHereMock.SetupGet(tag => tag.Length).Returns(5);
        discordHereMock.SetupGet(tag => tag.Key).Returns(0);
        discordHereMock.SetupGet(tag => tag.Value).Returns(here.Object);
        
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>()
        {
            discordMessageTagMock.Object,
            discordChannelMock.Object,
            discordUserTackMock.Object,
            discordRoleMock.Object,
            discordEveryoneMock.Object,
            discordHereMock.Object
        });
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<IStickerItem>());
        discordMessageMock.SetupGet(message => message.Content).Returns("""
        Emoji: 👀
        Server Emoji: <:eyes_r:737363893571027115>
        User: <@94583392179322880>
        Channel: <#1032389530248032356>
        Role: <@&366743881838100480>
        Everyone: @everyone
        Here: @here
        """.ReplaceLineEndings().Replace("\r\n", "\n"));
        
        var result = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        const string EXPECTED = """
            [displayName]: Emoji: :eyes:
            [displayName]: Server Emoji: :eyes_r:
            [displayName]: User: @Snasen
            [displayName]: Channel: #bloop
            [displayName]: Role: @himebot
            [displayName]: Everyone: @everyone
            [displayName]: Here: @here
            """;
        Assert.That(result, Is.EqualTo(EXPECTED));
    }
}