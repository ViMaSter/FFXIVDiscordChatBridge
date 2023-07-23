namespace FFXIVHelpers.Models;

public abstract class FromFFXIV
{
    protected FromFFXIV(Character character, string message)
    {
        Character = character;
        Message = message;
    }

    public Character Character { get; }
    public string Message { get; }
}
public class FromTellMessage : FromFFXIV
{
    public FromTellMessage(Character character, string message) : base(character, message)
    {
    }
}
    
public class FromMonitoredChannel : FromFFXIV
{
    public FromMonitoredChannel(Character character, string message) : base(character, message)
    {
    }
}