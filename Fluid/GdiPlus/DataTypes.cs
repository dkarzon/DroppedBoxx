using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Drawing.GdiPlus
{
    internal delegate int NotificationHookProc(out IntPtr token);
    internal delegate void NotificationUnhookProc(IntPtr token);


    [StructLayout(LayoutKind.Sequential)]
    internal class GdiplusStartupInput
    {
        public uint GdiplusVersion;             // Must be 1  (or 2 for the Ex version)
        int DebugEventCallback; // Ignored on free builds
        public bool SuppressBackgroundThread;     // FALSE unless you're prepared to call 
        // the hook/unhook functions properly
        public bool SuppressExternalCodecs;       // FALSE unless you want GDI+ only to use
        // its internal image codecs.

        public GdiplusStartupInput(
            int debugEventCallback,
            bool suppressBackgroundThread,
            bool suppressExternalCodecs)
        {
            GdiplusVersion = 1;
            DebugEventCallback = debugEventCallback;
            SuppressBackgroundThread = suppressBackgroundThread;
            SuppressExternalCodecs = suppressExternalCodecs;
        }
        public GdiplusStartupInput()
            : this(0, false, false)
        {
        }

    }

    internal enum GdiplusStartupParams : uint
    {
        GdiplusStartupDefault = 0,
        GdiplusStartupNoSetRound = 1,
        GdiplusStartupSetPSValue = 2,
        GdiplusStartupTransparencyMask = 0xFF000000
    }

    // Output structure for GdiplusStartup()

    [StructLayout(LayoutKind.Sequential)]
    internal struct GdiplusStartupOutput
    {
        // The following 2 fields are NULL if SuppressBackgroundThread is FALSE.
        // Otherwise, they are functions which must be called appropriately to
        // replace the background thread.
        //
        // These should be called on the application's main message loop - i.e.
        // a message loop which is active for the lifetime of GDI+.
        // "NotificationHook" should be called before starting the loop,
        // and "NotificationUnhook" should be called after the loop ends.

        IntPtr /*NotificationHookProc*/ NotificationHook;
        IntPtr /*NotificationUnhookProc*/ NotificationUnhook;
    };

    //---------------------------------------------------------------------------
    // Encoder Parameter structure
    //---------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    internal struct EncoderParameter
    {
        public Guid Guid;               // GUID of the parameter
        public uint NumberOfValues;     // Number of the parameter values
        public uint Type;               // Value type, like ValueTypeLONG  etc.
        IntPtr Value;              // A pointer to the parameter values
    }

    //---------------------------------------------------------------------------
    // Encoder Parameters structure
    //---------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    internal class EncoderParameters
    {
        public uint Count;                      // Number of parameters in this structure
        [MarshalAs(UnmanagedType.ByValArray)]
        public EncoderParameter[] Parameters;          // Parameter values
    };
}
