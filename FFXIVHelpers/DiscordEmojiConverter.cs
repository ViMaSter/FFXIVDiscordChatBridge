using System.Reflection;
using Discord;

namespace FFXIVHelpers;

public class DiscordEmojiConverter
{
    private readonly Dictionary<string, string> _unicodeAndNames = new();

    public DiscordEmojiConverter()
    {
        var emojiNamesAndUnicode = typeof(Emoji).GetProperty("NamesAndUnicodes", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as Dictionary<string, string>;
        foreach (var (key, value) in emojiNamesAndUnicode ?? Enumerable.Empty<KeyValuePair<string, string>>())
        {
            _unicodeAndNames.TryAdd(value, key);
        }
    }
    public string ReplaceEmoji(string input)
    {
        var foundCodes = _unicodeAndNames?.Keys.Where(input.Contains).ToList();
        return (foundCodes ?? Enumerable.Empty<string>()).Aggregate(input, (current, unicode) => current.Replace(unicode, $"{_unicodeAndNames![unicode]}"));
    }
}