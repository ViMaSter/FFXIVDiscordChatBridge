namespace FFXIVHelpers.Models;

public class Character
{
    private bool Equals(Character other)
    {
        return string.Equals(CharacterName, other.CharacterName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(WorldName, other.WorldName, StringComparison.InvariantCultureIgnoreCase);
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Character)obj);
    }

    public Character(string characterName, string worldName)
    {
        CharacterName = characterName;
        WorldName = worldName;
    }

    public string Format(CharacterNameDisplay characterNameDisplay)
    {
        if (characterNameDisplay == CharacterNameDisplay.WITH_WORLD)
        {
            return $"{CharacterName}@{WorldName}";
        }

        return $"{CharacterName}";
    }

    public override string ToString()
    {
        return Format(CharacterNameDisplay.WITH_WORLD);
    }

    public string CharacterName { get; }
    public string WorldName { get; }
}