using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FFXIVDiscordChatBridge;

static class CommandLineConfigurationExtensions
{
    public static void AddCommandLineConfiguration(this IServiceCollection services)
    {
        var args = Environment.GetCommandLineArgs();
        var options = args.Where(arg => arg.StartsWith("--"))
            .Select(arg => arg[2..].Split('='))
            .Select(arg => new KeyValuePair<string, string?>(arg[0], arg[1]));
            
        services.AddSingleton<IConfiguration>(_ => new ConfigurationBuilder()
            .AddInMemoryCollection(options)
            .Build());
            
        services.AddLogging();
    }
}