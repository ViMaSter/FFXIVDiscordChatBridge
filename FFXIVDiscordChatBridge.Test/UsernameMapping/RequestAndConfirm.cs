using FFXIVByteParser;
using FFXIVByteParser.Models;

namespace FFXIVDiscordChatBridge.Test.UsernameMapping;

public class RequestAndConfirm
{
    private readonly Character _hostingCharacter = new("Character Name", "World");
    
    private readonly Character _requestingCharacter = new("Requesting Name", "World");
    private const string DISCORD_REQUESTING_USERNAME = "Requesting";

    [Test]
    public void CanCreateMappingFromFFXIVFirstAndConfirmOnDiscord()
    {
        FFXIVDiscordChatBridge.UsernameMapping mapping = new();
        mapping.SetHostingCharacter(_hostingCharacter);

        {
            mapping.ReceiveFromFFXIV(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }

        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }

    [Test]
    public void CanCreateMappingFromDiscordFirstAndConfirmOnFFXIV()
    {
        FFXIVDiscordChatBridge.UsernameMapping mapping = new();
        mapping.SetHostingCharacter(_hostingCharacter);

        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }

        {
            mapping.ReceiveFromFFXIV(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }
}