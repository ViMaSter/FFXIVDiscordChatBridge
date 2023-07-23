namespace FFXIVHelpers.Test;

public class EmojiStringHelperTests
{
    [Test]
    public void PropertyReplacesEmoji()
    {
        var helper = new DiscordEmojiConverter();
        Assert.That(helper.ReplaceEmoji("👍👀"), Is.EqualTo(":+1::eyes:"));
    }
}