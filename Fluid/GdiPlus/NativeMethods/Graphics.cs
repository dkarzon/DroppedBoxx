using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        const string dllName = "gdiplus";

        [DllImport(dllName)]
        extern static internal GpStatus GdipFlush(GpGraphics graphics, FlushIntention intention);

        [DllImport(dllName)]
        extern static internal GpStatus GdipCreateFromHDC(HDC hdc, out GpGraphics graphics);

        [DllImport(dllName)]
        extern static internal GpStatus GdipCreateFromHDC2(HDC hdc, IntPtr hDevice, out GpGraphics graphics);

        [DllImport(dllName)]
        extern static internal GpStatus GdipCreateFromHWND(HWND hwnd, out GpGraphics graphics);

        [DllImport(dllName)]
        extern static internal GpStatus GdipCreateFromHWNDICM(HWND hwnd, out GpGraphics graphics);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDeleteGraphics(GpGraphics graphics);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetDC(GpGraphics graphics, out HDC hdc);

        [DllImport(dllName)]
        extern static internal GpStatus GdipReleaseDC(GpGraphics graphics, HDC hdc);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetCompositingMode(GpGraphics graphics, CompositingMode compositingMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetCompositingMode(GpGraphics graphics, out CompositingMode compositingMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetRenderingOrigin(GpGraphics graphics, int x, int y);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetRenderingOrigin(GpGraphics graphics, out int x, out int y);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetCompositingQuality(GpGraphics graphics, CompositingQuality compositingQuality);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetCompositingQuality(GpGraphics graphics, out CompositingQuality compositingQuality);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetSmoothingMode(GpGraphics graphics, SmoothingMode smoothingMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetSmoothingMode(GpGraphics graphics, out SmoothingMode smoothingMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPixelOffsetMode(GpGraphics graphics, PixelOffsetMode pixelOffsetMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPixelOffsetMode(GpGraphics graphics, out PixelOffsetMode pixelOffsetMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetTextRenderingHint(GpGraphics graphics, TextRenderingHint mode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetTextRenderingHint(GpGraphics graphics, out TextRenderingHint mode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetTextContrast(GpGraphics graphics, uint contrast);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetTextContrast(GpGraphics graphics, out uint contrast);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetInterpolationMode(GpGraphics graphics, InterpolationMode interpolationMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetInterpolationMode(GpGraphics graphics, out InterpolationMode interpolationMode);

        [DllImport(dllName)]
        extern static internal GpStatus  GdipSetWorldTransform(GpGraphics graphics, GpMatrix matrix);

        [DllImport(dllName)]
        extern static internal GpStatus GdipResetWorldTransform(GpGraphics graphics);

        [DllImport(dllName)]
        extern static internal GpStatus GdipMultiplyWorldTransform(GpGraphics graphics, GpMatrix matrix, MatrixOrder order);

        [DllImport(dllName)]
        extern static internal GpStatus GdipTranslateWorldTransform(GpGraphics graphics, float dx, float dy, MatrixOrder order);

        [DllImport(dllName)]
        extern static internal GpStatus GdipScaleWorldTransform(GpGraphics graphics, float sx, float sy, MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetClipBounds(GpGraphics graphics, out RectangleF rect);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetClipBoundsI(GpGraphics graphics, out Rectangle rect);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPageUnit(GpGraphics graphics, out Unit unit);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPageScale(GpGraphics graphics, out float scale);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetPageUnit(GpGraphics graphics, Unit unit);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetPageScale(GpGraphics graphics, float scale);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetDpiX(GpGraphics graphics, out float dpi);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetDpiY(GpGraphics graphics, out float dpi);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetClipHrgn(GpGraphics graphics, HRGN hRgn, CombineMode combineMode);


        [DllImport(dllName)]
        internal static extern GpStatus GdipSaveGraphics(GpGraphics graphics, out GraphicsState state);

        [DllImport(dllName)]
        internal static extern GpStatus GdipRestoreGraphics(GpGraphics graphics, GraphicsState state);

    }
}
