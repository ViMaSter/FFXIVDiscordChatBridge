using FFXIVHelpers.Models;
// ReSharper disable UnusedVariable - This test suite deliberately handles exceptions thrown when casting. Not using assignments, this code would run into no-ops

namespace FFXIVHelpers.Test;

public class CharacterTests
{
    // implicitly convert to string and throw NotSupportedException
    [Test]
    public void ToStringThrows()
    {
        var character = new Character("Character Name", "World");
        Assert.That(character.ToString(), Is.EqualTo(character.Format(CharacterNameDisplay.WithWorld)));
    }
    
    [Test]
    public void ReferenceComparison()
    {
        var character = new Character("Character Name", "World");
        Assert.Multiple(() =>
        {
            Assert.That(character.Equals(null), Is.False);
            Assert.That(character!.Equals(character), Is.True);
            Assert.That(character.Equals(null), Is.False);
            Assert.That(character!.Equals(new object()), Is.False);
            Assert.That(character.Equals(null), Is.False);
            Assert.That(character!.Equals(character), Is.True);
            Assert.That(character.Equals(new Character("Character Name", "World")), Is.True);
            Assert.That(character.Equals(new Character("Character Name", "World2")), Is.False);
            Assert.That(character.Equals(new Character("Character Name2", "World")), Is.False);
            Assert.That(character.Equals(new Character("Character Name2", "World2")), Is.False);
        });
    }
    
    // handle hash map usage
    [Test]
    public void GetHashCodeThrows()
    {
        // add same object twice to hashmap and assert that it is only added once
        var character = new Character("Character Name", "World");
        var hashSet = new HashSet<Character>
        {
            character,
            character
        };
        Assert.That(hashSet, Has.Count.EqualTo(1));
    }
}