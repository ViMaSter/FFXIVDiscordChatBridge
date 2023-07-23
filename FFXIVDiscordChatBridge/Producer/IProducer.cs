namespace FFXIVDiscordChatBridge.Producer;

public interface IFFXIV
{
    Task Send(string message);
}

public interface IDiscord
{
    Task Send(string message);
}
