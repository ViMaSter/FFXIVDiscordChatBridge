namespace FFXIVDiscordChatBridge.Consumer;

public interface IFFXIVConsumer
{
    Task Start();
    void Dispose();
    
    public delegate Task OnNewChatMessageDelegate(string message);
    event OnNewChatMessageDelegate OnNewChatMessage;
}