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
        Assert.Throws<InvalidOperationException>(() =>
        {
            var cast = $"{character}";
        });
        Assert.Throws<InvalidOperationException>(() =>
        {
            var cast = character.ToString();
        });
    }
    
    // handle reference comparisons
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
}