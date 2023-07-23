using FFXIVHelpers.Models;
using FFXIVHelpers.Test.UsernameMapping.Stubs;
using Microsoft.Extensions.Logging.Abstractions;

namespace FFXIVHelpers.Test.UsernameMapping;

public class CanAccessDataFromPersistence
{
    private readonly Character _hostingCharacter = new("Character Name", "World");
    
    private readonly Character _requestingCharacter = new("Re'Questing Name", "World");
    private const string DISCORD_REQUESTING_USERNAME = "Re'Questing";

    [Test]
    public void CanAccessMappingThatIsPreSetup()
    {
        var mapping = Mapping.CreateFromDiscord(DISCORD_REQUESTING_USERNAME, _requestingCharacter);
        mapping.FFXIV.Confirm();
        FFXIVHelpers.UsernameMapping mappings = new(NullLogger<FFXIVHelpers.UsernameMapping>.Instance, new InMemoryPersistence(new List<Mapping>
        {
            mapping
        }));
        mappings.SetHostingCharacter(_hostingCharacter);

        {
            Assert.Multiple(() =>
            {
                Assert.That(mappings.GetMappingFromDiscordUsername(DISCORD_REQUESTING_USERNAME), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
                Assert.That(mappings.GetMappingFromFFXIVUsername(_requestingCharacter), Is.EqualTo($"{_requestingCharacter.Format(CharacterNameDisplay.WithoutWorld)}/@{DISCORD_REQUESTING_USERNAME}"));
            });
        }
    }
}