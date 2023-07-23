using FFXIVHelpers.Models;
using FFXIVHelpers.Persistence;

namespace FFXIVHelpers.Test.Persistence;

public class FilePersistenceTests
{
    private const string DISCORD_USERNAME = "DiscordUsername";
    private const string FFXIV_USERNAME = "FFXIVUsername";
    private const string FFXIV_WORLD = "World";
    [Test]
    public void CanLoadMappingsFromFile()
    {
        FilePersistence.DeleteMappingFile();
        var mapping = Mapping.CreateFromDiscord(DISCORD_USERNAME, new Character(FFXIV_USERNAME, FFXIV_WORLD));
        mapping.FFXIV.Confirm();
        var mappings = new List<Mapping>
        {
            mapping
        };
        var persistence = new FilePersistence();
        persistence.WriteMappingsToFile(mappings);
        
        {
            var loadedMappings = persistence.LoadMappings();
            Assert.Multiple(() =>
            {
                Assert.That(loadedMappings, Has.Count.EqualTo(1));
                Assert.That(loadedMappings[0].Discord.Name, Is.EqualTo("DiscordUsername"));
                Assert.That(loadedMappings[0].FFXIV.Name.Format(CharacterNameDisplay.WITH_WORLD), Is.EqualTo($"{FFXIV_USERNAME}@{FFXIV_WORLD}"));
            });
        }
        
        FilePersistence.DeleteMappingFile();
        
        {
            var loadedMappings = persistence.LoadMappings();
            Assert.Multiple(() =>
            {
                Assert.That(loadedMappings, Has.Count.EqualTo(0));
            });
        }
    }
}