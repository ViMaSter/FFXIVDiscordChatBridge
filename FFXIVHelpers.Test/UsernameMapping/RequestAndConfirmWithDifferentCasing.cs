using FFXIVHelpers.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class RequestAndConfirmWithDifferentCasing
{
    private readonly Character _hostingCharacter = new("Character Name", "World");
    
    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private readonly Character _requestingCharacterDifferentCasing = new("re'questing name", "world");
    private const string DiscordRequestingUsername = "Re'Questing";

    [Test]
    public void CanCreateMappingFromFFXIVFirstAndConfirmOnDiscord()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance);
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
            mapping.ReceiveFromDiscord(_requestingCharacterDifferentCasing, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername}"));
            });
        }
    }

    [Test]
    public void CanCreateMappingFromDiscordFirstAndConfirmOnFFXIV()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance);
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
            mapping.ReceiveFromFFXIV(_requestingCharacterDifferentCasing, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{DiscordRequestingUsername}"));
            });
        }
    }
}