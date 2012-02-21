using System;

using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        [DllImport(dllName)]
        extern static internal GpStatus GdiplusStartup(
            out IntPtr token,
            GdiplusStartupInput input,
            out GdiplusStartupOutput output);

        [DllImport(dllName)]
        extern static internal void GdiplusShutdown(IntPtr token);

        IntPtr token;
        GdiplusStartupInput input;
        GdiplusStartupOutput output;

        /// <summary>
        /// It's important to call GdiplusStartup before any other GdiPlus method can be called, therefore
        /// a static GdiPlus is instantiated which executes GdiplusStartup once.
        /// </summary>
        private static GdiPlus instance = new GdiPlus();

        /// <summary>
        /// Initializes the GdiPlus interface.
        /// </summary>
        private GdiPlus()
        {
            input = new GdiplusStartupInput();
            GdiPlus.GdiplusStartup(out token, input, out output);
        }

        /// <summary>
        /// Shutdown the gdiplus engine.
        /// This happens when the static GdiPlus instance is destroyed which means the application has shutdown.
        /// </summary>
        ~GdiPlus()
        {
           //if (token!= IntPtr.Zero) GdiPlus.GdiplusShutdown(token);
        }
    }
}
