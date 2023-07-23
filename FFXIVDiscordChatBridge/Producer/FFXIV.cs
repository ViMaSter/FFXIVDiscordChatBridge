using System.Diagnostics;
using System.Runtime.InteropServices;
using FFXIVDiscordChatBridge.Consumer;
using Microsoft.Extensions.Logging;
using WindowsInput;
using WindowsInput.Native;

namespace FFXIVDiscordChatBridge.Producer;

public class FFXIV : IFFXIVProducer
{
    private readonly ILogger<FFXIV> _logger;
    private readonly IFFXIVConsumer _consumer;
    private readonly InputSimulator _inputSimulator = new();

    [DllImport ("User32.dll")]
    private static extern int SetForegroundWindow(IntPtr point);
    
    public List<string> messages = new();
        
    public FFXIV(ILogger<FFXIV> logger, IFFXIVConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
        var ffxivProcess = Process.GetProcessesByName("ffxiv").Concat(Process.GetProcessesByName("ffxiv_dx11")).FirstOrDefault();
        if (ffxivProcess == null)
        {
            throw new Exception("FFXIV not found");
        }

        var handle = ffxivProcess.MainWindowHandle;
        var result = SetForegroundWindow(handle);
        if (result == 0)
        {
            throw new Exception("Failed to set foreground window");
        }
        
        consumer.OnNewChatMessage += message =>
        {
            messages.Add(message);
            return Task.CompletedTask;
        };
    }
        
    public async Task Send(string message)
    {
        _logger.LogInformation("Sending message to FFXIV: {Message}", message);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        _inputSimulator.Keyboard.Sleep(300);
        _inputSimulator.Keyboard.TextEntry(message + "   "); // add a few spaces to prevent the message from being cut off at low frame rates
        _inputSimulator.Keyboard.Sleep(600);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        // check messages for our message for 2 seconds every 100 ms; if it doesn't show up, throw
        var sw = Stopwatch.StartNew();
        string? found = null;
        while (sw.ElapsedMilliseconds < 2000)
        {
            found = messages.FirstOrDefault(m => m.Contains(message));
            if (found != null)
            {
                messages.Remove(found);
                return;
            }
            await Task.Delay(100);
        }
        if (found == null)
        {
            _logger.LogError("Sent message not found in chat: {Message}; last 5 chat lines: {Last5Lines}", message, string.Join("\n", messages.TakeLast(5)));
        }
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
    }
}