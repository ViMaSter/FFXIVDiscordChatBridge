using FFXIVHelpers.Models;
using FFXIVHelpers.Persistence;

namespace FFXIVHelpers.Test.Persistence;

public class FilePersistenceTests
{
    private const string DiscordUsername = "DiscordUsername";
    private const string FFXIVUsername = "FFXIVUsername";
    private const string FFXIVWorld = "World";
    [Test]
    public void CanLoadMappingsFromFile()
    {
        FilePersistence.DeleteMappingFile();
        var mapping = Mapping.CreateFromDiscord(DiscordUsername, new Character(FFXIVUsername, FFXIVWorld));
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
                Assert.That(loadedMappings[0].FFXIV.Name.Format(CharacterNameDisplay.WithWorld), Is.EqualTo($"{FFXIVUsername}@{FFXIVWorld}"));
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