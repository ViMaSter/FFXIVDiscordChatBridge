using Discord.WebSocket;
using FFXIVDiscordChatBridge.Extensions;
using FFXIVHelpers;
using FFXIVHelpers.Extensions;
using FFXIVHelpers.Persistence;
using Microsoft.Extensions.Configuration;
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
            var services = new ServiceCollection();
            services.AddCommandLineConfiguration();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog();
            });
            
            SetupPulsewayExceptionHandling(services.BuildServiceProvider());

            Logger.Info("Starting FFXIVDiscordChatBridge");

            services.AddSingleton<DiscordEmojiConverter>();
            services.AddSingleton<DiscordMessageConverter>();
            
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
                .AddHttpClient(Producer.Discord.XIVAPIClientString, client => client.BaseAddress = new Uri("https://xivapi.com"))
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy);
            
            var serviceProvider = services.BuildServiceProvider();
            
            // ReSharper disable once UnusedVariable - required to stay alive to keep bot working
            var startup = ActivatorUtilities.CreateInstance<Startup>(serviceProvider);
            
            await Task.Delay(-1);
        }

        private static void SetupPulsewayExceptionHandling(ServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()!;
            
            var pulsewayReporter = new PulsewayReporter(
                configuration["pulsewayUsername"],
                configuration["pulsewayPassword"],
                configuration["pulsewayInstanceID"]
            );
            
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                pulsewayReporter.SendMessage("FFXIV - Unhandled Exception", e.ExceptionObject.ToString()!, PulsewayReporter.Priority.Critical);
                Logger.Fatal(e.ExceptionObject);
                Environment.Exit(1);
            };
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                switch (e.Exception)
                {
                    case IOException when e.Exception.InnerException is System.Net.Sockets.SocketException:
                        Logger.Warn(e.Exception, "known socket exception: {Exception}");
                        return;
                    case GatewayReconnectException:
                        Logger.Warn(e.Exception, "known gateway reconnect exception: {Exception}");
                        return;
                    case TaskCanceledException:
                        Logger.Warn(e.Exception, "known exception: {Exception}");
                        return;
                    case Discord.Net.HttpException httpException:
                        if (httpException.Errors.First().Errors.First().Code == "10008")
                        {
                            Logger.Warn(e.Exception, "known exception: {Exception}");
                            return;
                        }
                        break;
                    case System.Net.WebSockets.WebSocketException:
                        Logger.Warn(e.Exception, "known exception: {Exception}");
                        return;
                }

                pulsewayReporter.SendMessage("FFXIV - First Chance Exception", e.Exception.ToString(), PulsewayReporter.Priority.Critical);
                Logger.Fatal(e.Exception);
                Environment.Exit(2);
            };
            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                pulsewayReporter.SendMessage("FFXIV - Unobserved Task Exception", e.Exception.ToString(), PulsewayReporter.Priority.Critical);
                Logger.Error(e.Exception);
                Environment.Exit(3);
            };
        }
    }
}