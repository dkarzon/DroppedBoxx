using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        [DllImport(dllName)]
        extern static internal GpStatus GdipCreatePen1(int color, float width, Unit unit, out GpPen pen);

        [DllImport(dllName)]
        extern static internal GpStatus GdipCreatePen1(int color, float width, Unit unit, out IntPtr hPen);

        [DllImport(dllName)]
        extern static internal GpStatus GdipCreatePen2(GpBrush brush, float width, Unit unit, out GpPen pen);

        [DllImport(dllName)]
        extern static internal GpStatus GdipClonePen(GpPen pen, out GpPen clonepen);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDeletePen(GpPen pen);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenWidth(GpPen pen, float width);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenWidth(GpPen pen, out float width);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenUnit(GpPen pen, Unit unit);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenUnit(GpPen pen, out Unit unit);

        [DllImport(dllName)]
        extern static internal GpStatus
        GdipSetPenLineCap197819(GpPen pen, LineCap startCap, LineCap endCap,  DashCap dashCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenStartCap(GpPen pen, LineCap startCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenEndCap(GpPen pen, LineCap endCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenDashCap197819(GpPen pen, DashCap dashCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenStartCap(GpPen pen, out LineCap startCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenEndCap(GpPen pen, out LineCap endCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenDashCap197819(GpPen pen, out DashCap dashCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenLineJoin(GpPen pen, LineJoin lineJoin);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenLineJoin(GpPen pen, out LineJoin lineJoin);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenCustomStartCap(GpPen pen, GpCustomLineCap customCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenCustomStartCap(GpPen pen, out GpCustomLineCap customCap);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenCustomEndCap(GpPen pen, GpCustomLineCap customCap);

        [DllImport(dllName)]
        extern static internal GpStatus  GdipGetPenCustomEndCap(GpPen pen, out GpCustomLineCap customCap);

        [DllImport(dllName)]
        extern static internal GpStatus  GdipSetPenMiterLimit(GpPen pen, float miterLimit);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenMiterLimit(GpPen pen, out float miterLimit);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenMode(GpPen pen, PenAlignment penMode);

        [DllImport(dllName)]
        extern static internal GpStatus  GdipGetPenMode(GpPen pen, out PenAlignment penMode);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenTransform(GpPen pen, GpMatrix matrix);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenTransform(GpPen pen, out GpMatrix matrix);

        [DllImport(dllName)]
        extern static internal GpStatus GdipResetPenTransform(GpPen pen);

        [DllImport(dllName)]
        extern static internal GpStatus GdipMultiplyPenTransform(GpPen pen, GpMatrix matrix, MatrixOrder order);

        [DllImport(dllName)]
        extern static internal GpStatus GdipTranslatePenTransform(GpPen pen, float dx, float dy, MatrixOrder order);

        [DllImport(dllName)]
        extern static internal GpStatus GdipScalePenTransform(GpPen pen, float sx, float sy, MatrixOrder order);

        [DllImport(dllName)]
        extern static internal GpStatus GdipRotatePenTransform(GpPen pen, float angle, MatrixOrder order);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenColor(GpPen pen, int argb);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenColor(GpPen pen, out int argb);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenBrushFill(GpPen pen, GpBrush brush);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenBrushFill(GpPen pen, out GpBrush brush);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenFillType(GpPen pen, out PenType type);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenDashStyle(GpPen pen, out DashStyle dashstyle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenDashStyle(GpPen pen, DashStyle dashstyle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenDashOffset(GpPen pen, out float offset);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenDashOffset(GpPen pen, float offset);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenDashCount(GpPen pen, out int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenDashArray(GpPen pen, float[] dash, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenDashArray(GpPen pen, float[] dash, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenCompoundCount(GpPen pen, out int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipSetPenCompoundArray(GpPen pen, float[] dash, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipGetPenCompoundArray(GpPen pen, float[] dash, int count);

    }
}
