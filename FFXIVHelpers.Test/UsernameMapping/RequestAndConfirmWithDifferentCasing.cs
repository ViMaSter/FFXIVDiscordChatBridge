using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class RequestAndConfirmWithDifferentCasing
{
    private readonly Character _hostingCharacter = new("Character Name", "World");
    
    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private readonly Character _requestingCharacterDifferentCasing = new("re'questing name", "world");
    private const string DISCORD_REQUESTING_USERNAME = "Re'Questing";

    [Test]
    public void CanCreateMappingFromFFXIVFirstAndConfirmOnDiscord()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);

        {
            mapping.ReceiveFromFFXIV(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)));
            });
        }

        {
            mapping.ReceiveFromDiscord(_requestingCharacterDifferentCasing, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }

    [Test]
    public void CanCreateMappingFromDiscordFirstAndConfirmOnFFXIV()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);

        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo(_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)));
            });
        }

        {
            mapping.ReceiveFromFFXIV(_requestingCharacterDifferentCasing, DISCORD_REQUESTING_USERNAME, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }
}