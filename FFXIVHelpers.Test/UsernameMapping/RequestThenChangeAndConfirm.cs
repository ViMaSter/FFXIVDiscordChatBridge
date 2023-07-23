using FFXIVHelpers.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class RequestThenChangeAndConfirm
{
    private readonly Character _hostingCharacter = new("Character Name", "World");

    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private readonly Character _requestingCharacter2 = new("Re'Questing Name2", "World");
    private const string DISCORD_REQUESTING_USERNAME = "Re'Questing";

    [Test]
    public void CanCreateMappingFromFFXIVRequestAnotherMappingInFFXIVAndConfirmOnDiscord()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance);
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
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance);
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