using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class RespectsDisplayNames
{
    private readonly Character _hostingCharacter = new("Character Name", "World");
    
    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private readonly Character _requestingCharacterDifferentCasing = new("re'questing name", "world");
    
    private const string DiscordRequestingUsername = "Re'Questing";
    private const string DiscordRequestingDisplayname = "Dis'play";

    [Test]
    public void CanCreateMappingFromFFXIVFirstAndConfirmOnDiscord()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);

        {
            Assert.Multiple(() =>
            {
                Assert.That(mapping.ReceiveFromFFXIV(_requestingCharacter, DiscordRequestingUsername), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.Null);
            });
        }

        {
            mapping.ReceiveFromDiscord(_requestingCharacterDifferentCasing, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingUsername}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingUsername}"));
            });
        }
        
        mapping.UpdateDisplayNameMapping(new Dictionary<string, string>
        {
            {DiscordRequestingUsername, DiscordRequestingDisplayname}
        });
        Assert.Multiple(() =>
        {
            Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname}"));
            Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname}"));
        });
        
        mapping.UpdateDisplayNameMapping(new Dictionary<string, string>
        {
            {DiscordRequestingUsername, DiscordRequestingDisplayname+2}
        });
        Assert.Multiple(() =>
        {
            Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname+2}"));
            Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname+2}"));
        });
    }

    [Test]
    public void CanCreateMappingFromDiscordFirstAndConfirmOnFFXIV()
    {
        FFXIVHelpers.UsernameMapping mapping = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>()));
        mapping.SetHostingCharacter(_hostingCharacter);

        {
            mapping.ReceiveFromDiscord(_requestingCharacter, DiscordRequestingUsername, out var message);
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(message), Is.False);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.Null);
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.Null);
            });
        }

        {
            Assert.Multiple(() =>
            {
                Assert.That(mapping.ReceiveFromFFXIV(_requestingCharacterDifferentCasing, DiscordRequestingUsername), Is.True);
                Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingUsername}"));
                Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingUsername}"));
            });
        }
        
        mapping.UpdateDisplayNameMapping(new Dictionary<string, string>
        {
            {DiscordRequestingUsername, DiscordRequestingDisplayname}
        });
        Assert.Multiple(() =>
        {
            Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname}"));
            Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname}"));
        });
        
        mapping.UpdateDisplayNameMapping(new Dictionary<string, string>
        {
            {DiscordRequestingUsername, DiscordRequestingDisplayname+2}
        });
        Assert.Multiple(() =>
        {
            Assert.That(mapping.GetMappingFromDiscordUsername(DiscordRequestingUsername), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname+2}"));
            Assert.That(mapping.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DiscordRequestingDisplayname+2}"));
        });
    }
}