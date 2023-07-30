using System.Text;
using FFXIVHelpers.Models;
using Microsoft.Extensions.Logging;
using Sharlayan.Core;

namespace FFXIVHelpers;

public class FFXIVByteHandler
{
    private readonly ILogger<FFXIVByteHandler> _logger;
    private readonly string _chatChannelCode;
    private const string TELL_CHAT_RECEIVING_CHANNEL = "000D";
    private IEnumerable<string> MonitoredChannels => new[]{_chatChannelCode, TELL_CHAT_RECEIVING_CHANNEL};
    public Character CurrentCharacter { get; }

    public FFXIVByteHandler(ILogger<FFXIVByteHandler> logger, string chatChannelCode, string playerName, string worldName)
    {
        _logger = logger;
        _chatChannelCode = chatChannelCode;
        _logger.LogDebug("Chat channel code: {ChatChannelCode}", chatChannelCode);
        CurrentCharacter = new Character(playerName, worldName);
        _logger.LogDebug("Current character: {PlayerName}@{WorldName}", playerName, worldName);
    }

    private static byte[][] Split(byte[] input, byte separator, bool ignoreEmptyEntries = false)
    {
        var subArrays = new List<byte[]>();
        var start = 0;
        for (var i = 0; i <= input.Length; ++i)
        {
            if (input.Length == i || input[i] == separator)
            {
                if (i - start > 0 || ignoreEmptyEntries)
                {
                    var destination = new byte[i - start];
                    Array.Copy(input, start, destination, 0, i - start);
                    subArrays.Add(destination);
                }
                start = i + 1;
            }
        }

        return subArrays.ToArray();
    }

    private static class ItemLinkReplacer
    {
        private static readonly byte?[] ItemLinkStartPattern = { 0x02, 0x48 };
        private static readonly byte?[] ItemLinkEndPattern = { 0x01, 0x03, 0x02, 0x13 };

        private static readonly byte?[] BufferPrecedingItemNameLengthPattern = { 0x02, 0x48 };
        private static readonly byte?[] ItemNameEndPattern = { 0x02, 0x27, 0x07 };

        private struct ItemReplacement
        {
            public int StartIndex;
            public int EndIndex;
            private string _itemName;
            public string ItemName
            {
                get => $"[[{_itemName}]]";
                set => _itemName = value;
            }
        }

        private static List<ItemReplacement> GetReplacementsFromText(byte[] rawMessage)
        {
            var result = new List<ItemReplacement>();

            var replacements = rawMessage.Locate(ItemLinkStartPattern);
            if (replacements.Length < 3)
            {
                return result;
            }

            var itemLinkBufferStartPositions = replacements.Where((_, i) => i % 3 == 0);

            foreach (var itemLinkBufferStartPosition in itemLinkBufferStartPositions)
            {
                var itemLinkBufferPreEnd = rawMessage.Skip(itemLinkBufferStartPosition).ToArray().Locate(ItemLinkEndPattern).Last() + ItemLinkEndPattern.Length;
                var itemLinkBufferEnd = itemLinkBufferPreEnd + rawMessage.Skip(itemLinkBufferStartPosition + itemLinkBufferPreEnd).ToArray().Locate(new byte?[] { 0x03 }).Last() + 1;
                var itemLinkBuffer = rawMessage.Skip(itemLinkBufferStartPosition).Take(itemLinkBufferEnd).ToArray();

                var bufferPrecedingItemNameLength = itemLinkBuffer.Locate(BufferPrecedingItemNameLengthPattern).Last() + BufferPrecedingItemNameLengthPattern.Length;
                var bufferPrecedingItemName = itemLinkBuffer[bufferPrecedingItemNameLength];
                
                var itemNameEndPosition = itemLinkBuffer.Locate(ItemNameEndPattern).Last();
                var itemNameStartIndex = bufferPrecedingItemNameLength + 1 + bufferPrecedingItemName;
                var itemName = itemLinkBuffer.Skip(itemNameStartIndex).Take(itemNameEndPosition - (itemNameStartIndex)).ToArray();

                result.Add(new ItemReplacement
                {
                    StartIndex = itemLinkBufferStartPosition,
                    EndIndex = itemLinkBufferStartPosition + itemLinkBufferEnd,
                    ItemName = Encoding.UTF8.GetString(itemName),
                });
            }

            return result;
        }

        public static byte[] ReplaceItemReferences(byte[] rawMessage)
        {
            var messageCopy = rawMessage.ToArray();

            var replacements = GetReplacementsFromText(messageCopy);

            // apply the replacements in reverse to not change positional indices
            replacements.Reverse();
            foreach (var replacement in replacements)
            {
                var before = new ArraySegment<byte>(messageCopy, 0, replacement.StartIndex);
                var mid = Encoding.UTF8.GetBytes(replacement.ItemName);
                var after = new ArraySegment<byte>(messageCopy, replacement.EndIndex, messageCopy.Length - (replacement.EndIndex));
                messageCopy = before.Concat(mid).Concat(after).ToArray();
            }

            return messageCopy;
        }
    }

    private static class LocationReplacer
    {
        private static readonly byte?[] ItemLinkStartPattern = { 0x02, 0x27, null, 0x04 };
        private static readonly byte?[] ItemLinkEndPattern = { 0x01, 0x03 };

        private static readonly byte?[] ItemNameStartPattern = { 0x03, 0xEE, 0x82, 0xBB, 0x02, 0x49, 0x02, 0x01, 0x03, 0x02, 0x48, 0x02, 0x01, 0x03 };
        private static readonly byte?[] ItemNameEndPattern = { 0x02, 0x27, 0x07 };

        private struct ItemReplacement
        {
            public int StartIndex;
            public int EndIndex;
            private string _itemName;
            public string ItemName
            {
                get
                {
                    var locationName = _itemName[.._itemName.IndexOf('(')].Trim();
                    var x = _itemName.Substring(_itemName.IndexOf('(') + 1, _itemName.IndexOf(',') - _itemName.IndexOf('(') - 1).Trim();
                    var y = _itemName.Substring(_itemName.IndexOf(',') + 1, _itemName.IndexOf(')') - _itemName.IndexOf(',') - 1).Trim();
                    
                    return $"**{locationName} @ {x}, {y}**";
                }
                init => _itemName = value;
            }
        }

        private static List<ItemReplacement> GetReplacementsFromText(byte[] rawMessage)
        {
            var result = new List<ItemReplacement>();

            var replacements = rawMessage.Locate(ItemLinkStartPattern);

            var itemLinkBufferStartPositions = replacements;

            foreach (var itemLinkBufferStartPosition in itemLinkBufferStartPositions)
            {
                var itemNameStartIndex = rawMessage.Skip(itemLinkBufferStartPosition).ToArray().Locate(ItemNameStartPattern).Last() + ItemNameStartPattern.Length;
                var itemNameEndPosition = rawMessage.Skip(itemLinkBufferStartPosition + itemNameStartIndex).ToArray().Locate(ItemNameEndPattern).Last();
                var itemName = rawMessage.Skip(itemLinkBufferStartPosition + itemNameStartIndex).Take(itemNameEndPosition).ToArray();
                var itemLinkBufferEnd = rawMessage.Skip(itemLinkBufferStartPosition + itemNameStartIndex).ToArray().Locate(ItemLinkEndPattern).Last() + ItemLinkEndPattern.Length;

                result.Add(new ItemReplacement
                {
                    StartIndex = itemLinkBufferStartPosition,
                    EndIndex = itemLinkBufferStartPosition + itemNameStartIndex + itemLinkBufferEnd,
                    ItemName = Encoding.UTF8.GetString(itemName),
                });
            }

            return result;
        }

        public static byte[] ReplaceItemReferences(byte[] rawMessage)
        {
            var messageCopy = rawMessage.ToArray();

            var replacements = GetReplacementsFromText(messageCopy);

            // apply the replacements in reverse to not change positional indices
            replacements.Reverse();
            foreach (var replacement in replacements)
            {
                var before = new ArraySegment<byte>(messageCopy, 0, replacement.StartIndex);
                var mid = Encoding.UTF8.GetBytes(replacement.ItemName);
                var after = new ArraySegment<byte>(messageCopy, replacement.EndIndex, messageCopy.Length - (replacement.EndIndex));
                messageCopy = before.Concat(mid).Concat(after).ToArray();
            }

            return messageCopy;
        }
    }

    public class PfLinkReplacer
    {
        private static readonly byte?[] PfLinkStartPattern = { 0x02, 0x27, 0x08, 0x0A };
        private static readonly byte?[] PfLinkEndPattern = { 0x01, 0x03 };

        private static readonly byte?[] PfNameStartPattern = { 0x48, 0x02, 0x01, 0x03 };
        private static readonly byte?[] PfNameEndPattern = { 0x20, 0x02, 0x12, 0x02, 0x59, 0x03, 0x02, 0x27, 0x07 };

        public struct PfReplacement
        {
            public int StartIndex;
            public int EndIndex;
            private string _rawPfEntry;
            public string PfEntry
            {
                get => $"{{{_rawPfEntry}}}";
                set => _rawPfEntry = value;
            }
        }

        private static List<PfReplacement> GetReplacementsFromText(byte[] rawMessage)
        {
            var result = new List<PfReplacement>();

            var itemLinkStartPositions = rawMessage.Locate(PfLinkStartPattern);

            foreach (var itemLinkStartPosition in itemLinkStartPositions)
            {
                var itemNameEndPosition = rawMessage.Skip(itemLinkStartPosition).ToArray().Locate(PfNameEndPattern)[0];

                var itemLinkBuffer = rawMessage.Skip(itemLinkStartPosition).Take(itemNameEndPosition).ToArray();
                var itemNameStartPosition = itemLinkBuffer.Locate(PfNameStartPattern).Last() + PfNameStartPattern.Length;
                var itemName = itemLinkBuffer.Skip(itemNameStartPosition).Take(itemNameEndPosition).ToArray();

                var fullStop = rawMessage.Skip(itemLinkStartPosition + itemNameStartPosition).ToArray().Locate(PfLinkEndPattern)[0];

                result.Add(new PfReplacement
                {
                    StartIndex = itemLinkStartPosition,
                    EndIndex = itemLinkStartPosition + itemNameStartPosition + fullStop + PfLinkEndPattern.Length,
                    PfEntry = Encoding.UTF8.GetString(itemName),
                });
            }

            return result;
        }

        public static byte[] ReplaceItemReferences(byte[] rawMessage)
        {
            var messageCopy = rawMessage.ToArray();

            var replacements = GetReplacementsFromText(messageCopy);

            // apply the replacements in reverse to not change positional indices
            replacements.Reverse();
            foreach (var replacement in replacements)
            {
                var before = new ArraySegment<byte>(messageCopy, 0, replacement.StartIndex);
                var mid = Encoding.UTF8.GetBytes(replacement.PfEntry);
                var after = new ArraySegment<byte>(messageCopy, replacement.EndIndex, messageCopy.Length - (replacement.EndIndex));
                messageCopy = before.Concat(mid).Concat(after).ToArray();
            }

            return messageCopy;
        }
    }

    public bool TryFFXIVToDiscordFriendly(ChatLogItem chatLogItem, out FromFFXIV? chatLog)
    {
        if (MonitoredChannels.All(channel => channel != chatLogItem.Code))
        {
            _logger.LogTrace("Skipping message: Irrelevant channel code '{Code}'", chatLogItem.Code);
            chatLog = null;
            return false;
        }
        
        var isTell = chatLogItem.Code == TELL_CHAT_RECEIVING_CHANNEL;

        if (chatLogItem.Line.StartsWith(CurrentCharacter.CharacterName) && !chatLogItem.Line.Contains("FORCEEXEC"))
        {
            _logger.LogTrace($"Skipping message: Message from current character and no override (FORCEEXEC) present");
            chatLog = null;
            return false;
        }
        
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem - This is a log message, not a template
        _logger.LogTrace(BitConverter.ToString(chatLogItem.Bytes));

        var utf8Message = ItemLinkReplacer.ReplaceItemReferences(chatLogItem.Bytes);
        utf8Message = LocationReplacer.ReplaceItemReferences(utf8Message);
        utf8Message = PfLinkReplacer.ReplaceItemReferences(utf8Message);

        var split = Split(utf8Message, 0x1F);

        Character? logCharacter = null;
        string? logMessage = null;

        // cross-world user require special treatment
        switch (split[1].Count(b => b == 0x03))
        {
            case 3:
            {
                var crossWorldInfoSplit = Split(split[1], 0x03);
                logCharacter = new Character(
                    Encoding.UTF8.GetString(crossWorldInfoSplit[1].TakeWhile(item => item != 0x02).ToArray()),
                    Encoding.UTF8.GetString(crossWorldInfoSplit[3])
                );
                logMessage = Encoding.UTF8.GetString(split[2]).Trim();
            }
                break;
            case 2:
            {
                var crossWorldInfoSplit = Split(split[1], 0x03);
                logCharacter = new Character(
                    Encoding.UTF8.GetString(crossWorldInfoSplit[1].TakeWhile(item => item != 0x02).ToArray()),
                    CurrentCharacter.WorldName
                );
                logMessage = Encoding.UTF8.GetString(split[2]).Trim();
            }
                break;
            case 0:
                logCharacter = new Character(
                    Encoding.UTF8.GetString(split[1]),
                    CurrentCharacter.WorldName
                );
                logMessage = Encoding.UTF8.GetString(split[2]).Trim();
                break;
        }

        if (logCharacter == null)
        {
            _logger.LogTrace($"Skipping message: Failed to parse character name'");
            chatLog = null;
            return false;
        }

        if (string.IsNullOrEmpty(logMessage))
        {
            _logger.LogTrace($"Skipping message: Empty message'");
            chatLog = null;
            return false;
        }

        if (isTell)
        {
            chatLog = new FromTellMessage(logCharacter, logMessage);
            return true;
        }
        
        chatLog = new FromMonitoredChannel(logCharacter, logMessage);
        return true;
    }
}