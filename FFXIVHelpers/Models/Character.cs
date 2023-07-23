namespace FFXIVHelpers.Models;

public record Character(string CharacterName, string WorldName)
{
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
        throw new InvalidOperationException("Use .Format() instead");
    }
}