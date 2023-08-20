using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FFXIVDiscordChatBridge;

internal abstract class NativeWindowHelper
{
    private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
        IntPtr lParam);
        
#pragma warning disable CA2101 (False negative)
    [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int GetWindowTextA(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
#pragma warning restore CA2101

    private static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
    {
        var handles = new List<IntPtr>();

        foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
            EnumThreadWindows(thread.Id, 
                (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

        return handles;
    }

    public static bool IsShowingNoError(int processID)
    {
        return EnumerateProcessWindowHandles(processID)
            .Select(x =>
            {
                var sb = new StringBuilder(1024);
                GetWindowTextA(x, sb, sb.Capacity);
                return sb.ToString();
            })
            .Count(title => title == "FINAL FANTASY XIV") == 1;
    }
}