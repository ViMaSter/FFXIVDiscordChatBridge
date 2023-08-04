using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace FFXIVHelpers.Extensions;

public class DiscordMessageConverter
{
    private readonly DiscordEmojiConverter _discordEmojiConverter;
    private readonly UsernameMapping _usernameMapping;
    private readonly ILogger<DiscordMessageConverter> _logger;
    private readonly IDiscordClientWrapper _discordWrapper;
    
    private const string FFXIV_ARROW = ""; // this will render as '6' or '?' outside of FFXIV

    private static readonly string[] Formats = {
        "[{0}] in response to [{1}]:" + Environment.NewLine +
        FFXIV_ARROW+" {2}",
        
        "[{0}] @ [{1}]:" + Environment.NewLine +
        FFXIV_ARROW+" {2}",
        
        "[{0}] replying to [{1}]:" + Environment.NewLine +
        FFXIV_ARROW+" {2}",
        
        "[{0}] responding to [{1}]:" + Environment.NewLine +
        FFXIV_ARROW+" {2}",
        
        "[{0}] in response to [{1}]:" + Environment.NewLine +
        "> {2}",
        
        "[{0}] @ [{1}]:" + Environment.NewLine +
        "> {2}",
        
        "[{0}] replying to [{1}]:" + Environment.NewLine +
        "> {2}",
        
        "[{0}] responding to [{1}]:" + Environment.NewLine +
        "> {2}",
    };
    private static int _lastUsedFormat;
    private static string NextFormat
    {
        get
        {
            _lastUsedFormat++;
            if (_lastUsedFormat >= Formats.Length)
            {
                _lastUsedFormat = 0;
            }

            return Formats[_lastUsedFormat];
        }
    }

    public DiscordMessageConverter(DiscordEmojiConverter discordEmojiConverter, UsernameMapping usernameMapping, ILogger<DiscordMessageConverter> logger, IDiscordClientWrapper discordWrapper)
    {
        _discordEmojiConverter = discordEmojiConverter;
        _usernameMapping = usernameMapping;
        _logger = logger;
        _discordWrapper = discordWrapper;
    }
    
    
    public enum EventType
    {
        MessageSent,
        MessageEdited
    }

    public string ToFFXIVCompatible(IMessage socketMessage, EventType eventType)
    {
        if (socketMessage.Author is not IGuildUser guildUser)
        {
            throw new InvalidOperationException("Message received from non-guild user");
        }
        
        var textContent = socketMessage.Content;

        foreach (var socketMessageTag in socketMessage.Tags)
        {
            switch (socketMessageTag.Type)
            {
                case TagType.Emoji:
                    if (socketMessageTag.Value is not Emote e)
                    {
                        continue;
                    }

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $":{e.Name}:");
                    break;
                case TagType.UserMention:
                    if (socketMessageTag.Value is not SocketGuildUser u)
                    {
                        continue;
                    }

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $"@{u.Nickname}");
                    break;

                case TagType.ChannelMention:
                    if (socketMessageTag.Value is not SocketGuildChannel c)
                    {
                        continue;
                    }

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $"#{c.Name}");
                    break;
                case TagType.RoleMention:
                    if (socketMessageTag.Value is not SocketRole r)
                    {
                        continue;
                    }

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Length).Insert(socketMessageTag.Index, $"@{r.Name}");
                    break;
                case TagType.EveryoneMention:
                    break;

                case TagType.HereMention:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        textContent = textContent.ReplaceLineEndings();
        textContent = _discordEmojiConverter.ReplaceEmoji(textContent).Trim();

        var fromUser = _usernameMapping.GetMappingFromDiscordUsername(guildUser.Username) ?? guildUser.DisplayName;

        _logger.LogInformation("Received message from Discord ({EventType}): {FullMessage}", textContent, eventType);
        foreach (var line in textContent.Split(Environment.NewLine))
        {
            if (string.IsNullOrEmpty(line.Trim()))
            {
                continue;
            }

            var optionalEditSuffix = eventType == EventType.MessageEdited ? " (edit)" : "";

            textContent = $"[{fromUser}{optionalEditSuffix}]: {textContent}" + Environment.NewLine;
        }

        textContent = socketMessage.Attachments.Aggregate(textContent, (current, attachment) => current + $"{fromUser} sent an attachment: '{attachment.Filename}'");
        textContent = socketMessage.Stickers.Aggregate(textContent, (current, socketMessageSticker) => current + $"{fromUser} sent a '{socketMessageSticker.Name}' sticker: https://media.discordapp.net/stickers/{socketMessageSticker.Id}.webp");

        // override message format if this is a reply
        var isReply = socketMessage.Reference != null;
        if (!isReply)
        {
            return textContent.Trim();
        }

        ApplyReplyWrapper(socketMessage, ref textContent, fromUser);

        return textContent.Trim();
    }

    private void ApplyReplyWrapper(IMessage socketUserMessage, ref string input, string? fromUser)
    {
        var originalMessage = _discordWrapper.Channel!.GetMessageAsync(socketUserMessage.Reference.MessageId.Value).Result;
        if (originalMessage is not RestUserMessage originalUserMessage)
        {
            _logger.LogInformation("Received reply to non-UserMessage from Discord: {Message}", originalMessage);
            return;
        }

        var toUserDisplayName = originalUserMessage.Author switch
        {
            SocketGuildUser toUser => _usernameMapping.GetMappingFromDiscordUsername(toUser.Username) ?? toUser.Username,
            RestWebhookUser toBot => toBot.Username,
            _ => input
        };

        input = string.Format(NextFormat, fromUser, toUserDisplayName, input);
        _logger.LogInformation("Updating message format to handle response from {NewAuthor} to {OriginalAuthor}", fromUser, toUserDisplayName);
    }
}