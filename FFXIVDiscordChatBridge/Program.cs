using FFXIVDiscordChatBridge.Producer;
using NLog;

namespace FFXIVDiscordChatBridge
{
    static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomain_CurrentDomain_UnhandledException;

            Logger.Info("Starting FFXIVDiscordChatBridge");
        
            var args = Environment.GetCommandLineArgs();
            var parameters = args
                .Where(arg => arg.StartsWith("--"))
                .Select(arg => arg.Split('='))
                .ToDictionary(arg => arg[0], arg => arg[1]);
            
            if (!parameters.ContainsKey("--discordChannelID"))
            {
                throw new Exception("Missing --discordChannelID parameter");
            }
            if (!parameters.ContainsKey("--discordToken"))
            {
                throw new Exception("Missing --discordToken parameter");
            }
            if (!parameters.ContainsKey("--ffxivChannelCode"))
            {
                throw new Exception("Missing --ffxivChannelCode parameter");
            }
            var discordChannelId = parameters["--discordChannelID"];
            var discordToken = parameters["--discordToken"];
            
            var ffxivChannelCode = parameters["--ffxivChannelCode"];

            // setup discord singleton
            var discordWrapper = new DiscordClientWrapper(discordToken, discordChannelId);
            await discordWrapper.Initialize();
            
            // setup producers
            var ffxivProducer = new Producer.FFXIV();
            var discordProducer = new Producer.Discord(discordWrapper);
        
            // setup consumers            
            var ffxivConsumer = new Consumer.FFXIV(ffxivChannelCode, async (message) =>
            {
                await discordProducer.Send(message);
            });
            var discordConsumer = new Consumer.Discord(discordWrapper);
        
            // start consuming messages from FFXIV
            var ffxivConsumerTask = ffxivConsumer.Start();

            await Task.Delay(-1);
        }
    
        static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject);
        }
    }
}