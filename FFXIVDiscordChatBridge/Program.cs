using FFXIVDiscordChatBridge.Extensions;
using FFXIVHelpers;
using FFXIVHelpers.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace FFXIVDiscordChatBridge
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        private static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                Logger.Fatal(e.ExceptionObject);
                Environment.Exit(1);
            };
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                Logger.Error(e.Exception);
            };

            Logger.Info("Starting FFXIVDiscordChatBridge");
            
            var services = new ServiceCollection();
            services.AddCommandLineConfiguration();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog();
            });

            services.AddSingleton<DiscordEmojiConverter>();
            services.AddSingleton<IPersistence, FilePersistence>();
            
            services.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();
            
            services.AddSingleton<Producer.IFFXIV, Producer.FFXIV>();
            services.AddSingleton<Producer.IDiscord, Producer.Discord>();
            
            services.AddSingleton<UsernameMapping>();
            
            services.AddSingleton<Consumer.FFXIV>();
            services.AddSingleton<Consumer.Discord>();
            
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner execution times out
                .RetryAsync(3);

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);  

            services
                .AddHttpClient(Producer.Discord.XIVAPI_CLIENT_STRING, client => client.BaseAddress = new Uri("https://xivapi.com"))
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy);
            
            var serviceProvider = services.BuildServiceProvider();
            
            // ReSharper disable once UnusedVariable - required to stay alive to keep bot working
            var startup = ActivatorUtilities.CreateInstance<Startup>(serviceProvider);
            
            await Task.Delay(-1);
        }
    }
}