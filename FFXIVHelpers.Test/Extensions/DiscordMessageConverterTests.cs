using Discord;
using Discord.WebSocket;
using FFXIVHelpers.Extensions;
using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FFXIVHelpers.Test.Extensions;

[Parallelizable(ParallelScope.Fixtures)] // NextFormat is not thread-safe
public class DiscordMessageConverterTests
{
    private static class ReplyAssert
    {
        public static void That(string actual, string expectedWrittenContent, string expectedGeneratedContent, ReplyType replyType)
        {
            Console.WriteLine("----------");
            Console.WriteLine(nameof(actual)+":");
            Console.WriteLine(actual);
            Console.WriteLine("-----");
            Console.WriteLine(nameof(expectedWrittenContent)+":");
            Console.WriteLine(expectedWrittenContent);
            Console.WriteLine("-----");
            Console.WriteLine(nameof(expectedGeneratedContent)+":");
            Console.WriteLine(expectedGeneratedContent);
            Console.WriteLine("----------");

            
            if (replyType == ReplyType.None)
            {
                var expected = $"{string.Join(Environment.NewLine, expectedWrittenContent.ReplaceLineEndings().Split(Environment.NewLine).Select(line => $"[{UserDisplayName}]: {line}"))}{Environment.NewLine}{expectedGeneratedContent}".Trim();
                Assert.That(actual, Is.EqualTo(expected));
                return;
            }
            
            var content = $"{expectedWrittenContent}{Environment.NewLine}{expectedGeneratedContent}".Trim();
            var actualLines = actual.Split(Environment.NewLine);
            var expectedLines = $"{content}".Split(Environment.NewLine);
            Assert.That(actualLines[0], Contains.Substring(UserDisplayName));
            Assert.That(actualLines[0], replyType == ReplyType.User ? Contains.Substring(ReplyUserDisplayName) : Contains.Substring(ReplyBotDisplayName));
            for (var index = 0; index < expectedLines.Length; index++)
            {
                var expectedLine = expectedLines[index];
                var actualLine = string.Concat(actualLines[index+1].Skip(2));
                Assert.That(actualLine, Is.EqualTo(expectedLine));
                Assert.That(actualLine, Does.Not.StartWith("["+UserDisplayName));
            }
            
        }
    }
    
    private DiscordMessageConverter _discordMessageConverter = null!;
    private const string UserDisplayName = "displayName";
    private const string ReplyUserDisplayName = "ReplyUser";
    private const string ReplyBotDisplayName = "ReplyBot";
    private const string ReplyUnsupportedDisplayName = "UnsupportedName";
    private const string MessageContent = "message content";
    private const string AttachmentFilename = "originalFilename.extension";
    private const string StickerName = nameof(StickerName);
    private const ulong StickerId = 123456789;

    public enum ReplyType
    {
        None,
        User,
        Unsupported,
        Bot
    }

    [SetUp]
    public void Setup()
    {
        var discordEmojiConverter = new DiscordEmojiConverter();
        var usernameMapping = new FFXIVHelpers.UsernameMapping(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        var logger = NullLogger<DiscordMessageConverter>.Instance;

        var channel = new Mock<ISocketMessageChannel>();
        var messagesByType = new Dictionary<ReplyType, IMessage>()
        {
            { ReplyType.None, null! },
            { ReplyType.User, Mock.Of<IUserMessage>(message => message.Author == Mock.Of<IGuildUser>(user => user.DisplayName == ReplyUserDisplayName)) },
            { ReplyType.Bot, Mock.Of<IUserMessage>(message => message.Author == Mock.Of<IWebhookUser>(user => user.Username == ReplyBotDisplayName)) },
            { ReplyType.Unsupported, Mock.Of<IUserMessage>(message => message.Author == Mock.Of<IUser>(user => user.Username == ReplyUnsupportedDisplayName)) },
        };

        channel.Setup(c => c.GetMessageAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .Returns<ulong, CacheMode, RequestOptions>((id, mode, options) =>
            {
                var user = new Mock<IUser>();
                user.SetupGet(u => u.Username).Returns(ReplyUserDisplayName);
                return Task.FromResult(messagesByType[(ReplyType)id]);
            });
        var discordWrapper = Mock.Of<IDiscordClientWrapper>(wrapper => wrapper.Channel == channel.Object);
        _discordMessageConverter = new DiscordMessageConverter(discordEmojiConverter, usernameMapping, logger, discordWrapper);
    }
    
    private static void ApplyReplyChanges(ref Mock<IMessage> responseMessage, ReplyType replyType)
    {
        if (replyType == ReplyType.None)
        {
            return;
        }
        
        responseMessage.SetupGet(message => message.Reference).Returns(new MessageReference((ulong)replyType, 987654321, 123456789));
    }
    
    [TestCase(ReplyType.None)]
    [TestCase(ReplyType.User)]
    [TestCase(ReplyType.Bot)]
    public void CanHandleSimpleMessage(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MessageContent);
        ApplyReplyChanges(ref discordMessageMock, replyType);
        
        var actual = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        const string expected = $"{MessageContent}";
        ReplyAssert.That(actual, expected, "", replyType);
    }
    
    [TestCase(ReplyType.None)]
    [TestCase(ReplyType.User)]
    [TestCase(ReplyType.Bot)]
    public void SkipsEmptyLines(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MessageContent+Environment.NewLine+Environment.NewLine);
        ApplyReplyChanges(ref discordMessageMock, replyType);
        
        var actual = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        const string expected = $"{MessageContent}";
        ReplyAssert.That(actual, expected, "", replyType);
    }
    
    [TestCase(ReplyType.None)]
    [TestCase(ReplyType.User)]
    [TestCase(ReplyType.Bot)]
    public void CanHandleAttachments(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        var discordAttachmentMock = new Mock<IAttachment>();
        discordAttachmentMock.SetupGet(attachment => attachment.Filename).Returns(AttachmentFilename);
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>()
        {
            discordAttachmentMock.Object
        });
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MessageContent);
        ApplyReplyChanges(ref discordMessageMock, replyType);
        
        var actual = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        ReplyAssert.That(actual, MessageContent, $"{UserDisplayName} sent an attachment: '{AttachmentFilename}'", replyType);
    }
    
    [TestCase(ReplyType.None)]
    [TestCase(ReplyType.User)]
    [TestCase(ReplyType.Bot)]
    public void CanHandleStickers(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        var discordStickerMock = new Mock<IStickerItem>();
        discordStickerMock.SetupGet(sticker => sticker.Name).Returns(StickerName);
        discordStickerMock.SetupGet(sticker => sticker.Id).Returns(StickerId);
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<IStickerItem>()
        {
            discordStickerMock.Object
        });
        discordMessageMock.SetupGet(message => message.Content).Returns(MessageContent);
        ApplyReplyChanges(ref discordMessageMock, replyType);
        
        var actual = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        ReplyAssert.That(actual, MessageContent, $"{UserDisplayName} sent a '{StickerName}' sticker: https://media.discordapp.net/stickers/{StickerId}.webp", replyType);
    }
    
    [TestCase(ReplyType.None)]
    [TestCase(ReplyType.User)]
    [TestCase(ReplyType.Bot)]
    public void CanHandleTags(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
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
        ApplyReplyChanges(ref discordMessageMock, replyType);
        
        var actual = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
        string expected = """
            Emoji: :eyes:
            Server Emoji: :eyes_r:
            User: @Snasen
            Channel: #bloop
            Role: @himebot
            Everyone: @everyone
            Here: @here
            """.ReplaceLineEndings().Replace("\r\n", "\n");
        ReplyAssert.That(actual, expected, "", replyType);
    }
    
    [TestCase(ReplyType.None)]
    [TestCase(ReplyType.User)]
    [TestCase(ReplyType.Bot)]
    public void ThrowsForUnsupportedTags(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        
        var discordMessageTagMock = new Mock<ITag>();
        discordMessageTagMock.SetupGet(tag => tag.Type).Returns((TagType)99);
        
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>()
        {
            discordMessageTagMock.Object
        });
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<IStickerItem>());
        discordMessageMock.SetupGet(message => message.Content).Returns(string.Empty);
        ApplyReplyChanges(ref discordMessageMock, replyType);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent));
    }
    
    [Test]
    [TestCase(ReplyType.Unsupported)]
    public void ThrowsForUnsupportedUserType(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MessageContent);
        ApplyReplyChanges(ref discordMessageMock, replyType);
        var exception = Assert.Throws<NotSupportedException>(() => _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent));
        Console.WriteLine($"Content of expected exception:{Environment.NewLine}{exception}");
    }

    [Test]
    [TestCase(ReplyType.Bot)]
    [TestCase(ReplyType.User)]
    public void HasVaryingFormats(ReplyType replyType)
    {
        var discordMessageMock = new Mock<IMessage>();
        var discordUserMock = new Mock<IGuildUser>();
        discordUserMock.SetupGet(user => user.DisplayName).Returns(UserDisplayName);
        discordMessageMock.SetupGet(message => message.Author).Returns(discordUserMock.Object);
        discordMessageMock.SetupGet(message => message.Tags).Returns(new List<ITag>());
        discordMessageMock.SetupGet(message => message.Attachments).Returns(new List<IAttachment>());
        discordMessageMock.SetupGet(message => message.Stickers).Returns(new List<ISticker>());
        discordMessageMock.SetupGet(message => message.Content).Returns(MessageContent);
        ApplyReplyChanges(ref discordMessageMock, replyType);
 
        var messages = new List<string>();
        var previousLength = 0;
        do
        {
            Assert.That(previousLength, Is.LessThan(100));
            previousLength = messages.Count;
            var newItem = _discordMessageConverter.ToFFXIVCompatible(discordMessageMock.Object, DiscordMessageConverter.EventType.MessageSent);
            if (messages.Contains(newItem))
            {
                break;
            }
            messages.Add(newItem);
        } while (previousLength != messages.Count);
        
        Console.WriteLine("Has {0} reply variations", messages.Count);
        Assert.That(messages.Count, Is.GreaterThan(1));
    }
}