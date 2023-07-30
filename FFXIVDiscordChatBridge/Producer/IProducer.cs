using FFXIVHelpers.Models;

namespace FFXIVDiscordChatBridge.Producer;

public interface IFFXIV
{
    Task Send(string message);
}

public interface IDiscord
{
    Task Send(Character sender, string discordMappedName, string message);
}
