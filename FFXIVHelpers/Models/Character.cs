namespace FFXIVHelpers.Models;

public class Character
{
    public string CharacterName { get; }
    public string WorldName { get; }
    
    public Character(string characterName, string worldName)
    {
        CharacterName = characterName;
        WorldName = worldName;
    }
    
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

    public override int GetHashCode()
    {
        return HashCode.Combine(CharacterName, WorldName);
    }

    public string Format(CharacterNameDisplay characterNameDisplay)
    {
        if (characterNameDisplay == CharacterNameDisplay.WithWorld)
        {
            return $"{CharacterName}@{WorldName}";
        }

        return $"{CharacterName}";
    }

    public override string ToString()
    {
        return Format(CharacterNameDisplay.WithWorld);
    }
}