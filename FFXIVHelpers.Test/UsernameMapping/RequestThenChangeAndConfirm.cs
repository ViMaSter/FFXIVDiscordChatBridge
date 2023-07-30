using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class RequestThenChangeAndConfirm
{
    private readonly Character _hostingCharacter = new("Character Name", "World");

    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private readonly Character _requestingCharacter2 = new("Re'Questing Name2", "World");
    private const string DISCORD_REQUESTING_USERNAME = "Re'Questing";
    private const string DISCORD_REQUESTING_USERNAME2 = "Re'Questing2";

    [Test]
    public void CanCreateMappingFromFFXIVRequestAnotherMappingInFFXIVAndConfirmOnDiscord()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);
            
        {
            Assert.Multiple(() =>
            {
                Assert.That(mapping.ReceiveFromFFXIV(_requestingCharacter, DISCORD_REQUESTING_USERNAME), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.Null);
            });
        }
            
        {
            Assert.Multiple(() =>
            {
                Assert.That(mapping.ReceiveFromFFXIV(_requestingCharacter2, DISCORD_REQUESTING_USERNAME), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.Null);
            });
        }

        {
            mapping.ReceiveFromDiscord(_requestingCharacter2, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }
        
    [Test]
    public void CanCreateMappingFromDiscordRequestAnotherMappingInDiscordAndConfirmOnFFXIV()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);
            
        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.Null);
            });
        }
            
        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DISCORD_REQUESTING_USERNAME2, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME2), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.Null);
            });
        }

        {
            Assert.Multiple(() =>
            {
                Assert.That(mapping.ReceiveFromFFXIV(_requestingCharacter, DISCORD_REQUESTING_USERNAME2), Is.True);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME2), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME2}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME2}"));
            });
        }
    }
}