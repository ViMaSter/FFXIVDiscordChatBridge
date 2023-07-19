using FFXIVDiscordChatBridge.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NLog;

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
            
            services.AddSingleton<Producer.DiscordClientWrapper>();
            
            services.AddSingleton<Producer.FFXIV>();
            services.AddSingleton<Producer.Discord>();
            
            services.AddSingleton<Consumer.FFXIV>();
            services.AddSingleton<Consumer.Discord>();
            
            var serviceProvider = services.BuildServiceProvider();
            
            // ReSharper disable once UnusedVariable - required to stay alive to keep bot working
            var startup = ActivatorUtilities.CreateInstance<Startup>(serviceProvider);
            
            await Task.Delay(-1);
        }
    }
}