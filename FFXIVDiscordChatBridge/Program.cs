using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace FFXIVDiscordChatBridge
{
    static class ServiceExtensions
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
    static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Logger.Fatal(e.ExceptionObject);

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