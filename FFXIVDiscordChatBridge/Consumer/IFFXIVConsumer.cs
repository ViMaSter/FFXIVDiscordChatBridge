namespace FFXIVDiscordChatBridge.Consumer;

public interface IFFXIVConsumer
{
    Task Start();
    void Dispose();
}