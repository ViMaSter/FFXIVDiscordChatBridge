// ReSharper disable NotAccessedPositionalProperty.Global - External API definition
// ReSharper disable InconsistentNaming - External API definition
namespace FFXIVDiscordChatBridge.Producer.DiscordWebhookRequest;

public record RootObject(
    string content,
    string username,
    string avatar_url
);