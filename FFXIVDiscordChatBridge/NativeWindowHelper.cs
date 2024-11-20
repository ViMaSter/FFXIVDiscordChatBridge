using System.Diagnostics;
using System.Drawing.Imaging;
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
        
        var instances = EnumerateProcessWindowHandles(processID)
            .Select(x =>
            {
                var sb = new StringBuilder(1024);
                GetWindowTextA(x, sb, sb.Capacity);
                return new {handle = x, title = sb.ToString()};
            });
        var output = instances.Count(instance => instance.title == "FINAL FANTASY XIV") == 1;
        button1_Click(instances.First(instance => instance.title == "FINAL FANTASY XIV").handle);
        return output;
    }
    
    
    ///////////
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
       public int Left, Top, Right, Bottom;

       public RECT(int left, int top, int right, int bottom)
       {
         Left = left;
         Top = top;
         Right = right;
         Bottom = bottom;
       }

       public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

       public int X
       {
         get { return Left; }
         set { Right -= (Left - value); Left = value; }
       }

       public int Y
       {
         get { return Top; }
         set { Bottom -= (Top - value); Top = value; }
       }

       public int Height
       {
         get { return Bottom - Top; }
         set { Bottom = value + Top; }
       }

       public int Width
       {
         get { return Right - Left; }
         set { Right = value + Left; }
       }

       public System.Drawing.Point Location
       {
         get { return new System.Drawing.Point(Left, Top); }
         set { X = value.X; Y = value.Y; }
       }

       public System.Drawing.Size Size
       {
         get { return new System.Drawing.Size(Width, Height); }
         set { Width = value.Width; Height = value.Height; }
       }

       public static implicit operator System.Drawing.Rectangle(RECT r)
       {
         return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
       }

       public static implicit operator RECT(System.Drawing.Rectangle r)
       {
         return new RECT(r);
       }

       public static bool operator ==(RECT r1, RECT r2)
       {
         return r1.Equals(r2);
       }

       public static bool operator !=(RECT r1, RECT r2)
       {
         return !r1.Equals(r2);
       }

       public bool Equals(RECT r)
       {
         return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
       }

       public override bool Equals(object obj)
       {
         if (obj is RECT)
           return Equals((RECT)obj);
         else if (obj is System.Drawing.Rectangle)
           return Equals(new RECT((System.Drawing.Rectangle)obj));
         return false;
       }

       public override int GetHashCode()
       {
         return ((System.Drawing.Rectangle)this).GetHashCode();
       }

       public override string ToString()
       {
         return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
       }
    }
    
    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
    public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
    
    // DwmGetWindowAttribute 
    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    public static void button1_Click(IntPtr sourceWindowHandle)
    {
        
      RECT windowRect;
      GetWindowRect(sourceWindowHandle, out windowRect);
      RECT extendedWindowRect;
      DwmGetWindowAttribute(sourceWindowHandle, 9, out extendedWindowRect, Marshal.SizeOf(typeof(RECT)));
      int borderSize = (windowRect.Right - windowRect.Left - extendedWindowRect.Right + extendedWindowRect.Left) / 2;
      int titleBarSize = (windowRect.Bottom - windowRect.Top - extendedWindowRect.Bottom + extendedWindowRect.Top) - borderSize;
      var abmp = new Bitmap(extendedWindowRect.Width, extendedWindowRect.Height, PixelFormat.Format32bppArgb);
      Graphics gfxBmp = Graphics.FromImage(abmp);
      IntPtr hdcBitmap = gfxBmp.GetHdc();
      PrintWindow(sourceWindowHandle, hdcBitmap, 0);

      gfxBmp.ReleaseHdc(hdcBitmap);
      gfxBmp.Dispose();
      Bitmap bmp = abmp.Clone(new Rectangle(borderSize, titleBarSize+borderSize, extendedWindowRect.Width-borderSize*2, extendedWindowRect.Height- (titleBarSize+borderSize)), abmp.PixelFormat);

      
      abmp.Save("output2.png", ImageFormat.Png);
      bmp.Save("output3.png", ImageFormat.Png);

        var a = 2;
        // pictureBox1.Image = sourceBitmap;
        // pictureBox2.Image = destBitmap;
    }
}