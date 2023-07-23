using System.Numerics;
using FFXIVByteParser;
using FFXIVByteParser.Models;

namespace FFXIVDiscordChatBridge.Test.UsernameMapping;

public class RequestThenChangeAndConfirm
{
    private readonly Character _hostingCharacter = new("Character Name", "World");

    private readonly Character _requestingCharacter = new("Requesting Name", "World");
    private readonly Character _requestingCharacter2 = new("Requesting Name2", "World");
    private const string DISCORD_REQUESTING_USERNAME = "Requesting";

    [Test]
    public void CanCreateMappingFromFFXIVRequestAnotherMappingInFFXIVAndConfirmOnDiscord()
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
            mapping.ReceiveFromFFXIV(_requestingCharacter2, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo(_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }

        {
            mapping.ReceiveFromDiscord(_requestingCharacter2, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }
        
    [Test]
    public void CanCreateMappingFromDiscordRequestAnotherMappingInDiscordAndConfirmOnFFXIV()
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
            mapping.ReceiveFromDiscord(_requestingCharacter2, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo(_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }

        {
            mapping.ReceiveFromFFXIV(_requestingCharacter2, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }
}