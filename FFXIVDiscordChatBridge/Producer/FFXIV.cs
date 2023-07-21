using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace FFXIVDiscordChatBridge.Producer;

public class FFXIV : IFFXIVProducer
{
    private readonly ILogger<FFXIV> _logger;

    [DllImport ("User32.dll")]
    private static extern int SetForegroundWindow(IntPtr point);
        
    public FFXIV(ILogger<FFXIV> logger)
    {
        _logger = logger;
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
    }
        
    public async Task Send(string message)
    {
        _logger.LogInformation("Sending message to FFXIV: {Message}", message);
        SendKeys.SendWait("~");
        await Task.Delay(200);
        SendKeys.SendWait(message);
        await Task.Delay(500);
        SendKeys.SendWait("~");
    }
}