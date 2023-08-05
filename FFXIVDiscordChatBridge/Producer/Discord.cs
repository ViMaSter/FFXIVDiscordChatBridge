using System.Net.Http.Json;
using FFXIVDiscordChatBridge.Producer.DiscordWebhookRequest;
using FFXIVHelpers.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FFXIVDiscordChatBridge.Producer;

public class Discord : IDiscord
{
    public const string XIVAPI_CLIENT_STRING = "XIVAPI";

    private readonly ILogger<Discord> _logger;
    private readonly HttpClient _xivapiClient;
    private readonly Dictionary<Character, string> _avatarCache = new();
    private readonly string _discordWebhookURL;
    private readonly IDiscordClientWrapper _discordClientWrapper;

    public Discord(ILogger<Discord> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IDiscordClientWrapper discordClientWrapper)
    {
        _logger = logger;
        _xivapiClient = httpClientFactory.CreateClient(XIVAPI_CLIENT_STRING);
        _discordWebhookURL = configuration["discordWebhookURL"] ?? throw new InvalidOperationException();
        _discordClientWrapper = discordClientWrapper;
    }
    
    public async Task Send(Character sender, string? discordMappedName, string message)
    {
        if (!_avatarCache.ContainsKey(sender))
        {
            var httpResponse = await _xivapiClient.GetAsync($"/character/search?name={sender.CharacterName}&server={sender.WorldName}");
            var responseContent = await httpResponse.Content.ReadFromJsonAsync<XIVAPIResponse.RootObject>();
            if (responseContent != null && responseContent.Results.Any())
            {
                _avatarCache.Add(sender, responseContent.Results[0].Avatar);
            }
        }
        _logger.LogInformation("Sending message to Discord: {Content}", message);
        var displayName = sender.CharacterName;
        if (!string.IsNullOrEmpty(discordMappedName))
        {
            displayName = discordMappedName;
        }
        
        var webhookMessage = new RootObject(message, displayName, _avatarCache.TryGetValue(sender, out var value) ? value : _discordClientWrapper.Client.Rest.CurrentUser.GetAvatarUrl(ImageFormat.Png, 512));
        var responseMessage = await _xivapiClient.PostAsJsonAsync(_discordWebhookURL, webhookMessage);
        _logger.LogInformation("Discord returned {StatusCode}: {Content}", responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
    }
}