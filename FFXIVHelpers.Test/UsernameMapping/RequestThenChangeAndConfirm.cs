using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class RequestThenChangeAndConfirm
{
    private readonly Character _hostingCharacter = new("Character Name", "World");

    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private readonly Character _requestingCharacter2 = new("Re'Questing Name2", "World");
    private const string DiscordRequestingUsername = "Re'Questing";
    private const string DiscordRequestingUsername2 = "Re'Questing2";

    [Test]
    public void CanCreateMappingFromFFXIVRequestAnotherMappingInFFXIVAndConfirmOnDiscord()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);
            
        {
            mapping.ReceiveFromFFXIV(_requestingCharacter, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }
            
        {
            mapping.ReceiveFromFFXIV(_requestingCharacter2, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo(_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }

        {
            mapping.ReceiveFromDiscord(_requestingCharacter2, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter2), Is.EqualTo($"{_requestingCharacter2.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername}"));
            });
        }
    }
        
    [Test]
    public void CanCreateMappingFromDiscordRequestAnotherMappingInDiscordAndConfirmOnFFXIV()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);
            
        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }
            
        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DiscordRequestingUsername2, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername2), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)));
            });
        }

        {
            mapping.ReceiveFromFFXIV(_requestingCharacter, DiscordRequestingUsername2, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername2), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername2}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername2}"));
            });
        }
    }
}