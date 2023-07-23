using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using WindowsInput;
using WindowsInput.Native;

namespace FFXIVDiscordChatBridge.Producer;

public class FFXIV : IFFXIV
{
    private readonly ILogger<FFXIV> _logger;
    private readonly InputSimulator _inputSimulator = new();

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
        
    public Task Send(string message)
    {
        _logger.LogInformation("Sending message to FFXIV: {Message}", message);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        _inputSimulator.Keyboard.Sleep(300);
        _inputSimulator.Keyboard.TextEntry(message + "   "); // add a few spaces to prevent the message from being cut off at low frame rates
        _inputSimulator.Keyboard.Sleep(600);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
        _inputSimulator.Keyboard.Sleep(300);
        return Task.CompletedTask;
    }
}