using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFXIVDiscordChatBridge.Producer;

class FFXIV : WithLogger
{
    [DllImport ("User32.dll")]
    static extern int SetForegroundWindow(IntPtr point);
        
    public FFXIV()
    {
        var ffxivProcess = Process.GetProcessesByName("ffxiv").FirstOrDefault();
        if (ffxivProcess == null)
        {
            ffxivProcess = Process.GetProcessesByName("ffxiv_dx11").FirstOrDefault();
            if (ffxivProcess == null)
            {
                throw new Exception("FFXIV not found");
            }
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
        _logger.Info("Sending message to FFXIV: {message}", message);
        SendKeys.SendWait("~");
        await Task.Delay(200);
        SendKeys.SendWait(message);
        await Task.Delay(200);
        SendKeys.SendWait("~");
    }
}