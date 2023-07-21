namespace FFXIVDiscordChatBridge.Producer;

public interface IFFXIVProducer
{
    Task Send(string message);
}

public interface IDiscordProducer
{
    Task Send(string message);
}
