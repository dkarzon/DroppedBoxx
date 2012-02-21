using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        [DllImport(dllName)]
        internal static extern GpStatus GdipCloneBrush(GpBrush brush, out GpBrush cloneBrush);

        [DllImport(dllName)]
        internal static extern GpStatus GdipDeleteBrush(GpBrush brush);

        [DllImport(dllName)]
        internal static extern GpStatus GdipDeleteBrush(GpSolidFill brush);

        [DllImport(dllName)]
        internal static extern GpStatus  GdipGetBrushType(GpBrush brush, out BrushType type);



        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateHatchBrush(HatchStyle hatchstyle, int forecol, int backcol, out GpHatch brush);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetHatchStyle(GpHatch brush, out HatchStyle hatchstyle);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetHatchForegroundColor(GpHatch brush, out int forecol);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetHatchBackgroundColor(GpHatch brush, out int backcol);


        [DllImport(dllName)]
        internal static extern GpStatus
        GdipCreateTexture(GpImage image, WrapMode wrapmode, out GpTexture texture);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateTexture2(GpImage image, WrapMode wrapmode, float x, float y, float width, float height, out GpTexture texture);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateTextureIA(GpImage image,  GpImageAttributes imageAttributes, float x, float y, float width, float height, out GpTexture texture);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateTexture2I(GpImage image, WrapMode wrapmode, int x, int y, int width, int height, out GpTexture texture);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateTextureIAI(GpImage image, GpImageAttributes imageAttributes, int x, int y, int width, int height, out GpTexture texture);


        [DllImport(dllName)]
        internal static extern GpStatus GdipGetTextureTransform(GpTexture brush, GpMatrix matrix);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetTextureTransform(GpTexture brush, GpMatrix matrix);

        [DllImport(dllName)]
        internal static extern GpStatus  GdipResetTextureTransform(GpTexture brush);

        [DllImport(dllName)]
        internal static extern GpStatus GdipMultiplyTextureTransform(GpTexture brush, GpMatrix matrix,MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus GdipTranslateTextureTransform(GpTexture brush, float dx, float dy,  MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus GdipScaleTextureTransform(GpTexture brush, float sx, float sy,  MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus GdipRotateTextureTransform(GpTexture brush, float angle, MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetTextureWrapMode(GpTexture brush, WrapMode wrapmode);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetTextureWrapMode(GpTexture brush, out WrapMode wrapmode);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetTextureImage(GpTexture brush, out GpImage image);


        [DllImport(dllName)] internal static extern GpStatus GdipCreateSolidFill(int color, out GpSolidFill brush);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetSolidFillColor(GpSolidFill brush, int color);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetSolidFillColor(GpSolidFill brush, out int color);


        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateLineBrush(ref PointF point1,
            ref PointF point2,
            int color1, int color2,
            WrapMode wrapMode,
            out GpLineGradient lineGradient);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateLineBrushI(ref Point point1,
            ref Point point2,
            int color1, int color2,
            WrapMode wrapMode,
            out GpLineGradient lineGradient);

        #region not completely supported
        //[DllImport(dllName)]
        //internal static extern GpStatus
        //    GdipCreateLineBrushFromRect(ref RectangleF rect,
        //    int color1, int color2,
        //    LinearGradientMode mode,
        //    WrapMode wrapMode,
        //    out GpLineGradient lineGradient);

        //[DllImport(dllName)]
        //internal static extern GpStatus
        //    GdipCreateLineBrushFromRectI(ref Rectangle rect,
        //    int color1, int color2,
        //    LinearGradientMode mode,
        //    WrapMode wrapMode,
        //    out GpLineGradient lineGradient);

        //[DllImport(dllName)]
        //internal static extern GpStatus
        //    GdipCreateLineBrushFromRectWithAngle(ref RectangleF rect,
        //    int color1, int color2,
        //    float angle,
        //    bool isAngleScalable,
        //    WrapMode wrapMode,
        //    out GpLineGradient lineGradient);

        //[DllImport(dllName)]
        //internal static extern GpStatus
        //    GdipCreateLineBrushFromRectWithAngleI(ref Rectangle rect,
        //    int color1, int color2,
        //    float angle,
        //    bool isAngleScalable,
        //    WrapMode wrapMode,
        //    out GpLineGradient lineGradient);
        #endregion

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineColors(GpLineGradient brush, int color1, int color2);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineColors(GpLineGradient brush, int[] colors);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineRect(GpLineGradient brush, out RectangleF rect);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineRectI(GpLineGradient brush, out Rectangle rect);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineGammaCorrection(GpLineGradient brush, bool useGammaCorrection);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineGammaCorrection(GpLineGradient brush, out bool useGammaCorrection);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineBlendCount(GpLineGradient brush, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineBlend(GpLineGradient brush, float[] blendfactors, float[] positions,
            int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineBlend(GpLineGradient brush, float[] blend,
            float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLinePresetBlendCount(GpLineGradient brush, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLinePresetBlend(GpLineGradient brush, int[] blend,
            float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLinePresetBlend(GpLineGradient brush, int[] blend,
            float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineSigmaBlend(GpLineGradient brush, float focus, float scale);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineLinearBlend(GpLineGradient brush, float focus, float scale);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineWrapMode(GpLineGradient brush, WrapMode wrapmode);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineWrapMode(GpLineGradient brush, out WrapMode wrapmode);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetLineTransform(GpLineGradient brush, out GpMatrix matrix);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetLineTransform(GpLineGradient brush, GpMatrix matrix);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipResetLineTransform(GpLineGradient brush);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipMultiplyLineTransform(GpLineGradient brush, GpMatrix matrix,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipTranslateLineTransform(GpLineGradient brush, float dx, float dy,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipScaleLineTransform(GpLineGradient brush, float sx, float sy,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipRotateLineTransform(GpLineGradient brush, float angle,
            MatrixOrder order);

        //----------------------------------------------------------------------------
        // PathGradientBrush APIs
        //----------------------------------------------------------------------------

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreatePathGradient(PointF[] points,
            int count,
            WrapMode wrapMode,
            out GpPathGradient polyGradient);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreatePathGradientI(Point[] points,
            int count,
            WrapMode wrapMode,
            out GpPathGradient polyGradient);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreatePathGradientFromPath(GpPath path,
            out GpPathGradient polyGradient);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientCenterColor(
            GpPathGradient brush, out int color);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientCenterColor(
            GpPathGradient brush, int colors);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientSurroundColorsWithCount(
            GpPathGradient brush, int[] color, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientSurroundColorsWithCount(
            GpPathGradient brush,
            int[] color, ref int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientPath(GpPathGradient brush, out GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientPath(GpPathGradient brush, GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientCenterPoint(
            GpPathGradient brush, out PointF points);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientCenterPointI(
            GpPathGradient brush, out Point points);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientCenterPoint(
            GpPathGradient brush, ref PointF point);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientCenterPointI(
            GpPathGradient brush, ref Point point);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientRect(GpPathGradient brush, out RectangleF rect);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientRectI(GpPathGradient brush, out Rectangle rect);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientPointCount(GpPathGradient brush, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientSurroundColorCount(GpPathGradient brush, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientGammaCorrection(GpPathGradient brush,
            bool useGammaCorrection);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientGammaCorrection(GpPathGradient brush,
            out bool useGammaCorrection);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientBlendCount(GpPathGradient brush,
            out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientBlend(GpPathGradient brush,
            float[] blend, float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientBlend(GpPathGradient brush,
            float[] blend, float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientPresetBlendCount(GpPathGradient brush, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientPresetBlend(GpPathGradient brush, int[] blend,
            float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientPresetBlend(GpPathGradient brush, int[] blend,
            float[] positions, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientSigmaBlend(GpPathGradient brush, float focus, float scale);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientLinearBlend(GpPathGradient brush, float focus, float scale);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientWrapMode(GpPathGradient brush,
            out WrapMode wrapmode);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientWrapMode(GpPathGradient brush,
            WrapMode wrapmode);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientTransform(GpPathGradient brush,
            out GpMatrix matrix);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientTransform(GpPathGradient brush,
            GpMatrix matrix);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipResetPathGradientTransform(GpPathGradient brush);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipMultiplyPathGradientTransform(GpPathGradient brush,
            GpMatrix matrix,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipTranslatePathGradientTransform(GpPathGradient brush, float dx, float dy,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipScalePathGradientTransform(GpPathGradient brush, float sx, float sy,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipRotatePathGradientTransform(GpPathGradient brush, float angle,
            MatrixOrder order);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipGetPathGradientFocusScales(GpPathGradient brush, out float xScale,
            out float yScale);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetPathGradientFocusScales(GpPathGradient brush, float xScale,
            float yScale);

    }
}
