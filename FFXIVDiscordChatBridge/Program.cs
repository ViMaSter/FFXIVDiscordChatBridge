using FFXIVDiscordChatBridge.Extensions;
using FFXIVHelpers;
using FFXIVHelpers.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace FFXIVDiscordChatBridge
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        private static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) => Logger.Fatal(e.ExceptionObject);
            AppDomain.CurrentDomain.FirstChanceException += (_, e) => Logger.Error(e.Exception);

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
            
            services.AddSingleton<Producer.IDiscordClientWrapper, Producer.DiscordClientWrapper>();
            
            services.AddSingleton<Producer.IFFXIV, Producer.FFXIV>();
            services.AddSingleton<Producer.IDiscord, Producer.Discord>();
            
            services.AddSingleton<UsernameMapping>();
            
            services.AddSingleton<Consumer.FFXIV>();
            services.AddSingleton<Consumer.Discord>();
            
            var serviceProvider = services.BuildServiceProvider();
            
            // ReSharper disable once UnusedVariable - required to stay alive to keep bot working
            var startup = ActivatorUtilities.CreateInstance<Startup>(serviceProvider);
            
            await Task.Delay(-1);
        }
    }
}