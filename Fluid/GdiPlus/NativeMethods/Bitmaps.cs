using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Fluid.Drawing.Runtime.InteropServices.ComTypes;

namespace Fluid.Drawing.GdiPlus
{

    internal partial class GdiPlus
    {
        //----------------------------------------------------------------------------
        // Bitmap APIs
        //----------------------------------------------------------------------------

        //[DllImport(dllName)] internal static extern GpStatus 
        //GdipCreateBitmapFromStream(IStream* stream, out GpBitmap bitmap);

        //[DllImport(dllName)] internal static extern GpStatus 
        //GdipCreateBitmapFromFile(string filename, out GpBitmap bitmap);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateBitmapFromStreamICM(IStream stream, out GpBitmap bitmap);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateBitmapFromFileICM(string filename, out GpBitmap bitmap);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateBitmapFromScan0(int width,
             int height,
             int stride,
             PixelFormat format,
             IntPtr scan0,
             out GpBitmap bitmap);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateBitmapFromGraphics(int width,
                int height,
                GpGraphics target,
                out GpBitmap bitmap);


        //[DllImport(dllName)] internal static extern GpStatus 
        //GdipCreateBitmapFromGdiDib(GDIPCONST BITMAPINFO* gdiBitmapInfo,
        //                           VOID* gdiBitmapData,
        //                           out GpBitmap  bitmap);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateBitmapFromHBITMAP(IntPtr hbm,
               IntPtr hpal,
               out GpBitmap bitmap);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateHBITMAPFromBitmap(GpBitmap bitmap,
               out HBITMAP hbmReturn,
               int background);


        [DllImport(dllName)]
        internal static extern GpStatus
            GdipBitmapLockBits(GpBitmap bitmap,
            Rectangle rect,
            uint flags,
            PixelFormat format,
            BitmapData lockedBitmapData);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipBitmapUnlockBits(GpBitmap bitmap,
        BitmapData lockedBitmapData);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipBitmapGetPixel(GpBitmap bitmap, int x, int y, out int color);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipBitmapSetPixel(GpBitmap bitmap, int x, int y, int color);



        [DllImport(dllName)]
        public extern static GpStatus GdipSetClipRegion(GpGraphics nativeGraphics, GpRegion gpRegion, CombineMode combineMode);

        [DllImport(dllName)]
        public extern static GpStatus GdipSetClipRect(IntPtr nativeGraphics, float p, float p_3, float p_4, float p_5, CombineMode combineMode);

        [DllImport(dllName)]
        public extern static GpStatus GdipCreateFontFromDC(HDC hdc, out GpFont font);

        [DllImport(dllName)]
        public extern static GpStatus GdipDeleteFont(GpFont nativeFont);

        [DllImport(dllName)]
        public extern static GpStatus GdipDrawString(GpGraphics nativeGraphics, string text, int length, GpFont gpFont, RectangleF rect, int p, GpBrush gpBrush);

        [DllImport(dllName)]
        public extern static GpStatus GdipDrawString(GpGraphics nativeGraphics, string text, int length, int gpFont, RectangleF rect, int p, GpBrush gpBrush);


        [DllImport(dllName)]
        public extern static GpStatus GdipGraphicsPlusClear(GpGraphics nativeGraphics, int color);
    }
}
