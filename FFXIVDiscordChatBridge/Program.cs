using FFXIVDiscordChatBridge.Consumer;
using FFXIVDiscordChatBridge.Producer;
using NLog;
using NLog.Fluent;

namespace FFXIVDiscordChatBridge;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    private static string DiscordChannelID;

    private static Logger logger;

    [STAThread]
    static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_CurrentDomain_UnhandledException);

        logger = LogManager.GetCurrentClassLogger();
        logger.Info("Starting FFXIVDiscordChatBridge");
        
        var args = Environment.GetCommandLineArgs();
        var parameters = args
            .Where(arg => arg.StartsWith("--"))
            .Select(arg => arg.Split('='))
            .ToDictionary(arg => arg[0], arg => arg[1]);
            
        if (!parameters.ContainsKey("--discordChannelID"))
        {
            throw new Exception("Missing --discordChannelID parameter");
        }
        DiscordChannelID = parameters["--discordChannelID"];

        var ffxivProducer = new Producer.FFXIV();
        var ffxivListener = new Consumer.FFXIV(OnNewMessage);
        ffxivListener.Start();
        
        ffxivProducer.Send("hello").ConfigureAwait(true).GetAwaiter().GetResult();
    }

    private static void OnNewMessage(string s)
    {
        logger.Info("Write to Discord:");
        logger.Info(s);
    }
    
    static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        logger.Error(e.ExceptionObject);
    }
}