using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using FFXIVHelpers.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FFXIVHelpers;

public interface IDiscordClientWrapper
{
    public DiscordSocketClient Client { get; }
    public IMessageChannel? Channel { get; }
}

[ExcludeFromCodeCoverage(Justification = "This class simply serves as Dependency Injection-compatible wrapper for DiscordSocketClient")]
public class DiscordClientWrapper : IDiscordClientWrapper
{
    private readonly ILogger<DiscordClientWrapper> _logger;
    public DiscordSocketClient Client { get; }
    public IMessageChannel? Channel { get; private set; }

    private readonly string _discordToken;
    private readonly string _discordChannelId;

    public DiscordClientWrapper(ILogger<DiscordClientWrapper> logger, IConfiguration configuration)
    {
        _logger = logger;
        _discordToken = configuration["discordToken"] ?? throw new InvalidOperationException();
        _discordChannelId = configuration["discordChannelID"] ?? throw new InvalidOperationException();

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers | GatewayIntents.GuildPresences | GatewayIntents.GuildMessageReactions,
            LogLevel = LogSeverity.Verbose
        });
        Client.Log += (message) =>
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem - Required to redirect logs to NLog
            _logger.Log(message.Severity.ToNLogSeverity(), message.Message);
            return Task.CompletedTask;
        };
        Initialize().Wait();
    }

    private async Task Initialize()
    {
        _logger.LogInformation("Starting Discord client..");

        await Client.LoginAsync(TokenType.Bot, _discordToken);
        _logger.LogInformation("Logged in to Discord");
        await Client.StartAsync();
        _logger.LogInformation("Started Discord client; listening for messages");
        await Task.Run(async () =>
        {
            var ready = false;
            Client.Ready += () =>
            {
                _logger.LogInformation("Discord client is ready");
                ready = true;
                return Task.CompletedTask;
            };
            while (!ready)
            {
                await Task.Delay(100);
            }
        });
        
        Channel = Client.GetChannel(ulong.Parse(_discordChannelId)) as IMessageChannel ?? throw new InvalidOperationException();
    }
}
