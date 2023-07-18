namespace FFXIVDiscordChatBridge;

public class Startup
{
    // ReSharper disable NotAccessedField.Local - required to stay alive to keep bot working
    private readonly Task _ffxivConsumerTask;
    private readonly Task _discordConsumerTask;
        // ReSharper restore NotAccessedField.Local

    public Startup(Consumer.FFXIV ffxivConsumer, Consumer.Discord discordConsumer)
    {
        _ffxivConsumerTask = ffxivConsumer.Start();
        _discordConsumerTask = discordConsumer.Start();
    }
}