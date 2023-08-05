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
        var guildUser = socketMessage.Author as IGuildUser;
        var textContent = socketMessage.Content;
        var trackedTags = socketMessage.Tags.Select(AddTrackableIndex).ToList();

        for (var index = 0; index < trackedTags.Count; index++)
        {
            var socketMessageTag = trackedTags[index];
            switch (socketMessageTag.Tag.Type)
            {
                case TagType.Emoji:
                {
                    var emote = (Emote)socketMessageTag.Tag.Value;
                    var replacement = $":{emote.Name}:";
                    var replacementLengthDifference = replacement.Length - socketMessageTag.Tag.Length;

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Tag.Length).Insert(socketMessageTag.Index, replacement);
                    Helper.UpdateIndices(ref trackedTags, socketMessageTag.Index, replacementLengthDifference);
                    break;
                }
                case TagType.UserMention:
                {
                    var user = (IGuildUser)socketMessageTag.Tag.Value;
                    var replacement = $"@{user.Nickname}";
                    var replacementLengthDifference = replacement.Length - socketMessageTag.Tag.Length;

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Tag.Length).Insert(socketMessageTag.Index, replacement);
                    Helper.UpdateIndices(ref trackedTags, socketMessageTag.Index, replacementLengthDifference);
                    break;
                }

                case TagType.ChannelMention:
                {
                    var channel = (IGuildChannel)socketMessageTag.Tag.Value;

                    var replacement = $"#{channel.Name}";
                    var replacementLengthDifference = replacement.Length - socketMessageTag.Tag.Length;

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Tag.Length).Insert(socketMessageTag.Index, replacement);
                    Helper.UpdateIndices(ref trackedTags, socketMessageTag.Index, replacementLengthDifference);
                    break;
                }
                case TagType.RoleMention:
                {
                    var role = (IRole)socketMessageTag.Tag.Value;
                    var replacement = $"@{role.Name}";
                    var replacementLengthDifference = replacement.Length - socketMessageTag.Tag.Length;

                    textContent = textContent.Remove(socketMessageTag.Index, socketMessageTag.Tag.Length).Insert(socketMessageTag.Index, replacement);
                    Helper.UpdateIndices(ref trackedTags, socketMessageTag.Index, replacementLengthDifference);
                    break;
                }
                case TagType.EveryoneMention:
                    break;

                case TagType.HereMention:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        textContent = _discordEmojiConverter.ReplaceEmoji(textContent);

        var fromUser = _usernameMapping.GetMappingFromDiscordUsername(guildUser.Username) ?? guildUser.DisplayName;

        _logger.LogInformation("Received message from Discord ({EventType}): {FullMessage}", textContent, eventType);

        textContent = ApplyAuthorWrapper(textContent.ReplaceLineEndings(), fromUser, eventType).Trim();
        
        textContent = socketMessage.Attachments.Aggregate(textContent, (current, attachment) => current + $"{Environment.NewLine}{fromUser} sent an attachment: '{attachment.Filename}'");
        textContent = socketMessage.Stickers.Aggregate(textContent, (current, socketMessageSticker) => current + $"{Environment.NewLine}{fromUser} sent a '{socketMessageSticker.Name}' sticker: https://media.discordapp.net/stickers/{socketMessageSticker.Id}.webp");
        
        // override message format if this is a reply
        var isReply = socketMessage.Reference != null;
        if (!isReply)
        {
            return textContent;
        }

        ApplyReplyWrapper(socketMessage, ref textContent, fromUser, eventType);

        return textContent.Trim();
    }

    private static TrackableIndexTag AddTrackableIndex(ITag arg)
    {
        return new TrackableIndexTag()
        {
            Index = arg.Index,
            Tag = arg
        };
    }

    private string ApplyAuthorWrapper(string textContent, string fromUser, EventType eventType)
    {
        var fullOutput = "";
        foreach (var line in textContent.Split(Environment.NewLine))
        {
            if (string.IsNullOrEmpty(line.Trim()))
            {
                continue;
            }

            var optionalEditSuffix = eventType == EventType.MessageEdited ? " (edit)" : "";

            fullOutput += $"[{fromUser}{optionalEditSuffix}]: {line}" + Environment.NewLine;
        }

        return fullOutput;
    }

    private void ApplyReplyWrapper(IMessage socketUserMessage, ref string input, string? fromUser, EventType eventType)
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

public class TrackableIndexTag
{
    public int Index;
    public ITag Tag;

    public void SetIndex(int index)
    {
        Index = index;
    }
}

public static class Helper
{
    public static void UpdateIndices(ref List<TrackableIndexTag> trackedTags, int index, int difference)
    {
        foreach (var trackedTag in trackedTags)
        {
            if (trackedTag.Index < index)
            {
                continue;
            }
            trackedTag.SetIndex(trackedTag.Index + difference);
            
        }
    }
}